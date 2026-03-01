using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ITransactionDocumentRepository<T> where T : TransactionDocumentModel
    {
        // Returns metadata only (no FileData) — for listing documents
        Task<IEnumerable<T>> GetByTransactionId(int transactionId);
        // Returns full record including FileData — for downloads
        Task<T?> GetById(int id);
        Task<int> Create(T document);
        Task<int> Delete(int id);
    }
}
