using System;

namespace Project4.Database.Core.Annotations
{
    [AttributeUsage(AttributeTargets.Field)]
    public class Field : Attribute
    {
        public string FieldName { get; set; } = "";
        public bool Primary { get; set; } = false;
        public bool Nullable { get; set; } = false;
        public string FieldType { get; set; }
        public int Size { get; set; } = 0;
        public int DigitSize { get; set; } = 0;

        public Field(string fieldType)
        {
            FieldType = fieldType;
        }

        public Field(string fieldName, string fieldType)
        {
            FieldName = fieldName;
            FieldType = fieldType;
        }
    }
}