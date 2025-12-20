using System.Reflection;

namespace Tdf
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public class TdfUnion : TdfStruct
    {
        internal static readonly byte[] TDF_VALU_TAG = new byte[] { 0xDA, 0x1B, 0x35, (byte)TdfBaseType.TDF_TYPE_STRUCT };
        internal static readonly byte[] TDF_LEGACY_VALU_TAG = new byte[] { 0xDA, 0x1B, 0x35 };

        public byte ActiveMember { get; private set; }
        public TdfUnion(byte activeMember)
        {
            ActiveMember = activeMember;
        }

        public TdfUnion()  //tag not needed, this constructor is used on the struct
        {
            ActiveMember = 0x7F;
        }

        public void SetValue(object? value)
        {
            if (value == null)
            {
                ActiveMember = 0x7F;
                return;
            }

            Type valueType = value.GetType();

            foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                TdfUnion? union = field.GetCustomAttribute<TdfUnion>();
                if (union == null)
                    continue;

                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>) && valueType == field.FieldType.GetGenericArguments()[0]) //everything must be nullable
                {
                    ActiveMember = union.ActiveMember;
                    field.SetValue(this, value);
                }
                else
                {
                    try { field.SetValue(this, null); } catch { } //set null for other members
                }
            }
        }

        public Type? GetActiveMemberType(byte activeMember)
        {
            foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                TdfUnion? union = field.GetCustomAttribute<TdfUnion>();
                if (union == null)
                    continue;

                if (activeMember == union.ActiveMember && field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return field.FieldType.GetGenericArguments()[0];
            }

            return null;
        }

        public string? GetActiveMemberName(byte activeMember)
        {
            foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                TdfUnion? union = field.GetCustomAttribute<TdfUnion>();
                if (union == null)
                    continue;

                if (activeMember == union.ActiveMember && field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return field.Name;
            }

            return null;
        }

        public object? GetValue()
        {
            if (ActiveMember == 0x7F)
                return null;

            foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                TdfUnion? union = field.GetCustomAttribute<TdfUnion>();
                if (union == null)
                    continue;

                if (union.ActiveMember == ActiveMember)
                    return field.GetValue(this);
            }

            return null;
        }

        public string? GetValueName()
        {
            if (ActiveMember == 0x7F)
                return null;

            foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                TdfUnion? union = field.GetCustomAttribute<TdfUnion>();
                if (union == null)
                    continue;

                if (union.ActiveMember == ActiveMember)
                    return field.Name;
            }

            return null;
        }

    }
}
