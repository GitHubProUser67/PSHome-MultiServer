using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Tdf
{
    public class TdfFactory
    {
        private readonly ConcurrentDictionary<uint, Type> _tdfVariableTypeMap;
        private readonly ConcurrentDictionary<Type, Dictionary<string, FieldInfo>> _tdfTypeMap;

        public TdfFactory()
        {
            _tdfVariableTypeMap = new ConcurrentDictionary<uint, Type>();
            _tdfTypeMap = new ConcurrentDictionary<Type, Dictionary<string, FieldInfo>>();
        }

        public bool RegisterTdfType(Type tdfType)
        {
            var tdfStruct = tdfType.GetCustomAttribute<TdfStruct>();

            if (tdfStruct != null)
                _tdfVariableTypeMap.TryAdd(tdfStruct.TdfId, tdfType);
            else if (tdfType.BaseType != typeof(TdfUnion))
                return false;

            return _tdfTypeMap.TryAdd(tdfType, getTypeFieldContext(tdfType));
        }

        [RequiresUnreferencedCode("Uses reflection that may break when trimming.")]
        public int RegisterNamespace(Assembly assembly, string nameSpace)
        {
            var count = 0;
            foreach (var type in assembly.GetTypes())
            {
                if (type.Namespace == nameSpace && RegisterTdfType(type))
                    count++;
            }
            return count;
        }

        static Dictionary<string, FieldInfo> getTypeFieldContext(Type type)
        {
            Dictionary<string, FieldInfo> map = [];

            var fields = type.GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );
            foreach (var field in fields)
            {
                var tag = field.GetCustomAttribute<TdfMember>();
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
            return _tdfTypeMap.TryGetValue(type, out var context) ? context
                : RegisterTdfType(type) ? _tdfTypeMap[type]
                : [];
        }

        internal Dictionary<string, FieldInfo> GetContext(uint tdfId)
        {
            return _tdfVariableTypeMap.TryGetValue(tdfId, out var type) ? GetContext(type) : [];
        }

        internal Type GetType(uint tdfId)
        {
            return _tdfVariableTypeMap.TryGetValue(tdfId, out var type) ? type : typeof(object);
        }

        internal static uint GetTdfId(Type type)
        {
            var tdfStruct = type.GetCustomAttribute<TdfStruct>();
            return tdfStruct != null ? tdfStruct.TdfId : 0;
        }
    }
}
