using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccessLayer.Logger
{
    public class DapperContext
    {
        private readonly string _connectionString;
        public DapperContext(IConfiguration configuration)
        {
            _connectionString =
                                Environment.GetEnvironmentVariable("ConnectionStrings__AFSACDBConnection")
                                ?? configuration.GetConnectionString("ConnectionStrings__AFSACDBConnection")
                                ?? throw new InvalidOperationException(
                                    "Connection string 'AFSACDBConnection' is not configured.");
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
