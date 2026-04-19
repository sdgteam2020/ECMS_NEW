using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccessLayer.Logger
{
    public class DapperContextDb2
    {
        private readonly string _connectionString;
        public DapperContextDb2(IConfiguration configuration)
        {
            _connectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__AFSACDBConnection2")
                ?? configuration.GetConnectionString("ConnectionStrings__AFSACDBConnection2")
                ?? throw new InvalidOperationException(
                    "Connection string 'AFSACDBConnection2' is not configured.");
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
