using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperByAttribute
{
    public interface ICustomMapperTo<TDestination>
    {
        void MapperTo(TDestination destination);
    }
}
