using Project4.Database.Core;
using Project4.Database.Entities;
using Project4.Database.SQL;

namespace Project4.Database.Daos
{
    public class UserDao : Dao<User>
    {
        public bool DoesUserExist(string username)
        {
            var sqlBuilder = new SqlBuilder<User>(TableConfig);
            sqlBuilder.AddParameter("Username", username);

            var query = sqlBuilder.Build(QueryType.SelectCount);
            var totalUsers = ExecuteCount(query);
            
            return totalUsers > 0;
        }
    }
}