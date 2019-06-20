
using Project4.Database.Core.Annotations;
using Project4.Database.Core.Interfaces;
using Type = Project4.Database.Core.Type;

namespace Project4.Database.Entities
{
    [Entity("Team")]
    public class Team : IEntity
    {
        [Field("Team_Id", Type.Integer, Primary = true)]
        public int Id;

        [Field(Type.Varchar, Size = 45)]
        public string Name;

        [Field("Club_Id", Type.Integer)]
        public int ClubId;

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