using System;
using System.Data;
using System.Text;

namespace Entity
{
    public sealed class PersonInfo
    {
        private PersonInfo() { }

        public const string Id = "Id";
        public const string Age = "Age";
        public const string Name = "Name";

        public const string TableName = "Person";
        public const string AllFields = "Id, Age, Name";
    }

    public class Person : EntityBase
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;

                if (string.IsNullOrEmpty(value))
                    base.allChangedFields.Remove(PersonInfo.Id);
                else
                {
                    base.allChangedFields[PersonInfo.Id] = (object)value;
                    if (!base.keyFields.ContainsKey(PersonInfo.Id))
                        base.keyFields[PersonInfo.Id] = (object)value;
                }
            }
        }

        private int _age;
        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;

                if (value == int.MinValue)
                    base.allChangedFields.Remove(PersonInfo.Age);
                else
                    base.allChangedFields[PersonInfo.Age] = (object)value;
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                if (string.IsNullOrEmpty(value))
                    base.allChangedFields.Remove(PersonInfo.Name);
                else
                    base.allChangedFields[PersonInfo.Name] = (object)value;
            }
        }

        #region EntityBase

        protected override string AllFields
        {
            get { return PersonInfo.AllFields; }
        }

        protected override string TableName
        {
            get { return PersonInfo.TableName; }
        }

        protected override string[] PagedOrderFields
        {
            get { return new string[] { "Id" }; }
        }

        protected override IEntity RowToEntity(DataRow entityRow)
        {
            Person person = new Person();

            person.Id = ConvertHelper.ToString(entityRow[PersonInfo.Id]);
            person.Name = ConvertHelper.ToString(entityRow[PersonInfo.Name]);
            person.Age = ConvertHelper.ToInt32(entityRow[PersonInfo.Age]);

            return person;
        }

        protected override void SetSelf(IEntity entity)
        {
            Person person = entity as Person;
            if (person == null)
            {
                string errorMessage = string.Format("{0}类型不能转换为{1}类型",
                    entity.GetType().FullName,
                    this.GetType().FullName);
                throw new Exception(errorMessage);
            }

            this.Id = person.Id;
            this.Name = person.Name;
            this.Age = person.Age;
        }

        #endregion

    }
}
