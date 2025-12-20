using System.Collections.Concurrent;
using System.Reflection;

namespace Tdf
{
    public class TdfFactory
    {
        private ConcurrentDictionary<uint, Type> _tdfVariableTypeMap;
        private ConcurrentDictionary<Type, Dictionary<string, FieldInfo>> _tdfTypeMap;

        public TdfFactory()
        {
            _tdfVariableTypeMap = new ConcurrentDictionary<uint, Type>();
            _tdfTypeMap = new ConcurrentDictionary<Type, Dictionary<string, FieldInfo>>();
        }

        public bool RegisterTdfType(Type tdfType)
        {
            TdfStruct? tdfStruct = tdfType.GetCustomAttribute<TdfStruct>();

            if (tdfStruct != null)
                _tdfVariableTypeMap.TryAdd(tdfStruct.TdfId, tdfType);
            else if (tdfType.BaseType != typeof(TdfUnion))
                return false;

            if (!_tdfTypeMap.TryAdd(tdfType, getTypeFieldContext(tdfType)))
                return false;

            return true;
        }

        public int RegisterNamespace(Assembly assembly, string nameSpace)
        {
            int count = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.Namespace == nameSpace && RegisterTdfType(type))
                    count++;
            }
            return count;
        }


        Dictionary<string, FieldInfo> getTypeFieldContext(Type type)
        {
            Dictionary<string, FieldInfo> map = new Dictionary<string, FieldInfo>();

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                TdfMember? tag = field.GetCustomAttribute<TdfMember>();
                if (tag == null)
                    continue;

                map.Add(tag, field);
            }

            return map;
        }

        public TdfLegacyDecoder CreateLegacyDecoder()
        {
            return new TdfLegacyDecoder(this);
        }

        public TdfLegacyEncoder CreateLegacyEncoder()
        {
            return new TdfLegacyEncoder(this);
        }

        public TdfDecoder CreateDecoder(bool heat1Bug)
        {
            return new TdfDecoder(this, heat1Bug);
        }

        public TdfEncoder CreateEncoder(bool heat1Bug)
        {
            return new TdfEncoder(this, heat1Bug);
        }

        internal Dictionary<string, FieldInfo> GetContext(Type type)
        {
            if (_tdfTypeMap.TryGetValue(type, out Dictionary<string, FieldInfo>? context))
                return context;

            if (RegisterTdfType(type))
                return _tdfTypeMap[type];

            return new Dictionary<string, FieldInfo>();
        }

        internal Dictionary<string, FieldInfo> GetContext(uint tdfId)
        {
            if (_tdfVariableTypeMap.TryGetValue(tdfId, out Type? type))
                return GetContext(type);

            return new Dictionary<string, FieldInfo>();
        }

        internal Type GetType(uint tdfId)
        {
            if (_tdfVariableTypeMap.TryGetValue(tdfId, out Type? type))
                return type;
            return typeof(object);
        }

        internal uint GetTdfId(Type type)
        {
            TdfStruct? tdfStruct = type.GetCustomAttribute<TdfStruct>();
            if (tdfStruct != null)
                return tdfStruct.TdfId;
            return 0;
        }
    }
}