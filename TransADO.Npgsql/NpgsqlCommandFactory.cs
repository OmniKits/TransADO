using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransADO.Npgsql
{
    using System.Data;
    using System.Reflection;
    using TypeBuilders;

    public class NpgsqlCommandFactory : CommandFactory
    {
        public static new NpgsqlNameProvider DefaultTranslator { get; } = new NpgsqlNameProvider();

        public NpgsqlCommandFactory(NameProvider translator = null)
            : base(translator ?? DefaultTranslator)
        { }

        public static new NpgsqlCommandFactory Default { get; } = new NpgsqlCommandFactory();

        public override CommandType GetCommandType(MethodInfo method) => CommandType.Text;
        public override string GetCommandText(MethodInfo method)
        {
            var name = Translator.GetStoredProcedureName(method);
            var @params = method.GetParameters();
            var len = 7; // SELECT ()
            len += @params.Length == 0 ? 2 : @params.Length * 9; // ,  => :
            len += name.Length;
            foreach (var p in @params)
                len += p.Name.Length * 2;
            var sb = new StringBuilder("SELECT ", len);
            sb.Append(name);
            sb.Append('(');
            if (@params.Length > 0)
            {
                var pn = Translator.GetParameterName(@params[0]);
                sb.Append('"');
                sb.Append(pn);
                sb.Append('"');
                sb.Append(' ');
                sb.Append('=');
                sb.Append('>');
                sb.Append(' ');
                sb.Append(':');
                sb.Append(pn);
                for (var i = 1; i < @params.Length; i++)
                {
                    sb.Append(',');
                    sb.Append(' ');
                    pn = Translator.GetParameterName(@params[i]);
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
            }
            sb.Append(')');
            return sb.ToString();
        }
        public override string GetParameterName(ParameterInfo param) => Translator.GetParameterName(param);
    }
}
