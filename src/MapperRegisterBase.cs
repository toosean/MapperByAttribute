using AutoMapper;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperByAttribute
{
    public abstract class MapperRegisterBase
    {
        private static Type _typeCustomMapperFrom = typeof(ICustomMapperFrom<>);
        private static Type _typeICustomMapperTo = typeof(ICustomMapperTo<>);

        public abstract void AssertConfigurationIsValid();

        public virtual void Register<T>()
        {
            Register(typeof(T));
        }
        public abstract void Register(Type type);

        public virtual void Register(Assembly assembly)
        {
            var types = assembly.ExportedTypes.Where(w => w.IsDefined(typeof(MapperAttribute)));
            foreach (var type in types) Register(type);
        }

        protected virtual bool HasCustomFrom(Type source, Type destination)
        {
            var interfaceType = _typeCustomMapperFrom.MakeGenericType(destination);
            return (source.GetInterfaces().Any(a => a == interfaceType));
        }
        protected virtual bool HasCustomTo(Type source, Type destination)
        {
            var interfaceType = _typeICustomMapperTo.MakeGenericType(destination);
            return (source.GetInterfaces().Any(a => a == interfaceType));
        }

        protected virtual void InvokeCustomFrom(object source, object destination)
        {
            var interfaceType = _typeCustomMapperFrom.MakeGenericType(destination.GetType());
            if (source.GetType().GetInterfaces().Any(a => a == interfaceType))
            {
                interfaceType.GetMethod("MapperFrom").Invoke(source, new object[] { destination });
            }
        }
        protected virtual void InvokeCustomTo(object source, object destination)
        {
            var interfaceType = _typeICustomMapperTo.MakeGenericType(destination.GetType());
            if (source.GetType().GetInterfaces().Any(a => a == interfaceType))
            {
                interfaceType.GetMethod("MapperTo").Invoke(source, new object[] { destination });
            }
        }

        public abstract TDestination Map<TSource, TDestination>(TSource source, TDestination desctination);
        public abstract object Map(object source, object desctination);
    }


}
