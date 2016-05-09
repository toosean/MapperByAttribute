using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperByAttribute
{
    public interface ICustomMapperFrom<TSource>
    {
        void MapperFrom(TSource source);
    }
}
