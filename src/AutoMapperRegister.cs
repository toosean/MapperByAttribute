using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            if (!type.IsDefined(typeof(MapperAttribute))) throw new ArgumentException($"cant find {nameof(MapperAttribute)} on {type}。", nameof(type));

            var mapperAttributes = type.GetCustomAttributes<MapperAttribute>().ToArray();

            var mapperIgnoreProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                            .Where(w => w.IsDefined(typeof(MapperIgnoreAttribute)))
                                            .Select(s => new {
                                                property = s,
                                                attrs = s.GetCustomAttributes<MapperIgnoreAttribute>().ToArray()
                                            })
                                            .ToArray();

            var mapperForProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                            .Where(w => w.IsDefined(typeof(MapperForAttribute)))
                                            .Select(s => new {
                                                property = s,
                                                attrs = s.GetCustomAttributes<MapperForAttribute>().ToArray()
                                            })
                                            .ToArray();

            configurationList.Add(cfg =>
            {
                foreach (var attr in mapperAttributes)
                {
                    IMappingExpression fromDirectExpression = null;

                    var ignorePropertiesForLinkType = mapperIgnoreProperties.Where(w => w.attrs.Any(a => a.Type == attr.LinkType) || w.attrs.Any(a => a.Type == null)).ToArray();
                    var forPropertiesForLinkeType = mapperForProperties.Where(w => w.attrs.Any(a => a.Type == attr.LinkType) || w.attrs.Any(a => a.Type == null)).ToArray();

                    if (attr.Direction == MapperDirect.From || attr.Direction == MapperDirect.Both)
                    {
                        fromDirectExpression = cfg.CreateMap(attr.LinkType, type);

                        foreach (var ignoreProperty in ignorePropertiesForLinkType)
                            fromDirectExpression = fromDirectExpression.ForMember(ignoreProperty.property.Name, o => o.Ignore());

                        foreach (var forProperty in forPropertiesForLinkeType)
                        {
                            var forAttr = forProperty.attrs.FirstOrDefault(f => f.Type == attr.LinkType) ?? forProperty.attrs.First();
                            fromDirectExpression = fromDirectExpression.ForMember(forProperty.property.Name, o => o.MapFrom(forAttr.ForName));
                        }

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
                        foreach (var ignoreProperty in ignorePropertiesForLinkType)
                            reverseDirectExpression = reverseDirectExpression.ForSourceMember(ignoreProperty.property.Name, o => o.Ignore());

                        foreach (var forProperty in forPropertiesForLinkeType)
                        {
                            var forAttr = forProperty.attrs.FirstOrDefault(f => f.Type == attr.LinkType) ?? forProperty.attrs.First();
                            reverseDirectExpression = reverseDirectExpression.ForMember(forAttr.ForName, o => o.MapFrom(forProperty.property.Name));
                        }

                        if (HasCustomTo(type, attr.LinkType)) reverseDirectExpression = reverseDirectExpression.AfterMap(InvokeCustomTo);
                    }

                }
            });

        }

        public override TDestination Map<TSource, TDestination>(TSource source, TDestination desctination)
        {
            if (_mapper == null) throw new InvalidOperationException("call AssertConfigurationIsValid is require.");
            return _mapper.Map(source, desctination);
        }
        public override object Map(object source, object desctination)
        {
            if (_mapper == null) throw new InvalidOperationException("call AssertConfigurationIsValid is require.");
            return _mapper.Map(source, desctination);
        }
    }
}