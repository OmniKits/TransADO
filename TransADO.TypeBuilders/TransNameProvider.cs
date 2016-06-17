using System;
using System.Linq;
using System.Reflection;

namespace TransADO.TypeBuilders
{
    public class TransNameProvider
    {
        public static TransNameProvider DefaultInstance { get; } = new TransNameProvider();

        public virtual string TransName(string original, ICustomAttributeProvider attributeProvider)
        {
            var nameAttr = (TransNameAttribute)attributeProvider.GetCustomAttributes(typeof(TransNameAttribute), true).SingleOrDefault();
            return nameAttr?.Name ?? original;
        }
        public virtual string GetObjectName(MemberInfo member) => TransName(member.Name, member);
        public virtual string GetStoredProcedureName(MethodInfo method) => GetObjectName(method);
        public virtual string GetParameterName(ParameterInfo param) => TransName(param.Name, param);
    }
}
