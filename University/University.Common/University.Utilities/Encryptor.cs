using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace University.Utilities
{
    public class Encryptor
    {
        public static byte[] GenerateHash(string value, string salt)
        {
            byte[] data = Encoding.ASCII.GetBytes(salt + value);
            data = SHA1Managed.Create().ComputeHash(data);
            return data;
        }
        public static byte[] EncryptText(string key, string name)
        {
            byte[] functionReturnValue = null;
            try
            {
                int intPwdCount = 0;
                int intUserAscii = 0;
                string strBuff = "";
                name = name.Trim().ToUpper();
                if (Strings.Len(Strings.Trim(name)) > 0)
                {
                    for (intPwdCount = 1; intPwdCount <= key.Length; intPwdCount++)
                    {
                        intUserAscii = Strings.Asc(Strings.Mid(key, intPwdCount, 1));
                        intUserAscii = intUserAscii + Strings.Asc(Strings.Mid(Strings.Trim(name), (intPwdCount % Strings.Len(Strings.Trim(name))) + 1, 1));
                        strBuff = strBuff + Strings.Chr(intUserAscii & 0xff);
                    }
                    functionReturnValue = System.Text.Encoding.ASCII.GetBytes(Strings.Trim(strBuff));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return functionReturnValue;
        }
        public static string CreateSalt(int size)
        {
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
        public static string GetMd5Hash(string input, string salt)
        {
            var hasher = MD5.Create();
            var data = hasher.ComputeHash(Encoding.Default.GetBytes(input + salt));
            var builder = new StringBuilder();

            for (var i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }
        public static string DeSerialize(byte[] byteArray)
        {
            //MemoryStream stream = new MemoryStream(byteArray);
            //stream.Seek(0, 0);
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //return (string)binaryFormatter.Deserialize(stream);
            return DeSerializeEntities(byteArray);
        }
        public static bool CompareByteArray(byte[] source, byte[] target)
        {
            if (source == null || target == null || source.LongLength != target.LongLength)
            {
                return false;
            }
            for (long i = 0; i < source.LongLength; i++)
            {
                if (source[i] != target[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static byte[] ToByteArray(object source)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, source); /// data is the class i wanna serialize.
            ms.Seek(0, 0);
            StreamReader rdr = new StreamReader(ms);
            string str = rdr.ReadToEnd();
            byte[] byteArray = Encoding.ASCII.GetBytes(str);
            return byteArray;
        }
        public static string DeSerializeEntities(byte[] byteArray)
        {
            try
            {
                //using (MemoryStream stream = new MemoryStream(byteArray))
                //{
                //    stream.Seek(0, 0);
                //    BinaryFormatter binaryFormatter = new BinaryFormatter();
                //    return (string)binaryFormatter.Deserialize(stream);
                //}
                string strEntities = string.Empty;
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    stream.Position = 0;
                    var formatter = new BinaryFormatter();
                    //stream.Capacity = 200;
                    
                    //for (int i = 0; i < byteArray.Length; i++)
                    //{
                    //    strTemp+=Convert.ToBase64String(byteArray);
                    //}
                    //strTemp += Convert.ToBase64String(byteArray);
                    //strTemp = string.Empty;
                    //foreach (byte value in byteArray)
                    //{
                    //    strTemp += value + ";";
                    //}
                    //foreach (byte item in byteArray)
                    //{
                    //    strTemp =(string)formatter.Deserialize(item);
                    //}
                    strEntities = string.Empty;
                    strEntities = Encoding.ASCII.GetString(byteArray);
                    strEntities = strEntities.Substring(strEntities.IndexOf("[IsApplicable"), strEntities.IndexOf("]") + 1 - strEntities.IndexOf("[IsApplicable"));
                    return strEntities;
                }
            }
            catch (Exception)
            {                
                throw;
            }            
        }
        
        public static string SerializeToXMLString(object ObjectToSerialize)
        {
            StringWriter stringWriter = new StringWriter();
            try
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                XmlSerializer serializer = new XmlSerializer(ObjectToSerialize.GetType());
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.OmitXmlDeclaration = true;                
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, writerSettings))
                {
                    serializer.Serialize(xmlWriter, ObjectToSerialize, ns);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
            return stringWriter.ToString();
        }

        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            return (propertyExpression.Body as MemberExpression).Member.Name;
        }
    }
}
