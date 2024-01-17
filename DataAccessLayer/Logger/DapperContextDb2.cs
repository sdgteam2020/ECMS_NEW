using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccessLayer.Logger
{
    public class DapperContextDb2
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DapperContextDb2(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("AFSACDBConnection2");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
