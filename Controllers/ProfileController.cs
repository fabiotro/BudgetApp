using BudgetApp.Data.Repositories;
using BudgetApp.Enums;
using BudgetApp.Extensions;
using BudgetApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BudgetApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IUserRepository<UserModel> _userRepo;

        public ProfileController(
            ILogger<ProfileController> logger,
            IUserRepository<UserModel> userRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            int userId = GetCurrentUserId();
            var user = await _userRepo.GetById(userId);
            if (user == null) return NotFound();

            var vm = new EditProfileViewModel
            {
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IBAN = user.IBAN
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            int userId = GetCurrentUserId();
            var user = await _userRepo.GetById(userId);
            if (user == null) return NotFound();

            try
            {
                user.DisplayName = vm.DisplayName.Trim();
                user.FirstName = string.IsNullOrWhiteSpace(vm.FirstName) ? null : vm.FirstName.Trim();
                user.LastName = string.IsNullOrWhiteSpace(vm.LastName) ? null : vm.LastName.Trim();
                user.IBAN = string.IsNullOrWhiteSpace(vm.IBAN) ? null : vm.IBAN.Trim().Replace(" ", "").ToUpperInvariant();

                await _userRepo.Update(user);

                var toast = new ToastMessageViewModel
                {
                    Title = "Gespeichert",
                    Message = "Profil erfolgreich gespeichert.",
                    Type = ToastType.Success
                };
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(nameof(Edit));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving profile for user {UserId}", userId);
                var toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
                TempData.Put("ToastMsg", toast);
                return View(vm);
            }
        }
    }
}
