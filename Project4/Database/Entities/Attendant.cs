using Project4.Database.Core.Annotations;
using Project4.Database.Core.Interfaces;
using Type = Project4.Database.Core.Type;

namespace Project4.Database.Entities
{
    [Entity("Attendant")]
    public class Attendant : IEntity
    {
        [Field("Attendant_Id", Type.Integer, Primary = true)]
        public int Id;

        [Field(Type.Integer)]
        public int Attending;

        [Field(Type.Varchar)]
        public string Reason;

        [Field("User_Id", Type.Integer)]
        public int UserId;

        [Field("Event_Id", Type.Integer)]
        public int EventId;

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