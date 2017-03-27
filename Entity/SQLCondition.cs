using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Entity
{
    public class SQLCondition
    {
        private StringBuilder sqlBuilder = new StringBuilder();
        public string Conditions
        {
            get { return sqlBuilder.ToString(); }
        }

        private List<SqlParameter> parameters = new List<SqlParameter>();
        public List<SqlParameter> Parameters
        {
            get { return parameters; }
        }

        public SQLCondition Start()
        {
            this.sqlBuilder.Append(" 1 = 1");
            return this;
        }

        public SQLCondition And(params string[] conditions)
        {
            return this.concatConditions(SQLConst.And, conditions);
        }

        public SQLCondition Or(params string[] conditions)
        {
            return this.concatConditions(SQLConst.Or, conditions);
        }

        private SQLCondition concatConditions(string concatText, params string[] conditions)
        {
            if (conditions.Length <= 0)
                return this;

            if (conditions.Length == 1)
            {
                this.sqlBuilder.AppendFormat(" {0} {1}", concatText, conditions[0]);
                return this;
            }

            StringBuilder conditionBuilder = new StringBuilder();
            foreach (string item in conditions)
                conditionBuilder.AppendFormat("{0} {1}", item, concatText);
            conditionBuilder.Remove(conditionBuilder.Length - SQLConst.And.Length, SQLConst.And.Length);

            this.sqlBuilder.AppendFormat(" {0} ({1})", concatText, conditionBuilder);

            return this;
        }

        public SQLCondition GreaterThan(string fieldName, object value)
        {
            string parameterizedField = string.Format("{0}{1}_GreaterThan", SQLConst.ParameterizedChar, fieldName);
            this.And(string.Format("{0} > {1}", fieldName, parameterizedField));
            this.parameters.Add(new SqlParameter(parameterizedField, value));

            return this;
        }

        public SQLCondition LessThan(string fieldName, object value)
        {
            string parameterizedField = string.Format("{0}{1}_LessThan", SQLConst.ParameterizedChar, fieldName);
            this.And(string.Format("{0} < {1}", fieldName, parameterizedField));
            this.parameters.Add(new SqlParameter(parameterizedField, value));

            return this;
        }

        public SQLCondition Equal(string fieldName, object value)
        {
            string parameterizedField = string.Format("{0}{1}_Equal", SQLConst.ParameterizedChar, fieldName);
            this.And(string.Format("{0} = {1}", fieldName, parameterizedField));
            this.parameters.Add(new SqlParameter(parameterizedField, value));

            return this;
        }

        public SQLCondition GreaterEqual(string fieldName, object value)
        {
            string parameterizedField = string.Format("{0}{1}_GreaterEqual", SQLConst.ParameterizedChar, fieldName);
            this.And(string.Format("{0} >= {1}", fieldName, parameterizedField));
            this.parameters.Add(new SqlParameter(parameterizedField, value));

            return this;
        }

        public SQLCondition LessEqual(string fieldName, object value)
        {
            string parameterizedField = string.Format("{0}{1}_LessEqual", SQLConst.ParameterizedChar, fieldName);
            this.And(string.Format("{0} <= {1}", fieldName, parameterizedField));
            this.parameters.Add(new SqlParameter(parameterizedField, value));

            return this;
        }

        public SQLCondition NotEqual(string fieldName, object value)
        {
            string parameterizedField = string.Format("{0}{1}_NotEqual", SQLConst.ParameterizedChar, fieldName);
            this.And(string.Format("{0} <> {1}", fieldName, parameterizedField));
            this.parameters.Add(new SqlParameter(parameterizedField, value));

            return this;
        }

        public SQLCondition BetweenAnd(object littleValue, string fieldName, object bigValue)
        {
            string parameterizedLittleField = string.Format("{0}{1}_LittleValue", SQLConst.ParameterizedChar, fieldName);
            this.parameters.Add(new SqlParameter(parameterizedLittleField, littleValue));

            string parameterizedBigField = string.Format("{0}{1}_BigValue", SQLConst.ParameterizedChar, fieldName);
            this.parameters.Add(new SqlParameter(parameterizedBigField, bigValue));

            this.And(string.Format("({0} between {1} and {2})", fieldName, parameterizedLittleField, parameterizedBigField));

            return this;
        }
    }
}
