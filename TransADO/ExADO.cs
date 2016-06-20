using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace TransADO
{
    public static class ExADO
    {
        public static DbProviderFactory GetFactory(this ConnectionStringSettings settings)
         => DbProviderFactories.GetFactory(settings.ProviderName);

        public static void OpenConnectionAndExecuteNonQuery(this IDbCommand cmd)
        {
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
        }
        public static IDataReader OpenConnectionAndExecuteReader(this IDbCommand cmd)
        {
            cmd.Connection.Open();
            return cmd.ExecuteReader();
        }
        public static object OpenConnectionAndExecuteScalar(this IDbCommand cmd)
        {
            cmd.Connection.Open();
            return cmd.ExecuteScalar();
        }

        public static DbDataReader OpenConnectionAndExecuteReader(this DbCommand cmd)
        {
            cmd.Connection.Open();
            return cmd.ExecuteReader();
        }
#if !NO_AWAIT
        public static async Task OpenConnectionAndExecuteNonQueryAsync(this DbCommand cmd, CancellationToken cancellationToken = default(CancellationToken))
        {
            await cmd.Connection.OpenAsync(cancellationToken);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
        public static async Task<DbDataReader> OpenConnectionAndExecuteReaderAsync(this DbCommand cmd, CancellationToken cancellationToken = default(CancellationToken))
        {
            await cmd.Connection.OpenAsync(cancellationToken);
            return await cmd.ExecuteReaderAsync(cancellationToken);
        }
        public static async Task<object> OpenConnectionAndExecuteScalarAsync(this DbCommand cmd, CancellationToken cancellationToken = default(CancellationToken))
        {
            await cmd.Connection.OpenAsync(cancellationToken);
            return await cmd.ExecuteScalarAsync(cancellationToken);
        }
#endif
        public static DbConnection SpawnConnection(this ConnectionStringSettings settings)
        {
            var factory = GetFactory(settings);
            var conn = factory.CreateConnection();
            conn.ConnectionString = settings.ConnectionString;
            return conn;
        }

        public static IDbCommand SpawnCommand(this IDbConnection conn, string commandText, CommandType commandType = CommandType.Text)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            return cmd;
        }
        public static DbCommand SpawnCommand(this DbConnection conn, string commandText, CommandType commandType = CommandType.Text)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            return cmd;
        }

        #region spawn DbParameter

        public static DbParameter SpawnParameter(this DbCommand cmd, object value)
        {
            var p = cmd.CreateParameter();
            p.Value = value;
            return p;
        }

        public static DbParameter SpawnParameter(this DbCommand cmd, string name, object value = null)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            return p;
        }

        public static DbParameter SpawnParameter(this DbCommand cmd, DbType dbType, object value = null)
        {
            var p = cmd.CreateParameter();
            p.DbType = dbType;
            p.Value = value;
            return p;
        }

        public static DbParameter SpawnParameter(this DbCommand cmd, DbType dbType, int size, object value = null)
        {
            var p = cmd.CreateParameter();
            p.DbType = dbType;
            p.Size = size;
            p.Value = value;
            return p;
        }

        public static DbParameter SpawnParameter(this DbCommand cmd, string name, DbType dbType, object value = null)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = dbType;
            p.Value = value;
            return p;
        }

        public static DbParameter SpawnParameter(this DbCommand cmd, string name, DbType dbType, int size, object value = null)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = dbType;
            p.Size = size;
            p.Value = value;
            return p;
        }

        public static DbParameter SpawnParameter(this DbCommand cmd, string name,
            ParameterDirection direction, DbType? dbType = null, int? size = null, object value = null)
        {
            var p = cmd.CreateParameter();

            if (name != null)
                p.ParameterName = name;

            if (dbType.HasValue)
                p.DbType = dbType.Value;

            if (size.HasValue)
                p.Size = size.Value;

            p.Direction = direction;
            p.Value = value;
            return p;
        }

        #endregion

        #region spawn IDbDataParameter

        public static IDbDataParameter SpawnParameter(this IDbCommand cmd, object value)
        {
            var p = cmd.CreateParameter();
            p.Value = value;
            return p;
        }

        public static IDbDataParameter SpawnParameter(this IDbCommand cmd, string name, object value = null)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            return p;
        }

        public static IDbDataParameter SpawnParameter(this IDbCommand cmd, DbType dbType, object value = null)
        {
            var p = cmd.CreateParameter();
            p.DbType = dbType;
            p.Value = value;
            return p;
        }

        public static IDbDataParameter SpawnParameter(this IDbCommand cmd, DbType dbType, int size, object value = null)
        {
            var p = cmd.CreateParameter();
            p.DbType = dbType;
            p.Size = size;
            p.Value = value;
            return p;
        }

        public static IDbDataParameter SpawnParameter(this IDbCommand cmd, string name, DbType dbType, object value = null)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = dbType;
            p.Value = value;
            return p;
        }

        public static IDbDataParameter SpawnParameter(this IDbCommand cmd, string name, DbType dbType, int size, object value = null)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = dbType;
            p.Size = size;
            p.Value = value;
            return p;
        }

        public static IDbDataParameter SpawnParameter(this IDbCommand cmd, string name,
            ParameterDirection direction, DbType? dbType = null, int? size = null, object value = null)
        {
            var p = cmd.CreateParameter();

            if (name != null)
                p.ParameterName = name;

            if (dbType.HasValue)
                p.DbType = dbType.Value;

            if (size.HasValue)
                p.Size = size.Value;

            p.Direction = direction;
            p.Value = value;
            return p;
        }

        #endregion
    }
}
