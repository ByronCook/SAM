using System.Collections.Generic;

namespace Project4.Database.Core
{
    public class Type
    {
        public const string Boolean = "Boolean";
        public const string Date = "Date";
        public const string DateTime = "DateTime";
        public const string Time = "Time";
        public const string Char = "Char";
        public const string Varchar = "Varchar";
        public const string Text = "Text";
        public const string SmallInt = "SmallInt";
        public const string Integer = "Integer";
        public const string BigInt = "BigInt";
        public const string Decimal = "Decimal";

        public static readonly Type _BOOLEAN = new Type(1);
        public static readonly Type _DATE = new Type();
        public static readonly Type _DATETIME = new Type();
        public static readonly Type _TIME = new Type();
        public static readonly Type _CHAR = new Type(255);
        public static readonly Type _VARCHAR = new Type(255);
        public static readonly Type _TEXT = new Type();
        public static readonly Type _SMALLINT = new Type(5);
        public static readonly Type _INTEGER = new Type(10);
        public static readonly Type _BIGINT = new Type(19);
        public static readonly Type _DECIMAL = new Type(38, 5);

        public static IEnumerable<Type> Values
        {
            get
            {
                yield return _BOOLEAN;
                yield return _DATE;
                yield return _DATETIME;
                yield return _TIME;
                yield return _CHAR;
                yield return _VARCHAR;
                yield return _TEXT;
                yield return _SMALLINT;
                yield return _INTEGER;
                yield return _BIGINT;
                yield return _DECIMAL;
            }
        }

        private Type()
        {

        }

        private Type(int size)
        {
            Size = size;
            NeedSize = true;
        }

        private Type(int size, int digitSize)
        {
            Size = size;
            DigitSize = digitSize;
            NeedSize = true;
            NeedDigitSize = true;
        }

        public int Size { get; private set; }

        public void SetSize(int n)
        {
            Size = n;
            NeedSize = true;
        }

        public int DigitSize { get; private set; }

        public void SetDigitSize(int n)
        {
            DigitSize = n;
            NeedDigitSize = true;
        }

        public bool NeedSize { get; private set; }

        public bool NeedDigitSize { get; private set; }

        public static Type GetByName(string Name)
        {
            switch (Name)
            {
                case Boolean:
                    return _BOOLEAN;
                case Date:
                    return _DATE;
                case DateTime:
                    return _DATETIME;
                case Time:
                    return _TIME;
                case Char:
                    return _CHAR;
                case Varchar:
                    return _VARCHAR;
                case Text:
                    return _TEXT;
                case SmallInt:
                    return _SMALLINT;
                case Integer:
                    return _INTEGER;
                case BigInt:
                    return _BIGINT;
                case Decimal:
                    return _DECIMAL;
            }

            return null;
        }
    }
}