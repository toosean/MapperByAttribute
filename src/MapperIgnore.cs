using System;

namespace MapperByAttribute
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
    public class MapperIgnoreAttribute : Attribute
    {
    }
}
