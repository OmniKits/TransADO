using System;
using System.Data;

namespace TransADO.TypeBuilders
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class TransParamAttribute : TransNameAttribute
    {
        public TransParamAttribute()
            : base(null)
        { }
        public TransParamAttribute(string name)
            : base(name)
        { }

        public DbType? DbType { get; set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public int? Size { get; set; }
    }
}
