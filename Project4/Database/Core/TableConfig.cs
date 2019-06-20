using System.Collections.Generic;
using Project4.Database.Core.Interfaces;

namespace Project4.Database.Core
{
    public class TableConfig<T>
        where T : IEntity
    {
        public string name;
        public FieldConfig<T> primaryFieldConfig;
        public Dictionary<string, FieldConfig<T>> fields;
    }
}