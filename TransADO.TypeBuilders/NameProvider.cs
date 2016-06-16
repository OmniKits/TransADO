namespace TransADO.TypeBuilders
{
    using System.Reflection;

    public class NameProvider
    {
        public static NameProvider DefaultInstance { get; } = new NameProvider();

        public virtual string TransName(string original, ICustomAttributeProvider attributeProvider) => original;
        public virtual string GetObjectName(MemberInfo member) => TransName(member.Name, member);
        public virtual string GetStoredProcedureName(MethodInfo method) => GetObjectName(method);
        public virtual string GetParameterName(ParameterInfo param) => TransName(param.Name, param);
    }
}
