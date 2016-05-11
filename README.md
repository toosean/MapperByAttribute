# MapperByAttribute
using C# attribute configuration AutoMapper

see AutoMapper https://github.com/AutoMapper/AutoMapper

##How
```cs
class Program
{
    static void Main(string[] args)
    {
        var mapper = new AutoMapperRegister();

        //register dto.
        mapper.Register<Dto>();
        //or you can register all with assembly.
        mapper.Register(typeof(Dto).Assembly);
        
        //require call AssertConfigurationIsValid before call map
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

[Mapper(typeof(Entity))]
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

