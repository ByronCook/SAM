using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project4.Database.Core;
using Project4.Database.Core.Interfaces;

namespace Project4.Database.SQL
{
    internal class SqlBuilder<T> where T : IEntity
    {
        private const string NewLine = "\n";

        private readonly T _entity;
        private readonly TableConfig<T> _tableConfig;
        private readonly Dictionary<string, string> _parameters;
        private readonly Dictionary<string, List<string>> _parameterLists;
        private readonly StringBuilder _query;

        public int Id;
        public string SelectRange = "*";

        public SqlBuilder(TableConfig<T> tableConfig) : this(tableConfig, default(T))
        {

        }

        public SqlBuilder(TableConfig<T> tableConfig, T entity)
        {
            _tableConfig = tableConfig;
            _entity = entity;
            _parameters = new Dictionary<string, string>();
            _parameterLists = new Dictionary<string, List<string>>();
            _query = new StringBuilder();
        }

        public void AddParameter(string key, string value)
        {
            _parameters.Add(key, value);
        }

        public void AddParameters(string key, List<string> values)
        {
            _parameterLists.Add(key, values);
        }

        public void AddParameters(Dictionary<string, string> parameters)
        {
            parameters.ToList().ForEach(x => _parameters[x.Key] = x.Value);
        }

        public string Build(QueryType queryType)
        {
            switch (queryType)
            {
                case QueryType.Select:
                    return BuildSelectQuery();
                case QueryType.SelectCount:
                    SelectRange = "COUNT(*)";
                    return BuildSelectQuery();
                case QueryType.Insert:
                    return BuildInsertQuery();
                case QueryType.Update:
                    return BuildUpdateQuery();
                case QueryType.Delete:
                    return BuildDeleteQuery();
                default:
                    return "";
            }
        }

        private string BuildSelectQuery()
        {
            _query.Append("SELECT ")
                .Append(SelectRange)
                .Append(" FROM ")
                .Append(_tableConfig.name)
                .Append(" WHERE 1=1 ");

            if (_parameters.Count != 0 || _parameterLists.Count != 0)
            {
                foreach (var parameter in _parameters)
                {
                    _query.Append(NewLine);
                    _query.Append(" AND ").Append(parameter.Key).Append(" = '").Append(parameter.Value).Append("'");
                }

                foreach (var parameterList in _parameterLists)
                {
                    _query.Append(NewLine);

                    var first = true;
                    _query.Append(" AND (");

                    foreach (var item in parameterList.Value)
                    {
                        if (first)
                        {
                            _query.Append(parameterList.Key).Append(" = '").Append(item).Append("'");
                            first = false;
                        }
                        else
                        {
                            _query.Append(" OR ").Append(parameterList.Key).Append(" = '").Append(item).Append("'");
                        }
                    }

                    _query.Append(") ");
                }
            }
            else if (Id != 0)
            {
                _query.Append(NewLine);
                _query.Append(" AND ")
                    .Append(_tableConfig.primaryFieldConfig.name)
                    .Append(" = '").Append(Id).Append("'");
            }

            return _query.ToString();
        }

        private string BuildInsertQuery()
        {
            _query.Append("INSERT INTO ")
                .Append(_tableConfig.name)
                .Append(" ")
                .Append(NewLine);

            var fields = new Dictionary<string, string>();
            var fieldNames = new StringBuilder("(");
            var fieldValues = new StringBuilder("VALUES (");

            foreach (var pair in _tableConfig.fields)
            {
                if (pair.Value.primary) continue;
                var value = pair.Value.field.GetValue(_entity);

                switch (value)
                {
                    case null:
                        continue;
                    case DateTime dateValue:
                        var mySqlFormatDate = dateValue.ToString("yyyy-MM-dd HH:mm:ss");
                        fields.Add(pair.Key, mySqlFormatDate);
                        break;
                    default:
                        fields.Add(pair.Key, value.ToString());
                        break;
                }
            }

            var firstField = true;
            foreach (var pair in fields)
            {
                var escapedValue = pair.Value.Replace("\n", Environment.NewLine);
                if (firstField)
                {
                    fieldNames.Append(pair.Key);
                    fieldValues.Append("'").Append(escapedValue).Append("'");
                    firstField = false;
                }
                else
                {
                    fieldNames.Append(", ").Append(pair.Key);
                    fieldValues.Append(", '").Append(escapedValue).Append("'");
                }
            }

            fieldNames.Append(") ");
            fieldValues.Append(") ");

            _query.Append(fieldNames).Append(NewLine);
            _query.Append(fieldValues);

            return _query.ToString();
        }

        private string BuildUpdateQuery()
        {
            var fields = new Dictionary<string, string>();
            var where = "";
            foreach (var pair in _tableConfig.fields)
            {
                if (!pair.Value.primary)
                {
                    var value = pair.Value.field.GetValue(_entity);

                    switch (value)
                    {
                        case null:
                            continue;
                        case DateTime dateValue:
                            var mySqlFormatDate = dateValue.ToString("yyyy-MM-dd HH:mm:ss");
                            fields.Add(pair.Key, mySqlFormatDate);
                            break;
                        default:
                            fields.Add(pair.Key, value.ToString());
                            break;
                    }
                }
                else
                {
                    where = " WHERE " + pair.Key + " = '" + pair.Value.field.GetValue(_entity) + "'";
                }
            }

            _query.Append("UPDATE ")
                .Append(_tableConfig.name)
                .Append(" ")
                .Append(NewLine);

            

            bool firstField = true;
            foreach (var field in fields)
            {
                _query.Append(NewLine);
                if (firstField)
                {
                    _query.Append("SET ").Append(field.Key).Append(" = '").Append(field.Value).Append("'");
                    firstField = false;
                }
                else
                {
                    _query.Append(", ").Append(field.Key).Append(" = '").Append(field.Value).Append("'");
                }
            }

            _query.Append(where);

            return _query.ToString();
        }

        private string BuildDeleteQuery()
        {
            _query.Append("DELETE FROM ")
                .Append(_tableConfig.name);

            var first = true;

            if (Id != 0)
            {
                _query.Append(NewLine);
                _query.Append(" WHERE ")
                    .Append(_tableConfig.primaryFieldConfig.name)
                    .Append(" = '").Append(Id).Append("'");
                first = false;
            }

            foreach (var paramter in _parameters)
            {
                _query.Append(NewLine);
                if (first)
                {
                    _query.Append(" WHERE ").Append(paramter.Key).Append(" = '").Append(paramter.Value).Append("'");
                    first = false;
                }
                else
                {
                    _query.Append(" AND ").Append(paramter.Key).Append(" = '").Append(paramter.Value).Append("'");
                }
            }

            return _query.ToString();
        }
    }
}