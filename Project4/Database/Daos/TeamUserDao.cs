using System.Collections.Generic;
using Project4.Database.Core;
using Project4.Database.Entities;

namespace Project4.Database.Daos
{
    public class TeamUserDao : Dao<TeamUser>
    {
        public List<TeamUser> FindByTeamId(int id)
        {
            return Find("Team_Id", id.ToString());
        }
    }
}