using System.Collections.Generic;
using System.Data;

namespace Sift.Server.Util
{
    internal static class DbUtil
    {
        public static IDbCommand CreateCommand(IDbConnection conn, string query, IDictionary<string, object> dataParams = null)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = query;
            if (dataParams != null)
            {
                foreach (KeyValuePair<string, object> p in dataParams)
                {
                    IDataParameter dp = cmd.CreateParameter();
                    dp.ParameterName = p.Key;
                    dp.Value = p.Value;
                    cmd.Parameters.Add(dp);
                }
            }
            return cmd;
        }
    }
}
