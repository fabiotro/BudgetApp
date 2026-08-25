using System.Security.Claims;
using BudgetApp.Data.Repositories;
using BudgetApp.Enums;
using BudgetApp.Extensions;
using BudgetApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApp.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ICampRepository<CampModel> _campRepo;
        private readonly ICampUserRepository<CampUserModel> _campUserRepo;
        private readonly ITransactionRepository<TransactionModel> _transactionRepo;
        private readonly ITransactionDocumentRepository<TransactionDocumentModel> _docRepo;
        private readonly IPositionRepository<PositionModel> _positionRepo;

        private static readonly HashSet<string> AllowedContentTypes = new(
            StringComparer.OrdinalIgnoreCase
        )
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/gif",
            "application/pdf",
        };
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public TransactionController(
            ILogger<TransactionController> logger,
            ICampRepository<CampModel> campRepo,
            ICampUserRepository<CampUserModel> campUserRepo,
            ITransactionRepository<TransactionModel> transactionRepo,
            ITransactionDocumentRepository<TransactionDocumentModel> docRepo,
            IPositionRepository<PositionModel> positionRepo
        )
        {
            _logger = logger;
            _campRepo = campRepo;
            _campUserRepo = campUserRepo;
            _transactionRepo = transactionRepo;
            _docRepo = docRepo;
            _positionRepo = positionRepo;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private async Task<bool> HasCampAccessAsync(int campId, int userId)
        {
            var camp = await _campRepo.GetById(campId);
            if (camp == null)
                return false;
            if (camp.CreatedByUserId == userId)
                return true;
            var campUsers = await _campUserRepo.GetByCampId(campId);
            return campUsers.Any(cu => cu.UserId == userId);
        }

        private async Task PopulateFormListsAsync(CreateTransactionViewModel vm)
        {
            vm.CampUsers = (await _campUserRepo.GetByCampId(vm.CampId)).ToList();
            vm.Positions = (await _positionRepo.GetByCampId(vm.CampId)).ToList();
        }

        private async Task SaveAttachmentsAsync(int transactionId, List<IFormFile> attachments)
        {
            foreach (var file in attachments)
            {
                if (file.Length == 0)
                    continue;

                if (!AllowedContentTypes.Contains(file.ContentType))
                {
                    _logger.LogWarning(
                        "Skipping file {FileName}: unsupported content type {ContentType}",
                        file.FileName,
                        file.ContentType
                    );
                    continue;
                }

                if (file.Length > MaxFileSizeBytes)
                {
                    _logger.LogWarning(
                        "Skipping file {FileName}: exceeds max size ({Size} bytes)",
                        file.FileName,
                        file.Length
                    );
                    continue;
                }

                try
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);

                    var doc = new TransactionDocumentModel
                    {
                        TransactionId = transactionId,
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        FileSize = (int)file.Length,
                        FileData = ms.ToArray(),
                    };
                    await _docRepo.Create(doc);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error saving attachment {FileName} for transaction {TransactionId}",
                        file.FileName,
                        transactionId
                    );
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(int campId)
        {
            int userId = GetCurrentUserId();
            if (!await HasCampAccessAsync(campId, userId))
                return Forbid();

            var vm = new CreateTransactionViewModel
            {
                CampId = campId,
                PerformedByUserId = userId,
                Name = string.Empty,
            };
            await PopulateFormListsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateTransactionViewModel vm,
            List<IFormFile>? attachments
        )
        {
            if (!ModelState.IsValid)
            {
                await PopulateFormListsAsync(vm);
                return View(vm);
            }

            int userId = GetCurrentUserId();
            if (!await HasCampAccessAsync(vm.CampId, userId))
                return Forbid();

            try
            {
                var transaction = new TransactionModel
                {
                    CampId = vm.CampId,
                    PerformedByUserId = vm.PerformedByUserId,
                    PositionId = vm.PositionId,
                    Name = vm.Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(vm.Description)
                        ? null
                        : vm.Description.Trim(),
                    Amount = vm.Amount,
                    PaymentSource = vm.PaymentSource,
                    PaymentMethod = vm.PaymentMethod,
                };

                int newId = await _transactionRepo.Create(transaction);

                if (attachments != null && attachments.Count > 0)
                    await SaveAttachmentsAsync(newId, attachments);

                var toast = new ToastMessageViewModel
                {
                    Title = "Gespeichert",
                    Message = "Ausgabe erfolgreich erfasst.",
                    Type = ToastType.Success,
                };
                TempData.Put("ToastMsg", toast);
                return RedirectToAction("Detail", "Camp", new { id = vm.CampId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction for camp {CampId}", vm.CampId);
                var toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
                TempData.Put("ToastMsg", toast);
                await PopulateFormListsAsync(vm);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int userId = GetCurrentUserId();
            var transaction = await _transactionRepo.GetById(id);
            if (transaction == null)
                return NotFound();

            if (!await HasCampAccessAsync(transaction.CampId, userId))
                return Forbid();

            var existingDocs = (await _docRepo.GetByTransactionId(id)).ToList();

            var vm = new EditTransactionViewModel
            {
                Id = id,
                CampId = transaction.CampId,
                PerformedByUserId = transaction.PerformedByUserId,
                PositionId = transaction.PositionId,
                Name = transaction.Name,
                Description = transaction.Description,
                Amount = transaction.Amount,
                PaymentSource = transaction.PaymentSource,
                PaymentMethod = transaction.PaymentMethod,
                ExistingDocuments = existingDocs,
            };
            await PopulateFormListsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            EditTransactionViewModel vm,
            List<IFormFile>? newAttachments
        )
        {
            if (!ModelState.IsValid)
            {
                vm.ExistingDocuments = (await _docRepo.GetByTransactionId(vm.Id)).ToList();
                await PopulateFormListsAsync(vm);
                return View(vm);
            }

            int userId = GetCurrentUserId();
            var existing = await _transactionRepo.GetById(vm.Id);
            if (existing == null)
                return NotFound();

            if (!await HasCampAccessAsync(existing.CampId, userId))
                return Forbid();

            try
            {
                existing.PerformedByUserId = vm.PerformedByUserId;
                existing.PositionId = vm.PositionId;
                existing.Name = vm.Name.Trim();
                existing.Description = string.IsNullOrWhiteSpace(vm.Description)
                    ? null
                    : vm.Description.Trim();
                existing.Amount = vm.Amount;
                existing.PaymentSource = vm.PaymentSource;
                existing.PaymentMethod = vm.PaymentMethod;

                await _transactionRepo.Update(existing);

                if (newAttachments != null && newAttachments.Count > 0)
                    await SaveAttachmentsAsync(vm.Id, newAttachments);

                var toast = new ToastMessageViewModel
                {
                    Title = "Gespeichert",
                    Message = "Ausgabe erfolgreich aktualisiert.",
                    Type = ToastType.Success,
                };
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(
                    nameof(UserTransactions),
                    new { campId = existing.CampId, userId = existing.PerformedByUserId }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction {TransactionId}", vm.Id);
                var toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
                TempData.Put("ToastMsg", toast);
                vm.ExistingDocuments = (await _docRepo.GetByTransactionId(vm.Id)).ToList();
                await PopulateFormListsAsync(vm);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int campId)
        {
            int userId = GetCurrentUserId();
            var transaction = await _transactionRepo.GetById(id);
            if (transaction == null)
                return NotFound();

            if (!await HasCampAccessAsync(transaction.CampId, userId))
                return Forbid();

            try
            {
                await _transactionRepo.Delete(id);

                var toast = new ToastMessageViewModel
                {
                    Title = "Gelöscht",
                    Message = "Ausgabe erfolgreich gelöscht.",
                    Type = ToastType.Success,
                };
                TempData.Put("ToastMsg", toast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction {TransactionId}", id);
                var toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
                TempData.Put("ToastMsg", toast);
            }

            return RedirectToAction("Detail", "Camp", new { id = campId });
        }

        [HttpGet]
        public async Task<IActionResult> UserTransactions(int campId, int userId)
        {
            int currentUserId = GetCurrentUserId();
            if (!await HasCampAccessAsync(campId, currentUserId))
                return Forbid();

            var camp = await _campRepo.GetById(campId);
            if (camp == null)
                return NotFound();

            var allSummaries = await _transactionRepo.GetUserSummariesByCampId(campId);
            var userSummary = allSummaries.FirstOrDefault(s => s.UserId == userId);
            if (userSummary == null)
            {
                userSummary = new UserExpenseSummaryViewModel { UserId = userId };
            }

            var transactions = (await _transactionRepo.GetByUserId(userId, campId)).ToList();

            // Load positions for name lookup
            var positions = (await _positionRepo.GetByCampId(campId)).ToDictionary(p => p.Id);

            var transactionsWithDocs = new List<TransactionWithDocumentsViewModel>();
            foreach (var t in transactions)
            {
                var docs = (await _docRepo.GetByTransactionId(t.Id)).ToList();
                transactionsWithDocs.Add(
                    new TransactionWithDocumentsViewModel
                    {
                        Transaction = t,
                        Documents = docs,
                        PerformedByDisplayName = userSummary.FullName,
                        PositionName =
                            t.PositionId.HasValue
                            && positions.TryGetValue(t.PositionId.Value, out var pos)
                                ? pos.Name
                                : null,
                    }
                );
            }

            var vm = new UserTransactionsViewModel
            {
                Camp = camp,
                UserSummary = userSummary,
                Transactions = transactionsWithDocs,
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            int userId = GetCurrentUserId();
            var doc = await _docRepo.GetById(id);
            if (doc == null)
                return NotFound();

            var transaction = await _transactionRepo.GetById(doc.TransactionId);
            if (transaction == null)
                return NotFound();

            if (!await HasCampAccessAsync(transaction.CampId, userId))
                return Forbid();

            return File(doc.FileData!, doc.ContentType, doc.FileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocument(int id, int transactionId)
        {
            int userId = GetCurrentUserId();

            // Load metadata (no FileData needed)
            var docs = await _docRepo.GetByTransactionId(transactionId);
            var doc = docs.FirstOrDefault(d => d.Id == id);
            if (doc == null)
                return NotFound();

            var transaction = await _transactionRepo.GetById(transactionId);
            if (transaction == null)
                return NotFound();

            if (!await HasCampAccessAsync(transaction.CampId, userId))
                return Forbid();

            try
            {
                await _docRepo.Delete(id);

                var toast = new ToastMessageViewModel
                {
                    Title = "Gelöscht",
                    Message = "Dokument erfolgreich gelöscht.",
                    Type = ToastType.Success,
                };
                TempData.Put("ToastMsg", toast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document {DocumentId}", id);
                var toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
                TempData.Put("ToastMsg", toast);
            }

            return RedirectToAction(nameof(Edit), new { id = transactionId });
        }
    }
}
