using System.Reflection;

namespace TransADO.Npgsql
{
    using System;
    using global::Npgsql;
    using TypeBuilders;

    public class NpgsqlNameProvider : TransNameProvider, INpgsqlNameTranslator
    {
        public static new NpgsqlNameProvider DefaultInstance { get; } = new NpgsqlNameProvider();

        public override string GetObjectName(MemberInfo member)=> "\"" + TransName(member.Name, member) + "\"";
        public override string GetStoredProcedureName(MethodInfo method) => GetObjectName(method);
        public override string GetParameterName(ParameterInfo param) => TransName(param.Name, param);

        string INpgsqlNameTranslator.TranslateTypeName(string clrName) => clrName;
        string INpgsqlNameTranslator.TranslateMemberName(string clrName) => clrName;
    }
}
