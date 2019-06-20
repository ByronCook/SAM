using System;
using System.Data;
using Project4.Database.Core.Annotations;
using Project4.Database.Core.Interfaces;
using Type = Project4.Database.Core.Type;

namespace Project4.Database.Entities
{
    [Entity("Event")]
    public class Event : IEntity
    {
        [Field("Event_Id", Type.Integer, Primary = true)]
        public int Id;

        [Field(Type.Varchar, Size = 45)]
        public string Name;

        [Field(Type.Varchar, Size = 256)]
        public string Description;

        [Field("Type", Type.Integer)]
        public int EventType;

        [Field(Type.DateTime)]
        public DateTime StartDate;

        [Field(Type.DateTime)]
        public DateTime EndDate;

        [Field("Team_Id", Type.Integer)]
        public int Team;

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