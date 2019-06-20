using System;

namespace Project4.Database.Core.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Entity : Attribute
    {
        public string TableName { get; }

        public Entity(string tableName)
        {
            TableName = tableName;
        }
    }
}