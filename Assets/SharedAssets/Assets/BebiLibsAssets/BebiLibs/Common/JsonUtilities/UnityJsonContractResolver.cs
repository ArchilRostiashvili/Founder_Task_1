using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Linq;

namespace BebiLibs
{
    public class UnityJsonContractResolver : DefaultContractResolver
    {
        public UnityJsonContractResolver() { }

        protected override List<MemberInfo> GetSerializableMembers(System.Type objectType)
        {
            List<MemberInfo> members = base.GetSerializableMembers(objectType);
            members.AddRange(GetMissingMembers(objectType, members));
            return members;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

            if (member.GetCustomAttribute<SerializeField>() != null)
            {
                jsonProperty.Ignored = false;
                jsonProperty.Writable = CanWriteMemberWithSerializeField(member);
                jsonProperty.Readable = CanReadMemberWithSerializeField(member);
                jsonProperty.HasMemberAttribute = true;
            }

            if (member.MemberType == MemberTypes.Property)
            {
                jsonProperty.Ignored = true;
            }

            return jsonProperty;
        }

        protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> lists = base.CreateProperties(type, memberSerialization);
            return lists;
        }

        protected override JsonObjectContract CreateObjectContract(System.Type objectType)
        {
            JsonObjectContract jsonObjectContract = base.CreateObjectContract(objectType);
#if false
            if (typeof(ScriptableObject).IsAssignableFrom(objectType))
            {
                jsonObjectContract.DefaultCreator = () =>
                {
                    return ScriptableObject.CreateInstance(objectType);
                };
            }
#endif
            return jsonObjectContract;
        }

        private static IEnumerable<MemberInfo> GetMissingMembers(System.Type type, List<MemberInfo> alreadyAdded)
        {
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Cast<MemberInfo>()
                .Concat(type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
                .Where(o => o.GetCustomAttribute<SerializeField>() != null
                    && !alreadyAdded.Contains(o));
        }

        private static bool CanReadMemberWithSerializeField(MemberInfo member)
        {
            if (member is PropertyInfo property)
            {
                return property.CanRead;
            }
            else
            {
                return true;
            }
        }

        private static bool CanWriteMemberWithSerializeField(MemberInfo member)
        {
            if (member is PropertyInfo property)
            {
                return property.CanWrite;
            }
            else
            {
                return true;
            }
        }
    }

    public class FixedUnityTypeContractResolver : UnityJsonContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

            if (!jsonProperty.Ignored && member.GetCustomAttribute<Newtonsoft.Json.JsonIgnoreAttribute>() != null)
                jsonProperty.Ignored = true;

            return jsonProperty;
        }
    }

}
