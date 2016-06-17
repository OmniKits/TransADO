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

        public DbType? DbTypeNullable { get; private set; }
        public DbType DbType
        {
            get { return DbTypeNullable.Value; }
            set { DbTypeNullable = value; }
        }
        public byte? PrecisionNullable { get; private set; }
        public byte Precision
        {
            get { return PrecisionNullable.Value; }
            set { PrecisionNullable = value; }
        }
        public byte? ScaleNullable { get; private set; }
        public byte Scale
        {
            get { return ScaleNullable.Value; }
            set { ScaleNullable = value; }
        }
        public int? SizeNullable { get; private set; }
        public int Size
        {
            get { return SizeNullable.Value; }
            set { SizeNullable = value; }
        }
    }
}
