using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

public static class TransADO
{
    public static DbProviderFactory GetFactory(this ConnectionStringSettings settings)
        => DbProviderFactories.GetFactory(settings.ProviderName);

    public static DbConnection SpawnConnection(this ConnectionStringSettings settings)
    {
        var factory = GetFactory(settings);
        var conn = factory.CreateConnection();
        conn.ConnectionString = settings.ConnectionString;
        return conn;
    }

    public static DbCommand SpawnCommand(this DbConnection conn, string commandText, CommandType commandType = CommandType.Text)
    {
        var cmd = conn.CreateCommand();
        cmd.CommandText = commandText;
        cmd.CommandType = commandType;
        return cmd;
    }

    #region SpawnParameter

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
}
