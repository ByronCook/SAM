using Project4.Database.Core.Annotations;
using Project4.Database.Core.Interfaces;
using Project4.Database.Core;

namespace Project4.Database.Entities
{
    [Entity("News")]
    public class News : Java.Lang.Object, IEntity
    {
        [Field("News_Id", Type.Integer, Primary = true)]
        public int Id;

        [Field(Type.Varchar, Size = 45)]
        public string Title;

        [Field(Type.Text)]
        public string Body;

        [Field("Team_Id", Type.Integer)]
        public int Team_Id;

        public int GetId()
        {
            return Id;
        }

        public void SetId(int id)
        {
            this.Id = id;
        }
    }
}