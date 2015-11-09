using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace University.Utilities
{
    public static class Extensions
    {
        public static bool HasValue<T>(this List<T> lstValues)
        {            
            return (lstValues != null && lstValues.Count > 0);
        }

        public static string SerializeListObject<T>(this List<T> paramList)
        {
            StringWriter stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(paramList.GetType());
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.OmitXmlDeclaration = true;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, writerSettings))
            {
                serializer.Serialize(xmlWriter, paramList);
            }
            return stringWriter.ToString();
        }

        public static bool HasValue(this decimal? value)
        {            
            return (value != null && value > 0);
        }

        public static bool HasValue(this int? value)
        {            
            return (value != null && value > 0);
        }

        public static bool HasValue(this DataSet ds)
        {            
            return (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0);
        }

        public static List<T> ToCollection<T>(this DataTable dt)
        {
            List<T> lst = new List<T>();
            Type tClass = typeof(T);
            PropertyInfo[] pClass = tClass.GetProperties();
            List<DataColumn> dc = dt.Columns.Cast<DataColumn>().ToList();
            T cn;
            foreach (DataRow item in dt.Rows)
            {
                cn = (T)Activator.CreateInstance(tClass);
                foreach (PropertyInfo pc in pClass)
                {

                    DataColumn d = dc.Find(c => c.ColumnName == pc.Name);
                    if (d != null)
                    {
                        if (item[pc.Name] != DBNull.Value)
                        {
                            pc.SetValue(cn, (item[pc.Name]), null);
                        }
                    }
                }
                lst.Add(cn);
            }
            return lst;
        }       

        public static List<TEntity> Trim<TEntity>(this List<TEntity> collection)
        {
            if (collection!=null && collection.Count()>0)
            {
                Type type = typeof(TEntity);
                IEnumerable<PropertyDescriptor> properties = TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>()
                    .Where(p => p.PropertyType == typeof(string));
                foreach (TEntity entity in collection)
                {
                    foreach (PropertyDescriptor property in properties)
                    {
                        string value = (string)property.GetValue(entity);

                        if (!String.IsNullOrEmpty(value))
                        {
                            value = value.TrimEnd();
                            property.SetValue(entity, value);
                        }
                    }
                }   
            }
            return collection;
        }

        public static List<int> SplitIntoIntegerList<T>(this string value, char separator)
        {
            List<int> lstObject = new List<int>();
            string[] strSplitOperator;
            strSplitOperator = value.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);            
            if (strSplitOperator != null && strSplitOperator.Length > 0)
            {
                foreach (var item in strSplitOperator)
                {
                    lstObject.Add(Convert.ToInt32(item));
                }
            }
            return lstObject;
        }

        public static List<string> SplitIntoStringList(this string value, char separator)
        {
            List<string> lstObject = new List<string>();
            string[] strSplitOperator;
            if (!string.IsNullOrEmpty(value))
            {
                strSplitOperator = value.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                if (strSplitOperator != null && strSplitOperator.Length > 0)
                {
                    foreach (var item in strSplitOperator)
                    {
                        lstObject.Add(item);
                    }
                }   
            }
            return lstObject.Trim();
        }

        public static DataTable ConvertTo<T>(this IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            return table;
        }
    }
}
