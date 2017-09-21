using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sift.Server.Util
{
    internal static class DbUtil
    {
        private static readonly IFormatter serializer = new BinaryFormatter();

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

        public static object Deserialize(byte[] value)
        {
            using (MemoryStream stream = new MemoryStream(value))
            {
                return serializer.Deserialize(stream);
            }
        }

        public static byte[] Serialize(object value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, value);
                return stream.ToArray();
            }
        }
    }
}
