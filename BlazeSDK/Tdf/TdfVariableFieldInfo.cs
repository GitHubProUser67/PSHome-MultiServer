using System.Reflection;

namespace Tdf
{
    internal class TdfVariableFieldInfo : FieldInfo
    {
        private readonly FieldInfo _orgFieldInfo;
        private readonly Type _actualFieldType;
        public TdfVariableFieldInfo(FieldInfo orgFieldInfo, Type actualFieldType)
        {
            _orgFieldInfo = orgFieldInfo;
            _actualFieldType = actualFieldType;
        }

        public override FieldAttributes Attributes => _orgFieldInfo.Attributes;

        public override Type? DeclaringType => _orgFieldInfo.DeclaringType;

        public override RuntimeFieldHandle FieldHandle => _orgFieldInfo.FieldHandle;

        public override Type FieldType => _actualFieldType;

        public override string Name => _orgFieldInfo.Name;

        public override Type? ReflectedType => _orgFieldInfo.ReflectedType;

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _orgFieldInfo.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _orgFieldInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override object? GetValue(object? obj)
        {
            return _orgFieldInfo.GetValue(obj);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _orgFieldInfo.IsDefined(attributeType, inherit);
        }

        public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, System.Globalization.CultureInfo? culture)
        {
            _orgFieldInfo.SetValue(obj, value, invokeAttr, binder, culture);
        }
    }
}
