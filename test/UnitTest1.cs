using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperByAttribute;

namespace Test
{
    class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }

    [MapperTwoDirection(typeof(Entity))]
    class Dto 
        : ICustomMapperTo<Entity>
        , ICustomMapperFrom<Entity>
    {
        public string Name { get; set; }

        [MapperIgnore]
        public string Password { get; set; }

        [MapperIgnore]
        public int SourcePasswordLength { get; set; }

        void ICustomMapperFrom<Entity>.MapperFrom(Entity source)
        {
            SourcePasswordLength = source.Password == null ? 0 : source.Password.Length;
        }

        void ICustomMapperTo<Entity>.MapperTo(Entity destination)
        {
            destination.Password = this.Password != null ? "Hash" + this.Password : null;
        }
    }

    [TestClass]
    public class UnitTest1
    {

        private Entity entity = new Entity { Id = 1, Name = "Sean", Password = "[Hash Password Bytes]" };

        private Dto Init()
        {
            var mapper = new AutoMapperRegister();
            mapper.Register<Dto>();
            mapper.AssertConfigurationIsValid();
            
            var dto = mapper.Map(entity, new Dto());
            return dto;
        }

        [TestMethod]
        public void TestNormal()
        {
            var dto = Init();
            Assert.IsTrue(dto.Name == entity.Name);
            
        }

        [TestMethod]
        public void TestIgnore()
        {
            var dto = Init();
            Assert.IsNull(dto.Password);
        }

        [TestMethod]
        public void TestCustomMapperFrom()
        {
            var dto = Init();
            Assert.IsTrue(dto.SourcePasswordLength == entity.Password.Length);
        }

        [TestMethod]
        public void TestCustomMapperTo()
        {

            var mapper = new AutoMapperRegister();
            mapper.Register<Dto>();
            mapper.AssertConfigurationIsValid();

            var entity = new Entity { Id = 1, Name = "Sean", Password = "[Hash Password Bytes]" };
            var dto = mapper.Map(entity, new Dto());
            dto.Password = "new password";
            entity = mapper.Map(dto, entity);


            Assert.IsTrue(entity.Password.StartsWith("Hash"));
        }
    }
}
