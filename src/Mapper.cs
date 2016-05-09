using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperByAttribute
{
    public class MapperAttribute : Attribute
    {
        public Type LinkType { get; protected set; }
        public MapperDirect Direction { get; set; }

        public MapperAttribute(Type type)
        {
            LinkType = type;
            Direction = MapperDirect.From;
        }
    }

    [Flags]
    public enum MapperDirect
    {
        From, To, Both = From | To
    }

    public class MapperTwoDirectionAttribute : MapperAttribute
    {
        public MapperTwoDirectionAttribute(Type type):base(type)
        {
            Direction = MapperDirect.Both;
        }
    }

    public class MapperReverseAttribute : MapperAttribute
    {
        public MapperReverseAttribute(Type type) : base(type)
        {
            Direction = MapperDirect.To;
        }
    }
}
