# MapperByAttribute
使用 Attribute 来配置 AutoMapper
AutoMapper 见 https://github.com/AutoMapper/AutoMapper

##使用方法
```cs
class Program
{
    static void Main(string[] args)
    {
        var mapper = new AutoMapperRegister();

        //登记 Dto 对象
        mapper.Register<Dto>();
        //登记一个程序集里所有的对象
        mapper.Register(typeof(Dto).Assembly);
        
        //必须验证，否则 mapper 不生效
        mapper.AssertConfigurationIsValid();

        var entity = new Entity { Id = 1, Name = "Sean", Password = "[Hash Password Bytes]" };

        var dto = mapper.Map(entity, new Dto());

    }
}
class Entity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
}

[MapperTwoDirection(typeof(Entity))]
class Dto : ICustomMapperTo<Entity>
{
    public string Name { get; set; }

    [MapperIgnore]
    public  string Password { get; set; }

    void ICustomMapperTo<Entity>.MapperTo(Entity destination)
    {
        //destination.Password = Hash(this.Password);
    }
}
```

