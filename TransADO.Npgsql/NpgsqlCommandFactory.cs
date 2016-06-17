using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TransADO.Npgsql
{
    using TypeBuilders;

    public class NpgsqlCommandFactory : TransCommandFactory
    {
        public static new NpgsqlNameProvider DefaultNameProvider { get; } = new NpgsqlNameProvider();

        public NpgsqlCommandFactory(TransNameProvider nameProvider = null)
            : base(nameProvider ?? DefaultNameProvider)
        { }

        public static new NpgsqlCommandFactory Default { get; } = new NpgsqlCommandFactory();

        public override CommandType GetCommandType(MethodInfo method) => CommandType.Text;
        static void AppendParameter(StringBuilder sb, string pn)
        {
            sb.Append('"');
            sb.Append(pn);
            sb.Append('"');
            sb.Append(' ');
            sb.Append('=');
            sb.Append('>');
            sb.Append(' ');
            sb.Append(':');
            sb.Append(pn);
        }
        public override string GetCommandText(MethodInfo method)
        {
            const string SELECT = "SELECT ";

            var name = NameProvider.GetStoredProcedureName(method);
            var @params = method.GetParameters();
            var len = SELECT.Length;
            len += @params.Length == 0 ? 2 /* () */ : @params.Length * 9; /* , "" => : */
            len += name.Length;
            foreach (var p in @params)
                len += p.Name.Length * 2;
            var sb = new StringBuilder(SELECT, len);
            sb.Append(name);
            sb.Append('(');
            if (@params.Length == 0)
            {
                sb.Append(')');
                return sb.ToString();
            }

            var pn = NameProvider.GetParameterName(@params[0]);
            AppendParameter(sb, pn);

            for (var i = 1; i < @params.Length; i++)
            {
                sb.Append(',');
                sb.Append(' ');
                pn = NameProvider.GetParameterName(@params[i]);
                AppendParameter(sb, pn);
            }

            sb.Append(')');

            if (sb.Length != len)
                Trace.WriteLine($"sb: {len} !! {sb.Length} / {sb.Capacity}");

            return sb.ToString();
        }
        public override string GetParameterName(ParameterInfo param) => NameProvider.GetParameterName(param);
    }
}
