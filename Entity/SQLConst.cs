using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public sealed class SQLConst
    {
        private SQLConst() { }

        public const string ParameterizedChar = "@";
        public const string FieldSeperator = ", ";

        public const string Select = "SELECT";
        public const string From = "FROM";

        public const string Update = "UPDATE";
        public const string Set = "SET";

        public const string Delete = "DELETE";

        public const string Insert = "INSERT";
        public const string Into = "INTO";
        public const string Values = "VALUES";

        public const string Where = "WHERE";
        public const string And = "AND";
        public const string Or = "OR";
        public const string As = "AS";

        public const string OrderBy = "ORDER BY";
        public const string Asc = "ASC";
        public const string Desc = "DESC";

        public const string GroupBy = "GROUP BY";
        public const string Having = "HAVING";

        public const string Top = "TOP";
        public const string Like = "LIKE";
        public const string Over = "OVER";

        public const string RowNumber = "ROW_NUMBER()";
    }
}
