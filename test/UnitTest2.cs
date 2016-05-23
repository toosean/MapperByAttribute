using MapperByAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class UnitTest2
    {
        class EntityA
        {
            public string P1 { get; set; }

            public string PA { get; set; }

            public string Custom { get; set; }
        }

        class EntityB
        {
            public string P2 { get; set; }

            public string PA { get; set; }

            public string Custom { get; set; }
        }

        [MapperTwoDirection(typeof(EntityA))]
        [MapperTwoDirection(typeof(EntityB))]
        class DtoA 
            : ICustomMapperFrom<EntityA>
            , ICustomMapperTo<EntityA>
            , ICustomMapperFrom<EntityB>
            , ICustomMapperTo<EntityB>
        {
            [MapperFor(typeof(EntityA), nameof(EntityA.P1))]
            [MapperFor(typeof(EntityB), nameof(EntityB.P2))]
            public string _p { get; set; }

            [MapperIgnore(typeof(EntityA))]
            public string PA { get; set; }

            [MapperIgnore]
            public string Ct { get; set; }


            void ICustomMapperFrom<EntityA>.MapperFrom(EntityA source)
            {
                Ct = "MapperFromEntityA";
            }
            void ICustomMapperTo<EntityA>.MapperTo(EntityA destination)
            {
                destination.Custom = "MapperToEntityA";
            }
            void ICustomMapperFrom<EntityB>.MapperFrom(EntityB source)
            {
                Ct = "MapperFromEntityB";
            }
            void ICustomMapperTo<EntityB>.MapperTo(EntityB destination)
            {
                destination.Custom = "MapperToEntityB";
            }
        }


        [TestMethod]
        public void TestMapperFor()
        {
            var mapper = new AutoMapperRegister();
            mapper.Register<DtoA>();
            mapper.AssertConfigurationIsValid();

            var dto = mapper.Map(new EntityA { P1 = "P_1" }, new DtoA());
            Assert.IsTrue(dto._p == "P_1");
            Assert.IsTrue(dto.Ct == "MapperFromEntityA");

            dto = mapper.Map(new EntityB { P2 = "P_2" }, new DtoA());
            Assert.IsTrue(dto._p == "P_2");
            Assert.IsTrue(dto.Ct == "MapperFromEntityB");

            dto = mapper.Map(new EntityA { PA = "P_1A" }, new DtoA());
            Assert.IsTrue(dto.PA == null);

            dto = mapper.Map(new EntityB { PA = "P_2A" }, new DtoA());
            Assert.IsTrue(dto.PA == "P_2A");

            var entityA = new EntityA();
            var entityB = new EntityB();

            mapper.Map(new DtoA(),entityA);
            Assert.IsTrue(entityA.Custom == "MapperToEntityA");
            mapper.Map(new DtoA(), entityB);
            Assert.IsTrue(entityB.Custom == "MapperToEntityB");


        }
    }
}
