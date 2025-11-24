namespace BudgetApp.Data.Repositories
{
    public class BaseRepository
    {
        protected readonly DapperContext _context;

        public BaseRepository(DapperContext context)
        {
            _context = context;
        }

        protected void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException("Die Id muss grösser als Null sein.", nameof(id));
            }
        }
    }
}
