using System;
using System.Collections.Generic;
using NUnit.Framework;
using Portal.Core.Model.Entities;
using ToYuml.Test.Objects;
using System.Linq;

namespace ToYuml.Test
{
    [TestFixture]
    public class YumlGeneratorTests
    {
        [Test]
        public void Can_Generate_Single_Class_Diagram()
        {
            var types = new List<Type> {typeof (Animal)};
            Assert.AreEqual("http://yuml.me/diagram/scruffy/class/[Animal]", new YumlGenerator(types).Yuml());
        }

        [Test]
        public void Can_Generate_Inherited_Class_Diagram()
        {
            var types = new List<Type> { typeof(Bird), typeof(Animal)};
            Assert.AreEqual("http://yuml.me/diagram/scruffy/class/[Bird],[Bird]^-[Animal],[Animal]", new YumlGenerator(types).Yuml());
        }

        [Test]
        public void Will_Not_Generate_Inherited_Class_Diagram_If_Not_In_List()
        {
            var types = new List<Type> { typeof(Bird) };
            Assert.AreEqual("http://yuml.me/diagram/scruffy/class/[Bird]", new YumlGenerator(types).Yuml());
        }

        [Test]
        public void Can_Generate_Inherited_Class_Diagram_To_Several_Layers()
        {
            var types = new List<Type> { typeof(Eagle), typeof(Bird), typeof(Animal) };
            Assert.AreEqual("http://yuml.me/diagram/scruffy/class/[Eagle],[Eagle]^-[Bird],[Bird]^-[Animal],[Bird],[Animal]", new YumlGenerator(types).Yuml());
        }

        [Test]
        public void Can_Generate_Class_With_Interfaces()
        {
            var types = new List<Type> { typeof(Eagle), typeof(IBirdOfPray) };
            Assert.AreEqual("http://yuml.me/diagram/scruffy/class/[<<IBirdOfPray>>;Eagle]", new YumlGenerator(types).Yuml());
        }

        [Test]
        public void Can_Generate_Class_With_Association()
        {
            var types = new List<Type> { typeof(Eagle), typeof(Claw) };
            Assert.AreEqual("http://yuml.me/diagram/scruffy/class/[Eagle],[Eagle]->[Claw],[Claw]", new YumlGenerator(types).Yuml());
        }

        [Test]
        public void Can_Generate_Class_With_A_Many_Association()
        {
            var types = new List<Type> { typeof(Eagle), typeof(Claw), typeof(Wing) };
            var yuml = new YumlGenerator(types).Yuml();
            Console.WriteLine(yuml);
            Assert.AreEqual("http://yuml.me/diagram/scruffy/class/[Eagle],[Eagle]->[Claw],[Eagle]1-0..*[Wing],[Claw],[Wing]", yuml);
        }

        //1-0..*
        [Test]
        public void Random()
        {
            var types = new List<Type>();
            types.AddRange(new AssemblyFilter(typeof(Folder).Assembly).Types);
            var yuml = new YumlGenerator(new AssemblyFilter(typeof(NHibernate.TransientObjectException).Assembly).Types.Where(t => t.Namespace.Contains("Cache")).ToList()).Yuml();
            Console.WriteLine(yuml);

        }


    }
}
