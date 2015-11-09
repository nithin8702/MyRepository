using System.Data;
using System.Data.SqlClient;

namespace Inatech.Marine.DataContext
{
    public class StoredProcedureHelper
    {
        //public static SqlParameter[] GetSQLParamsFromObject(object obj)
        //{
        //    if (obj == null) return new SqlParameter[] { };
        //    Type objType = ObjectContext.GetObjectType(obj.GetType());
        //    IDictionary<string, object> keyValue = objType.GetProperties().Where(p => p.GetCustomAttributes(typeof(ExcludeSQLParamAttribute), false).Length == 0)
        //                .ToDictionary(p => "@" + p.Name, p => p.GetValue(obj, null) ?? DBNull.Value);

        //    SqlParameter[] sqlParameter = new SqlParameter[keyValue.Count];

        //    int loop = 0;
        //    keyValue.ToList().ForEach(kv =>
        //    {
        //        sqlParameter[loop] = new SqlParameter(kv.Key, kv.Value);
        //        loop++;
        //    });

        //    return sqlParameter;
        //}

        //public static string GetConnectionString(CurrentUser currentUser)
        //{
        //    TenantDBContext db = ContextProvider.GetTenantContext(currentUser.Tenant);
        //    ConfigurationDBContext dbConfig = ContextProvider.GetConfigContext();
        //    Tenant tenant = dbConfig.Tenants.Single(ten => ten.TenantId == currentUser.TenantId);
        //    string connectionString = tenant.ConnectionString;
        //    return connectionString;
        //}

        public static void Execute(string connectionString, ref SqlParameter[] sqlParameter, string storProcedureName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = storProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var parameter in sqlParameter)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    var isExecuted = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void Execute(string connectionString, string storProcedureName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = storProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;                 

                    var isExecuted = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static DataSet GetDataSet(string connectionString, ref SqlParameter[] sqlParameter, string storProcedureName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = storProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var parameter in sqlParameter)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                   
                    cmd.ExecuteNonQuery();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        // Fill the DataSet using default values for DataTable names, etc
                        DataSet dataset = new DataSet();
                        da.Fill(dataset);
                        conn.Close();
                        return dataset;
                    }                   
                    
                }
            }
        }

        public static string GetExecuteScalar(string connectionString, ref SqlParameter[] sqlParameter, string storProcedureName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = storProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var parameter in sqlParameter)
                    {
                        cmd.Parameters.Add(parameter);
                    }
             object Responsobj=cmd.ExecuteScalar();
             string ReturnString = Responsobj.ToString();
             return ReturnString;
                    }
            }
        }

        public static int Execute(string connectionString, ref SqlParameter[] sqlParameter, string storProcedureName, int temp = 0)
        {
            int isExecuted;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = storProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var parameter in sqlParameter)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    isExecuted = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return isExecuted;
        }

    }
}
