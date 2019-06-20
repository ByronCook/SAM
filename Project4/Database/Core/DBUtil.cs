using System.Collections.Generic;
using System.Reflection;
using MySql.Data.MySqlClient;
using Project4.Database.Core.Annotations;
using Project4.Database.Core.Interfaces;

namespace Project4.Database.Core
{
    public class DBUtil
    {
        public static TableConfig<T> CreateTableConfig<T>(System.Type type)
            where T : IEntity
        {
            var entityAttribute = type.GetCustomAttribute(typeof(Entity));

            if (entityAttribute == null)
            {
                return new TableConfig<T>();
            }

            var entityAttr = (Entity)entityAttribute;

            return new TableConfig<T>
            {
                name = entityAttr.TableName,
                fields = new Dictionary<string, FieldConfig<T>>()
            };
        }

        public static FieldConfig<T> CreateFieldConfig<T>(FieldInfo field)
            where T : IEntity
        {
            var fieldAttribute = field.GetCustomAttribute(typeof(Field));

            if (fieldAttribute == null)
            {
                return new FieldConfig<T>();
            }

            var fieldAttr = (Field)fieldAttribute;

            var fieldName = fieldAttr.FieldName == "" ? field.Name : fieldAttr.FieldName;

            var cfg = new FieldConfig<T>
            {
                field = field,
                name = fieldName,
                type = Type.GetByName(fieldAttr.FieldType),
                primary = fieldAttr.Primary,
                size = fieldAttr.Size,
                digitSize = fieldAttr.DigitSize
            };

            return cfg;
        }

        public static MySqlCommand CreateQuery(string sql, DB database)
        {
            return new MySqlCommand(sql, database.GetConnection());
        }
    }
}