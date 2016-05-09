using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MapperByAttribute
{
    public class AutoMapperRegister : MapperRegisterBase
    {
        private MapperConfiguration _config = null;
        private IMapper _mapper = null;

        private List<Action<IMapperConfiguration>> configurationList;

        public AutoMapperRegister()
        {
            configurationList = new List<Action<IMapperConfiguration>>();
        }

        protected virtual void Registering(IMapperConfiguration cfg)
        {
            //子类型重写该方法获得 IMapperConfiguration 对象
        }

        public override void AssertConfigurationIsValid()
        {
            _config = new MapperConfiguration(cfg =>
            {
                Registering(cfg);
                foreach (var action in configurationList) action(cfg);
            });
            _config.AssertConfigurationIsValid();
            _mapper = _config.CreateMapper();
        }

        public override void Register(Type type)
        {
            if (!type.IsDefined(typeof(MapperAttribute))) throw new ArgumentException($"注册的类型没有添加 {nameof(MapperAttribute)} 特性。", nameof(type));

            var mapperAttributes = type.GetCustomAttributes<MapperAttribute>();
            var mapperIgnoreProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                            .Where(w => w.IsDefined(typeof(MapperIgnoreAttribute)));

            configurationList.Add(cfg =>
            {
                foreach (var attr in mapperAttributes)
                {
                    IMappingExpression fromDirectExpression = null;

                    if (attr.Direction == MapperDirect.From || attr.Direction == MapperDirect.Both)
                    {
                        fromDirectExpression = cfg.CreateMap(attr.LinkType, type);

                        foreach (var ignoreProperty in mapperIgnoreProperties)
                            fromDirectExpression = fromDirectExpression.ForMember(ignoreProperty.Name, o => o.Ignore());

                        if (HasCustomFrom(attr.LinkType, type)) fromDirectExpression = fromDirectExpression.AfterMap(InvokeCustomFrom);

                    }

                    IMappingExpression reverseDirectExpression = null;

                    if (attr.Direction == MapperDirect.Both)
                    {
                        reverseDirectExpression = fromDirectExpression.ReverseMap();
                    }
                    else if (attr.Direction == MapperDirect.To)
                    {
                        reverseDirectExpression = cfg.CreateMap(type, attr.LinkType);
                    }

                    if (reverseDirectExpression != null)
                    {
                        foreach (var ignoreProperty in mapperIgnoreProperties)
                        {
                            reverseDirectExpression = reverseDirectExpression.ForSourceMember(ignoreProperty.Name, o => o.Ignore());
                            /*if (attr.LinkType.GetProperty(ignoreProperty.Name) != null)
                                reverseDirectExpression = reverseDirectExpression.ForMember(ignoreProperty.Name, o => o.Ignore());*/
                        }

                        if (HasCustomTo(type, attr.LinkType)) reverseDirectExpression = reverseDirectExpression.AfterMap(InvokeCustomTo);
                    }

                }
            });

        }

        public override TDestination Map<TSource, TDestination>(TSource source, TDestination desctination)
        {
            return _mapper.Map(source, desctination);
        }
        public override object Map(object source, object desctination)
        {
            return _mapper.Map(source, desctination);
        }
    }
}