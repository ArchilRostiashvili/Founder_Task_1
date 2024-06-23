using UnityEngine;

namespace BebiLibs
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class ObjectInspectorAttribute : PropertyAttribute
    {
        public bool IsReadOnly { get; private set; }
        public ObjectInspectorAttribute(bool isReadOnly = true)
        {
            this.IsReadOnly = isReadOnly;
        }
    }
}
