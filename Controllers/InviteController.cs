using System.Security.Claims;
using BudgetApp.Data.Repositories;
using BudgetApp.Enums;
using BudgetApp.Extensions;
using BudgetApp.Models;
using BudgetApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApp.Controllers
{
    [Authorize]
    public class InviteController : Controller
    {
        private readonly ILogger<InviteController> _logger;
        private readonly ICampRepository<CampModel> _campRepo;
        private readonly ICampUserRepository<CampUserModel> _campUserRepo;
        private readonly ICampInviteRepository<CampInviteModel> _inviteRepo;
        private readonly IUserRepository<UserModel> _userRepo;
        private readonly IEmailService _emailService;

        public InviteController(
            ILogger<InviteController> logger,
            ICampRepository<CampModel> campRepo,
            ICampUserRepository<CampUserModel> campUserRepo,
            ICampInviteRepository<CampInviteModel> inviteRepo,
            IUserRepository<UserModel> userRepo,
            IEmailService emailService
        )
        {
            _logger = logger;
            _campRepo = campRepo;
            _campUserRepo = campUserRepo;
            _inviteRepo = inviteRepo;
            _userRepo = userRepo;
            _emailService = emailService;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private static string FormatCampLabel(
            DateTime startDate,
            DateTime endDate,
            string? mainLeader
        ) =>
            $"{startDate:dd.MM.yyyy} – {endDate:dd.MM.yyyy}"
            + (string.IsNullOrWhiteSpace(mainLeader) ? "" : $" ({mainLeader})");

        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            int userId = GetCurrentUserId();
            var invites = (await _inviteRepo.GetPendingByInvitedUserId(userId)).ToList();
            var result = invites.Select(i => new
            {
                i.Id,
                i.CampId,
                InvitedBy = i.InvitedByDisplayName,
                CampName = FormatCampLabel(i.CampStartDate, i.CampEndDate, i.CampMainLeader),
            });
            return Json(new { count = invites.Count, invites = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(SendInviteViewModel vm)
        {
            int userId = GetCurrentUserId();

            var camp = await _campRepo.GetById(vm.CampId);
            if (camp == null)
                return NotFound();

            var campUsers = (await _campUserRepo.GetByCampId(vm.CampId)).ToList();
            bool isMainLeader = campUsers.Any(cu => cu.UserId == userId && cu.IsMainLeader);
            if (!isMainLeader)
            {
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Nur die Hauptleitung kann Einladungen versenden.",
                        Type = ToastType.Error,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = vm.CampId });
            }

            if (!ModelState.IsValid)
            {
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Ungültige E-Mail-Adresse.",
                        Type = ToastType.Error,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = vm.CampId });
            }

            var targetUser = await _userRepo.GetByEmail(vm.Email.Trim().ToLowerInvariant());
            if (targetUser == null)
            {
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Kein Benutzer mit dieser E-Mail-Adresse gefunden.",
                        Type = ToastType.Error,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = vm.CampId });
            }

            if (targetUser.Id == userId)
            {
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Sie können sich nicht selbst einladen.",
                        Type = ToastType.Error,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = vm.CampId });
            }

            if (campUsers.Any(cu => cu.UserId == targetUser.Id))
            {
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = $"{targetUser.DisplayName} ist bereits Mitglied dieses Lagers.",
                        Type = ToastType.Error,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = vm.CampId });
            }

            var existingInvites = await _inviteRepo.GetByCampId(vm.CampId);
            var existingInvite = existingInvites.FirstOrDefault(i =>
                i.InvitedUserId == targetUser.Id
            );
            if (existingInvite != null && existingInvite.Status == InviteStatus.Pending)
            {
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message =
                            $"{targetUser.DisplayName} hat bereits eine ausstehende Einladung.",
                        Type = ToastType.Error,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = vm.CampId });
            }

            try
            {
                if (existingInvite != null)
                {
                    // A declined invite for this CampId+InvitedUserId already exists —
                    // UQ_CampInvite_CampId_InvitedUserId forbids a second row, so revive it.
                    await _inviteRepo.Reinvite(existingInvite.Id, userId);
                }
                else
                {
                    var invite = new CampInviteModel
                    {
                        CampId = vm.CampId,
                        InvitedByUserId = userId,
                        InvitedUserId = targetUser.Id,
                        Status = InviteStatus.Pending,
                    };
                    await _inviteRepo.Create(invite);
                }

                try
                {
                    await _emailService.SendCampInviteEmailAsync(
                        targetUser.Email,
                        targetUser.DisplayName,
                        FormatCampLabel(camp.StartDate, camp.EndDate, camp.MainLeader),
                        Url.Action("Index", "Home", new { openInvites = "1" }, Request.Scheme)!
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error sending invite email to user {UserId} for camp {CampId}",
                        targetUser.Id,
                        vm.CampId
                    );
                }

                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Eingeladen",
                        Message = $"Einladung an {targetUser.DisplayName} gesendet.",
                        Type = ToastType.Success,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending invite to user {UserId} for camp {CampId}",
                    targetUser.Id,
                    vm.CampId
                );
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Ein Fehler ist aufgetreten.",
                        Type = ToastType.Error,
                    }
                );
            }

            return RedirectToAction("Detail", "Camp", new { id = vm.CampId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reinvite(int id)
        {
            int userId = GetCurrentUserId();
            var invite = await _inviteRepo.GetById(id);
            if (invite == null)
                return NotFound();

            var campUsers = (await _campUserRepo.GetByCampId(invite.CampId)).ToList();
            bool isMainLeader = campUsers.Any(cu => cu.UserId == userId && cu.IsMainLeader);
            if (!isMainLeader)
            {
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Nur die Hauptleitung kann Einladungen versenden.",
                        Type = ToastType.Error,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = invite.CampId });
            }

            if (invite.Status != InviteStatus.Declined)
            {
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Diese Einladung kann nicht erneut gesendet werden.",
                        Type = ToastType.Warning,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = invite.CampId });
            }

            try
            {
                await _inviteRepo.Reinvite(id, userId);

                try
                {
                    await _emailService.SendCampInviteEmailAsync(
                        invite.InvitedUserEmail!,
                        invite.InvitedUserDisplayName!,
                        FormatCampLabel(
                            invite.CampStartDate,
                            invite.CampEndDate,
                            invite.CampMainLeader
                        ),
                        Url.Action("Index", "Home", new { openInvites = "1" }, Request.Scheme)!
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending reinvite email for invite {InviteId}", id);
                }

                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Eingeladen",
                        Message = $"Einladung an {invite.InvitedUserDisplayName} erneut gesendet.",
                        Type = ToastType.Success,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reinviting for invite {InviteId}", id);
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Ein Fehler ist aufgetreten.",
                        Type = ToastType.Error,
                    }
                );
            }

            return RedirectToAction("Detail", "Camp", new { id = invite.CampId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            int userId = GetCurrentUserId();
            var invite = await _inviteRepo.GetById(id);

            if (invite == null)
                return NotFound();
            if (invite.InvitedUserId != userId)
                return Forbid();

            if (invite.Status != InviteStatus.Pending)
            {
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Diese Einladung ist nicht mehr ausstehend.",
                        Type = ToastType.Warning,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = invite.CampId });
            }

            try
            {
                await _inviteRepo.UpdateStatus(id, InviteStatus.Accepted);

                var campUser = new CampUserModel
                {
                    CampId = invite.CampId,
                    UserId = userId,
                    IsMainLeader = false,
                };
                await _campUserRepo.Create(campUser);

                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Angenommen",
                        Message = "Einladung angenommen. Willkommen im Lager!",
                        Type = ToastType.Success,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = invite.CampId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting invite {InviteId}", id);
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Ein Fehler ist aufgetreten.",
                        Type = ToastType.Error,
                    }
                );
                return RedirectToAction("Detail", "Camp", new { id = invite.CampId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(int id)
        {
            int userId = GetCurrentUserId();
            var invite = await _inviteRepo.GetById(id);

            if (invite == null)
                return NotFound();
            if (invite.InvitedUserId != userId)
                return Forbid();

            if (invite.Status != InviteStatus.Pending)
            {
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Diese Einladung ist nicht mehr ausstehend.",
                        Type = ToastType.Warning,
                    }
                );
                return RedirectToAction("Index", "Home");
            }

            try
            {
                await _inviteRepo.UpdateStatus(id, InviteStatus.Declined);

                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Abgelehnt",
                        Message = "Einladung abgelehnt.",
                        Type = ToastType.Info,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error declining invite {InviteId}", id);
                TempData.Put(
                    "ToastMsg",
                    new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Ein Fehler ist aufgetreten.",
                        Type = ToastType.Error,
                    }
                );
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
