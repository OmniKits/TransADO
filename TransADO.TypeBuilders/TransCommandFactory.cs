using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace TransADO.TypeBuilders
{
    using global::TypeBuilders;

    static class Helpers
    {
        static readonly Type TypeNullable = typeof(Nullable<>);
        public static bool IsNullable(this Type type)
         => type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == TypeNullable;
    }

    public class TransCommandFactory : ConstrainedType<IDbConnection>
    {
        static readonly Type TypeObject = typeof(object);

        static readonly MethodInfo MethodCreateCommand = ThisType.GetMethod(nameof(IDbConnection.CreateCommand));

        static readonly Type TypeCommand = typeof(IDbCommand);
        static readonly MethodInfo MethodSetCommandType = TypeCommand.GetProperty(nameof(IDbCommand.CommandType)).GetSetMethod();
        static readonly MethodInfo MethodSetCommandText = TypeCommand.GetProperty(nameof(IDbCommand.CommandText)).GetSetMethod();
        static readonly MethodInfo MethodGetParameters = TypeCommand.GetProperty(nameof(IDbCommand.Parameters)).GetGetMethod();
        static readonly MethodInfo MethodCreateParameter = TypeCommand.GetMethod(nameof(IDbCommand.CreateParameter));

        static readonly Type TypeDataParameter = typeof(IDataParameter);
        static readonly MethodInfo MethodSetParameterName = TypeDataParameter.GetProperty(nameof(IDataParameter.ParameterName)).GetSetMethod();
        static readonly MethodInfo MethodSetValue = TypeDataParameter.GetProperty(nameof(IDataParameter.Value)).GetSetMethod();
        static readonly Type TypeParamAttribute = typeof(TransParamAttribute);
        static readonly MethodInfo MethodSetDbType = TypeDataParameter.GetProperty(nameof(IDataParameter.DbType)).GetSetMethod();
        static readonly Type TypeDbDataParameter = typeof(IDbDataParameter);
        static readonly MethodInfo MethodSetPrecision = TypeDbDataParameter.GetProperty(nameof(IDbDataParameter.Precision)).GetSetMethod();
        static readonly MethodInfo MethodSetScale = TypeDbDataParameter.GetProperty(nameof(IDbDataParameter.Scale)).GetSetMethod();
        static readonly MethodInfo MethodSetSize = TypeDbDataParameter.GetProperty(nameof(IDbDataParameter.Size)).GetSetMethod();

        static readonly MethodInfo MethodAddParameter = typeof(IList).GetMethod(nameof(IList.Add));

        static readonly FieldInfo FieldDBNull = typeof(DBNull).GetField(nameof(DBNull.Value));

        public static TransNameProvider DefaultNameProvider { get; } = new TransNameProvider();

        public TransNameProvider NameProvider { get; }
        public TransCommandFactory(TransNameProvider nameProvider = null)
        {
            NameProvider = nameProvider ?? DefaultNameProvider;
        }

        public static TransCommandFactory Default { get; } = new TransCommandFactory();

        protected virtual string GetCommandCore(MethodInfo method, ref CommandType type) => "\"" + NameProvider.GetStoredProcedureName(method) + "\"";
        public virtual string GetCommandText(MethodInfo method, ref CommandType type) => GetCommandCore(method, ref type);

        public virtual string GetParameterName(ParameterInfo param) => "@" + NameProvider.GetParameterName(param);

        public override void ImplementMethod(MethodInfo declaration, MethodBuilder implement, FieldInfo input)
        {
            var returnType = declaration.ReturnType;
            if (!TypeCommand.IsAssignableFrom(returnType))
                throw new InvalidOperationException();

            var @params = declaration.GetParameters();
            var ilGen = implement.GetILGenerator();
            var cmd = ilGen.DeclareLocal(TypeCommand);

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, input);

            ilGen.Emit(OpCodes.Callvirt, MethodCreateCommand);
            ilGen.Emit(OpCodes.Stloc, cmd);

            var type = CommandType.StoredProcedure;
            var text = GetCommandText(declaration, ref type);

            ilGen.Emit(OpCodes.Ldloc, cmd);
            ilGen.Emit(OpCodes.Ldc_I4, (int)type);
            ilGen.Emit(OpCodes.Callvirt, MethodSetCommandType);

            ilGen.Emit(OpCodes.Ldloc, cmd);
            ilGen.Emit(OpCodes.Ldstr, text);
            ilGen.Emit(OpCodes.Callvirt, MethodSetCommandText);

            ilGen.Emit(OpCodes.Ldloc, cmd);
            ilGen.Emit(OpCodes.Callvirt, MethodGetParameters);
            for (var i = 0; i < @params.Length;)
            {
                var param = @params[i++];

                ilGen.Emit(OpCodes.Dup);

                ilGen.Emit(OpCodes.Ldloc, cmd);
                ilGen.Emit(OpCodes.Callvirt, MethodCreateParameter);

                ilGen.Emit(OpCodes.Dup);
                ilGen.Emit(OpCodes.Ldstr, GetParameterName(param));
                ilGen.Emit(OpCodes.Callvirt, MethodSetParameterName);

                var paramAttr = (TransParamAttribute)param.GetCustomAttributes(TypeParamAttribute, true).SingleOrDefault();
                if (paramAttr != null)
                {
                    var dbType = paramAttr.DbTypeNullable;
                    if (dbType.HasValue)
                    {
                        ilGen.Emit(OpCodes.Dup);
                        ilGen.Emit(OpCodes.Ldc_I4, (int)dbType.Value);
                        ilGen.Emit(OpCodes.Callvirt, MethodSetDbType);
                    }

                    var precision = paramAttr.PrecisionNullable;
                    if (precision.HasValue)
                    {
                        ilGen.Emit(OpCodes.Dup);
                        ilGen.Emit(OpCodes.Ldc_I4, precision.Value);
                        ilGen.Emit(OpCodes.Callvirt, MethodSetPrecision);
                    }

                    var scale = paramAttr.ScaleNullable;
                    if (scale.HasValue)
                    {
                        ilGen.Emit(OpCodes.Dup);
                        ilGen.Emit(OpCodes.Ldc_I4, scale.Value);
                        ilGen.Emit(OpCodes.Callvirt, MethodSetScale);
                    }

                    var size = paramAttr.SizeNullable;
                    if (size.HasValue)
                    {
                        ilGen.Emit(OpCodes.Dup);
                        ilGen.Emit(OpCodes.Ldc_I4, size.Value);
                        ilGen.Emit(OpCodes.Callvirt, MethodSetSize);
                    }
                }

                ilGen.Emit(OpCodes.Dup);
                ilGen.Emit(OpCodes.Ldarg_S, (byte)i);
                var paramType = param.ParameterType;
                if (paramType != TypeObject)
                {
                    var nullable = !paramType.IsValueType;
                    if (!nullable)
                    {
                        ilGen.Emit(OpCodes.Box, paramType);
                        nullable = paramType.IsNullable();
                    }
                    if (nullable)
                    {
                        var label = ilGen.DefineLabel();
                        ilGen.Emit(OpCodes.Dup);
                        ilGen.Emit(OpCodes.Ldnull);
                        ilGen.Emit(OpCodes.Ceq);

                        ilGen.Emit(OpCodes.Brfalse_S, label);
                        ilGen.Emit(OpCodes.Pop);
                        ilGen.Emit(OpCodes.Ldsfld, FieldDBNull);

                        ilGen.MarkLabel(label);
                    }
                }
                ilGen.Emit(OpCodes.Callvirt, MethodSetValue);

                ilGen.Emit(OpCodes.Callvirt, MethodAddParameter);
                ilGen.Emit(OpCodes.Pop); // pop result int
            }
            ilGen.Emit(OpCodes.Pop); // pop ParameterCollection

            ilGen.Emit(OpCodes.Ldloc, cmd);
            ilGen.Emit(OpCodes.Castclass, returnType);
            ilGen.Emit(OpCodes.Ret);
        }
    }
}
