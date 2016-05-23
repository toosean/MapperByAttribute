using System;

namespace MapperByAttribute
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = true,Inherited = true)]
    public class MapperIgnoreAttribute : Attribute
    {
        public Type Type { get; private set; }

        public MapperIgnoreAttribute()
        {

        }

        public MapperIgnoreAttribute(Type type)
        {
            Type = type;
        }
    }
}
