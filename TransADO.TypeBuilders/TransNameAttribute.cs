using System;

namespace TransADO.TypeBuilders
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class TransNameAttribute : Attribute
    {
        public string Name { get; }
        public TransNameAttribute(string name)
        {
            Name = name;
        }
    }
}
