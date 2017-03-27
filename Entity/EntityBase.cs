using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Entity
{
    public abstract class EntityBase : IEntity
    {
        public const int PageSize = 10;

        protected abstract string AllFields { get; }
        protected abstract string TableName { get; }
        protected abstract string[] PagedOrderFields { get; }

        protected Dictionary<string, object> allChangedFields = new Dictionary<string, object>();
        protected Dictionary<string, object> keyFields = new Dictionary<string, object>();

        #region Select

        public bool GetEntity()
        {
            List<IEntity> entities = this.GetEntities();
            if (entities == null || entities.Count <= 0)
                return false;

            this.SetSelf(entities[0]);
            return true;
        }
        protected abstract void SetSelf(IEntity entity);

        public List<IEntity> GetEntities()
        {
            return this.GetEntities(new SQLCondition());
        }
        public List<IEntity> GetEntities(SQLCondition sqlCondition)
        {
            string sqlStr = this.buildSelectSQL(sqlCondition.Conditions);
            SqlParameter[] pars = this.buildAllParameters(sqlCondition.Parameters);
            DataTable entityTable = SQLHelper.ExecuteDataTable(sqlStr, pars);
            if (entityTable == null || entityTable.Rows == null || entityTable.Rows.Count <= 0)
                return null;

            return TableToEntities(entityTable);
        }

        private string buildSelectSQL(string additionalCondition)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("{0} {1} {2} {3}", SQLConst.Select, this.AllFields, SQLConst.From, this.TableName);

            string conditionSQL = this.buildAllConditionSQL(additionalCondition);
            if (!string.IsNullOrEmpty(conditionSQL))
                sqlBuilder.AppendFormat(" {0} {1}", SQLConst.Where, conditionSQL);

            return sqlBuilder.ToString();
        }
        private List<IEntity> TableToEntities(DataTable entityTable)
        {
            if (entityTable == null || entityTable.Rows == null || entityTable.Rows.Count <= 0)
                return null;

            List<IEntity> entities = new List<IEntity>();
            foreach (DataRow entityRow in entityTable.Rows)
            {
                IEntity entity = this.RowToEntity(entityRow);
                if (entity == null)
                    continue;

                entities.Add(entity);
            }

            if (entities.Count <= 0)
                return null;

            return entities;
        }
        protected abstract IEntity RowToEntity(DataRow entityRow);

        public List<IEntity> GetPagedEntities(int pageIndex, int pageSize)
        {
            return this.GetPagedEntities(pageIndex, pageSize, new SQLCondition());
        }
        public List<IEntity> GetPagedEntities(int pageIndex, int pageSize, SQLCondition sqlCondition)
        {
            string sqlStr = this.BuildPagedSQL(pageIndex, pageSize, sqlCondition.Conditions);
            SqlParameter[] pars = this.buildAllParameters(sqlCondition.Parameters);
            DataTable entityTable = SQLHelper.ExecuteDataTable(sqlStr);
            if (entityTable == null || entityTable.Rows == null || entityTable.Rows.Count <= 0)
                return null;

            return TableToEntities(entityTable);
        }
        private string BuildPagedSQL(int pageIndex, int pageSize, string additionalConditions)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;

            string orderColumn = string.Empty;
            string orderedSQL = this.BuildTableSQL(additionalConditions, out orderColumn);

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("{0} {1} {2} {3} {4} ({5}) as temp {6} temp.{7} >= {8} {9} temp.{7} <= {10}",
                SQLConst.Select,
                SQLConst.Top,
                pageSize,
                this.AllFields,
                SQLConst.From,
                orderedSQL,
                SQLConst.Where,
                orderColumn,
                startIndex,
                SQLConst.And,
                endIndex);

            return sqlBuilder.ToString();
        }
        private string BuildTableSQL(string additionalConditions, out string orderColumn)
        {
            orderColumn = "NO";

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("{0} {1} {2}({3} {4}) {5} {6}, {7} {8} {9}",
                SQLConst.Select,
                SQLConst.RowNumber,
                SQLConst.Over,
                SQLConst.OrderBy,
                this.GetPagedOrderFields(),
                SQLConst.As,
                orderColumn,
                this.AllFields,
                SQLConst.From,
                this.TableName);

            if (!string.IsNullOrEmpty(additionalConditions))
                sqlBuilder.AppendFormat(" {0} {1}", SQLConst.Where, additionalConditions);

            return sqlBuilder.ToString();
        }
        private string GetPagedOrderFields()
        {
            StringBuilder orderFields = new StringBuilder();

            foreach (string orderField in this.PagedOrderFields)
                orderFields.AppendFormat("{0} {1}{2}", orderField, SQLConst.Desc, SQLConst.FieldSeperator);
            orderFields.Remove(orderFields.Length - SQLConst.FieldSeperator.Length, SQLConst.FieldSeperator.Length);

            return orderFields.ToString();
        }

        #endregion

        #region Delete

        public bool Delete()
        {
            return this.Delete(new SQLCondition());
        }
        public bool Delete(SQLCondition sqlCondition)
        {
            string sqlStr = this.buildDeleteSQL(sqlCondition);
            SqlParameter[] pars = this.buildAllParameters(sqlCondition.Parameters);
            int resultCount = SQLHelper.ExecteNonQuery(sqlStr, pars);

            return resultCount > 0;
        }

        private string buildDeleteSQL(SQLCondition sqlCondition)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("{0} {1} {2}", SQLConst.Delete, SQLConst.From, this.TableName);

            string conditionSQL = this.buildAllConditionSQL(sqlCondition.Conditions);
            if (!string.IsNullOrEmpty(conditionSQL))
                sqlBuilder.AppendFormat(" {0} {1}",SQLConst.Where, conditionSQL);

            return sqlBuilder.ToString();
        }

        #endregion

        #region Insert

        public bool Insert()
        {
            string insertSQL = this.buildInsertSQL();
            SqlParameter[] pars = EntitySqlHelper.buildParameters(this.allChangedFields);
            int resultCount = SQLHelper.ExecteNonQuery(insertSQL, pars);

            return resultCount > 0;
        }

        private string buildInsertSQL()
        {
            StringBuilder intoSQL = new StringBuilder();
            StringBuilder valueSQL = new StringBuilder();
            foreach (KeyValuePair<string, object> item in this.allChangedFields)
            {
                intoSQL.AppendFormat("{0}{1}", item.Key, SQLConst.FieldSeperator);
                valueSQL.AppendFormat("{0}{1}{2}", SQLConst.ParameterizedChar, item.Key, SQLConst.FieldSeperator);
            }
            intoSQL.Remove(intoSQL.Length - SQLConst.FieldSeperator.Length, SQLConst.FieldSeperator.Length);
            valueSQL.Remove(valueSQL.Length - SQLConst.FieldSeperator.Length, SQLConst.FieldSeperator.Length);

            return string.Format("{0} {1} {2}({3}) {4} ({5})",
                SQLConst.Insert,
                SQLConst.Into,
                this.TableName,
                intoSQL.ToString(),
                SQLConst.Values,
                valueSQL.ToString());
        }

        #endregion

        #region Update

        public bool Update()
        {
            string updateSQL = this.buildUpdateSQL();
            SqlParameter[] pars = this.buildAllParameters();
            int resultCount = SQLHelper.ExecteNonQuery(updateSQL, pars);

            return resultCount > 0;
        }

        private string buildUpdateSQL()
        {
            StringBuilder updateSQL = new StringBuilder();
            updateSQL.AppendFormat("{0} {1} {2}", SQLConst.Update, this.TableName, SQLConst.Set);

            StringBuilder setSQL = new StringBuilder();
            foreach (KeyValuePair<string, object> item in this.allChangedFields)
            {
                if (this.keyFields.ContainsKey(item.Key))
                    continue;

                setSQL.AppendFormat(" {0} = {1}{0}{2}", item.Key, SQLConst.ParameterizedChar, SQLConst.FieldSeperator);
            }
            setSQL.Remove(setSQL.Length - SQLConst.FieldSeperator.Length, SQLConst.FieldSeperator.Length);
            updateSQL.Append(setSQL);

            if (this.keyFields.Count > 0)
            {
                updateSQL.AppendFormat(" {0} 1 = 1", SQLConst.Where);
                foreach (KeyValuePair<string, object> item in this.keyFields)
                    updateSQL.AppendFormat(" {0} {1} = {2}{1}", SQLConst.And, item.Key, SQLConst.ParameterizedChar);
            }

            return updateSQL.ToString();
        }

        #endregion

        #region SQL Builder

        private string buildAllConditionSQL(string additionalCondition)
        {
            Dictionary<string, object> allConditionFields = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> item in this.keyFields)
                allConditionFields[item.Key] = item.Value;

            foreach (KeyValuePair<string, object> item in this.allChangedFields)
                allConditionFields[item.Key] = item.Value;

            return EntitySqlHelper.buildConditionSQL(allConditionFields, additionalCondition);
        }

        private SqlParameter[] buildAllParameters()
        {
            return this.buildAllParameters(null);
        }

        private SqlParameter[] buildAllParameters(List<SqlParameter> additionalPars)
        {
            List<SqlParameter> allPars = new List<SqlParameter>();

            Dictionary<string, object> allFields = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> item in this.keyFields)
                allFields[item.Key] = item.Value;
            foreach (KeyValuePair<string, object> item in this.allChangedFields)
                allFields[item.Key] = item.Value;

            return EntitySqlHelper.buildParameters(allFields, additionalPars);
        }

        #endregion
    }
}
