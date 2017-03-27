//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Entity
//{
//    public class EntityFieldCollection : IEnumerable<KeyValuePair<string, object>>
//    {
//        private Dictionary<string, object> keyFields = new Dictionary<string, object>();
//        public readonly Dictionary<string, object> KeyFields
//        {
//            get { return keyFields; }
//        }

//        private Dictionary<string, object> nonKeyFields = new Dictionary<string, object>();
//        public readonly Dictionary<string, object> NonKeyFields
//        {
//            get { return nonKeyFields; }
//        }

//        private Dictionary<string, object> allChangedFields = new Dictionary<string, object>();
//        public readonly Dictionary<string, object> AllFields
//        {
//            get { return allChangedFields; }
//        }

//        public void AddKeyField(string key, object value)
//        {
//            this.keyFields.Add(key, value);
//            this.allChangedFields.Add(key, value);
//        }

//        public void AddNonKeyField(string key, object value)
//        {
//            this.nonKeyFields.Add(key, value);
//            this.allChangedFields.Add(key, value);
//        }

//        public void Remove(string key)
//        {
//            this.nonKeyFields.Remove(key);
//            this.keyFields.Remove(key);
//            this.allChangedFields.Remove(key);
//        }

//        public void Clear()
//        {
//            this.keyFields.Clear();
//            this.nonKeyFields.Clear();
//            this.allChangedFields.Clear();
//        }
//    }
//}
