using Project4.Database.Core;
using Project4.Database.Core.Annotations;
using Project4.Database.Core.Interfaces;

namespace Project4.Database.Entities
{
    [Entity("Club")]
    public class Club : IEntity
    {
        [Field("Club_Id", Type.Integer, Primary = true)]
        public int Id;

        [Field(Type.Varchar, Size = 45)]
        public string Name;

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