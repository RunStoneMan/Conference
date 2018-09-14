

namespace Infrastructure.Sql.Utils
{
    using System.Data;
    using System.Data.Common;
    using System.Data.Sql;

    public interface IDbConnectionFactory
    {
        DbConnection CreateConnection(string nameOrConnectionString);
    }


    public class SqlDbConnection : IDbConnectionFactory
    {
        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            return new System.Data.SqlClient.SqlConnection(nameOrConnectionString);
        }
    }
}
