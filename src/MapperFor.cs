using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperByAttribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MapperForAttribute : Attribute
    {

        public Type Type { get; private set; }
        public string ForName { get; private set; }

        public MapperForAttribute(Type type, string name)
        {
            Type = type;
            ForName = name;
        }

        public MapperForAttribute(string name)
        {
            ForName = name;
        }
    }
}
