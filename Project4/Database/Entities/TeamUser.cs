using Project4.Database.Core.Annotations;
using Project4.Database.Core.Interfaces;
using Type = Project4.Database.Core.Type;

namespace Project4.Database.Entities
{
    [Entity("TeamUser")]
    public class TeamUser : IEntity
    {
        [Field("TeamUser_Id", Type.Integer, Primary = true)]
        public int Id;

        [Field("User_Id", Type.Integer)]
        public int UserId;

        [Field("Team_Id", Type.Integer)]
        public int TeamId;

        public int GetId()
        {
            return Id;
        }

        public void SetId(int id)
        {
            Id = id;
        }
    }
}