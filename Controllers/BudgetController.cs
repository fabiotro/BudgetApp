using BudgetApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApp.Controllers
{
    public class BudgetController : Controller
    {
        public IActionResult NewBudget()
        {
            return View(new NewBudgetViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertCamp(NewBudgetViewModel newBudget)
        {
            if (!ModelState.IsValid)
                return View("NewBudget", newBudget);

            return View("NewBudget", newBudget); // Placeholder for further processing
        }
    }
}
