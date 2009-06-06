using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ToYuml.Test.Objects;

namespace ToYuml.Test
{
    [TestFixture]
    public class AssemblyFilterFixtures
    {

        [Test]
        public void Can_Determine_Class_Name()
        {
            var reflectionHelper = new AssemblyFilter(typeof(Animal).Assembly);
            Assert.That(reflectionHelper.Types.Contains(typeof(Animal)));
        }

    }
}
