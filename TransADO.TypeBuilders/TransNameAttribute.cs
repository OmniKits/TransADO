using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransADO.TypeBuilders
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Module | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class TransNameAttribute : Attribute
    {
        public string Name { get; }
        public TransNameAttribute(string name)
        {
            Name = name;
        }
    }
}
