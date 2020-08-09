using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using BatisServiceProvider.Services;

namespace BatisAutomationWebApi.Utilities
{
    public sealed class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
            JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
        }


        private sealed class DynamicJsonObject : DynamicObject
        {
            private readonly IDictionary<string, object> _dictionary;

            public IDictionary<string, object> Dictionary
            {
                get { return _dictionary; }
            }

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _dictionary = dictionary;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("{");
                ToString(sb);
                return sb.ToString();
            }

            private void ToString(StringBuilder sb)
            {
                var firstInDictionary = true;
                foreach (var pair in _dictionary)
                {
                    if (!firstInDictionary)
                        sb.Append(",");
                    firstInDictionary = false;
                    var value = pair.Value;
                    var name = pair.Key;
                    if (value is string)
                    {
                        sb.AppendFormat("{0}:\"{1}\"", name, value);
                    }
                    else if (value is IDictionary<string, object>)
                    {
                        new DynamicJsonObject((IDictionary<string, object>)value).ToString(sb);
                    }
                    else if (value is ArrayList)
                    {
                        sb.Append(name + ":[");
                        var firstInArray = true;
                        foreach (var arrayValue in (ArrayList)value)
                        {
                            if (!firstInArray)
                                sb.Append(",");
                            firstInArray = false;
                            if (arrayValue is IDictionary<string, object>)
                                new DynamicJsonObject((IDictionary<string, object>)arrayValue).ToString(sb);
                            else if (arrayValue is string)
                                sb.AppendFormat("\"{0}\"", arrayValue);
                            else
                                sb.AppendFormat("{0}", arrayValue);

                        }
                        sb.Append("]");
                    }
                    else
                    {
                        sb.AppendFormat("{0}:{1}", name, value);
                    }
                }
                sb.Append("}");
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (!_dictionary.TryGetValue(binder.Name, out result))
                {
                    // return null to avoid exception.  caller can check for null this way...
                    result = null;
                    return true;
                }

                result = WrapResultObject(result);
                return true;
            }

            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                if (indexes.Length == 1 && indexes[0] != null)
                {
                    if (!_dictionary.TryGetValue(indexes[0].ToString(), out result))
                    {
                        // return null to avoid exception.  caller can check for null this way...
                        result = null;
                        return true;
                    }

                    result = WrapResultObject(result);
                    return true;
                }

                return base.TryGetIndex(binder, indexes, out result);
            }

            private static object WrapResultObject(object result)
            {
                var dictionary = result as IDictionary<string, object>;
                if (dictionary != null)
                    return new DynamicJsonObject(dictionary);

                var arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    return arrayList[0] is IDictionary<string, object>
                        ? new List<object>(
                            arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)))
                        : new List<object>(arrayList.Cast<object>());
                }

                return result;
            }

        }

    }

    public static class ExpandoExtention
    {
        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
        }
    }

    public class DynamicParamsClass
    {

        private string _sqlQuery;
        public DynamicParamsClass(string query)
        {
            SqlQuery = query;
        }

        public string Id
        {
            set { SelectedValue.Id = value; }
            get { return SelectedValue.Id; }
        }

        public string SqlQuery
        {
            set
            {
                if (value != _sqlQuery)
                {
                    //TODO:Add Execute query here!!
                    if (!string.IsNullOrEmpty(value))
                    {

                        var service = new EnterpriseFormsService();
                        ValidValues = GetValidValues(service.ExecuteQuery(value));
                    }
                }
                _sqlQuery = value;
            }
            get { return _sqlQuery; }
        }

        private List<IdValuePairClass> GetValidValues(IList<Tuple<string, string>> validValues)
        {
            var result = new List<IdValuePairClass>();
            foreach (var val in validValues)
            {
                result.Add(new IdValuePairClass() { Id = val.Item1, Value = val.Item2 });
            }
            return result;
        }

        public IdValuePairClass SelectedValue { set; get; }
        public List<IdValuePairClass> ValidValues { set; get; }
    }

    public class IdValuePairClass
    {
        protected bool Equals(IdValuePairClass other)
        {
            return string.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public string Id { set; get; }
        public string Value { set; get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IdValuePairClass)obj);
        }
    }
    public class TableBookMark
    {
        public string Name { set; get; }
        public string Value { set; get; }
        public string TableName { set; get; }
        public string TableBookmarkId { set; get; }
        public bool? ValueIsNumeric { set; get; }
        public string Id { set; get; }
        public string RowNumber { set; get; }
    }

}