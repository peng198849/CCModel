using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Entity
{
    public sealed class EntitySqlHelper
    {
        private EntitySqlHelper() { }

        public static string buildConditionSQL(Dictionary<string, object> fields)
        {
            return buildConditionSQL(fields, string.Empty);
        }

        public static string buildConditionSQL(Dictionary<string, object> fields, string additionalCondition)
        {
            if (fields.Count <= 0)
            {
                if (string.IsNullOrEmpty(additionalCondition))
                    return null;

                return additionalCondition;
            }

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("1 = 1");

            foreach (KeyValuePair<string, object> item in fields)
                sqlBuilder.AppendFormat(" {0} {1} = {2}{1}", SQLConst.And, item.Key, SQLConst.ParameterizedChar);

            if (!string.IsNullOrEmpty(additionalCondition))
                sqlBuilder.AppendFormat(" {0} {1}", SQLConst.And, additionalCondition);

            return sqlBuilder.ToString();
        }

        public static SqlParameter[] buildParameters(Dictionary<string, object> fields)
        {
            return buildParameters(fields, null);
        }

        public static SqlParameter[] buildParameters(Dictionary<string, object> fields, List<SqlParameter> additionalPars)
        {
            if (fields.Count <= 0)
            {
                if (additionalPars == null || additionalPars.Count <= 0)
                    return null;

                return additionalPars.ToArray();
            }

            List<SqlParameter> pars = new List<SqlParameter>();
            if (additionalPars != null && additionalPars.Count > 0)
                pars.AddRange(additionalPars);

            foreach (KeyValuePair<string, object> item in fields)
            {
                string parameterizedKey = SQLConst.ParameterizedChar + item.Key;
                pars.Add(new SqlParameter(parameterizedKey, item.Value));
            }

            if (pars.Count <= 0)
                return null;

            return pars.ToArray();
        }
    }
}
