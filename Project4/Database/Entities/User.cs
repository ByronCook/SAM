using System;
using Project4.Database.Core.Annotations;
using Project4.Database.Core.Interfaces;
using Type = Project4.Database.Core.Type;

namespace Project4.Database.Entities
{
    [Entity("User")]
    public class User : Java.Lang.Object, IEntity
    {
        [Field("User_Id", Type.Integer, Primary = true)]
        public int Id;

        [Field(Type.Varchar, Size = 45)]
        public string Username;

        [Field(Type.Varchar, Size = 100)]
        public string Firstname;

        [Field(Type.Varchar, Size = 100)]
        public string Lastname;

        [Field(Type.Varchar)]
        public string Email;

        [Field(Type.Varchar, Size = 61)]
        public string PasswordHash;

        [Field(Type.Varchar, Size = 29)]
        public string PasswordSalt;

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