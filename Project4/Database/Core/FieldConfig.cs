using System.Reflection;
using Project4.Database.Core.Interfaces;

namespace Project4.Database.Core
{
    public class FieldConfig<T> where T : IEntity
    {
        // Database Config
        public string name;
        public Type type;
        public bool primary;
        public int size;
        public int digitSize;

        // Field in code
        public FieldInfo field;
    }
}