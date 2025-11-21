using Microsoft.Data.SqlClient;
using System.Data;

namespace BudgetApp.Data
{
    public class DapperContext
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
