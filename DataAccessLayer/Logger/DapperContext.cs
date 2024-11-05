using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccessLayer.Logger
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("AFSACDBConnection");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
        
        // Method to create a connection and start a transaction
        public (IDbConnection, IDbTransaction) CreateConnectionWithTransaction()
        {
            var connection = CreateConnection();
            connection.Open();
            var transaction = connection.BeginTransaction();
            return (connection, transaction);
        }
    }
}
