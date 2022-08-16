using System.Data;
using System.Data.SqlClient;

namespace Dapper_Order.Context
{
    public class Dappercontext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public Dappercontext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
