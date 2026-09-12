using System.Security.Claims;
using System.Security.Cryptography;
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
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IUserRepository<UserModel> _userRepo;
        private readonly IEmailService _emailService;

        public ProfileController(
            ILogger<ProfileController> logger,
            IUserRepository<UserModel> userRepo,
            IEmailService emailService
        )
        {
            _logger = logger;
            _userRepo = userRepo;
            _emailService = emailService;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // The profile edit form is loaded into a sidebar via fetch(); requests made
        // that way are marked with this header so the controller can return just the
        // partial/JSON the sidebar's JS expects instead of a full page.
        private bool IsAjaxRequest() => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            int userId = GetCurrentUserId();
            var user = await _userRepo.GetById(userId);
            if (user == null)
                return NotFound();

            var vm = new EditProfileViewModel
            {
                Email = user.Email,
                IsEmailConfirmed = user.IsEmailConfirmed,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IBAN = user.IBAN,
            };
            if (IsAjaxRequest())
                return PartialView("_EditPartial", vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                if (IsAjaxRequest())
                    return PartialView("_EditPartial", vm);
                return View(vm);
            }

            int userId = GetCurrentUserId();
            var user = await _userRepo.GetById(userId);
            if (user == null)
                return NotFound();

            try
            {
                user.DisplayName = vm.DisplayName.Trim();
                user.FirstName = string.IsNullOrWhiteSpace(vm.FirstName)
                    ? null
                    : vm.FirstName.Trim();
                user.LastName = string.IsNullOrWhiteSpace(vm.LastName) ? null : vm.LastName.Trim();
                user.IBAN = string.IsNullOrWhiteSpace(vm.IBAN)
                    ? null
                    : vm.IBAN.Trim().Replace(" ", "").ToUpperInvariant();

                await _userRepo.Update(user);

                if (IsAjaxRequest())
                    return Json(
                        new
                        {
                            success = true,
                            title = "Gespeichert",
                            message = "Profil erfolgreich gespeichert.",
                            displayName = user.DisplayName,
                        }
                    );

                var toast = new ToastMessageViewModel
                {
                    Title = "Gespeichert",
                    Message = "Profil erfolgreich gespeichert.",
                    Type = ToastType.Success,
                };
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(nameof(Edit));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving profile for user {UserId}", userId);

                if (IsAjaxRequest())
                    return Json(
                        new
                        {
                            success = false,
                            title = "Fehler",
                            message = "Ein Fehler ist aufgetreten.",
                        }
                    );

                var toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
                TempData.Put("ToastMsg", toast);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmation()
        {
            int userId = GetCurrentUserId();
            var user = await _userRepo.GetById(userId);
            if (user == null)
                return NotFound();

            if (!user.IsEmailConfirmed)
            {
                user.EmailConfirmationToken = Convert.ToHexString(
                    RandomNumberGenerator.GetBytes(32)
                );
                user.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24);
                await _userRepo.Update(user);

                try
                {
                    var confirmUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { token = user.EmailConfirmationToken },
                        Request.Scheme
                    )!;
                    await _emailService.SendConfirmationEmailAsync(
                        user.Email,
                        user.DisplayName,
                        confirmUrl
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error resending confirmation email for user {UserId}",
                        userId
                    );
                }
            }

            if (IsAjaxRequest())
                return Json(
                    new
                    {
                        success = true,
                        title = "Gesendet",
                        message = "Bestätigungs-E-Mail wurde versendet.",
                    }
                );

            TempData.Put(
                "ToastMsg",
                new ToastMessageViewModel
                {
                    Title = "Gesendet",
                    Message = "Bestätigungs-E-Mail wurde versendet.",
                    Type = ToastType.Success,
                }
            );
            return RedirectToAction(nameof(Edit));
        }
    }
}
