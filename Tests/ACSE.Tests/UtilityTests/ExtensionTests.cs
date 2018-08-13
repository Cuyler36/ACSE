using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACSE.Tests.UtilityTests
{
    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void TestSetBit()
        {
            byte test = 0xF;
            test.SetBit(4, true);
            Assert.AreEqual(test, 0x1F);
        }

        [TestMethod]
        public void TestGetBit()
        {
            const byte test = 0x02;
            Assert.AreEqual(test.GetBit(0), 0);
            Assert.AreEqual(test.GetBit(1), 1);
        }

        [TestMethod]
        public void TestGetBits()
        {
            const byte test = 0x1F;
            var output = test.GetBits();
            CollectionAssert.AreEqual(output as byte[] ?? output.ToArray(), new byte[] {1, 1, 1, 1, 1, 0, 0, 0});
        }

        [TestMethod]
        public void TestGetByte()
        {
            Assert.AreEqual(new byte[] {1, 1, 1, 1, 1, 0, 0, 0}.GetByte(), 0x1F);
            Assert.AreEqual(new byte[] {0, 1}.GetByte(), 2);
        }

        [TestMethod]
        public void TestGetNibbles()
        {
            const byte nibbleTest = 0xA5;
            var (lowerNibble, upperNibble) = nibbleTest.GetNibbles();
            Assert.AreEqual(lowerNibble, 0x5);
            Assert.AreEqual(upperNibble, 0xA);
        }
    }
}
