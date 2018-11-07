using ACSE.Core.Saves.Checksums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACSE.Tests
{
    [TestClass]
    public class ChecksumTests
    {
        private static readonly byte[] TestBuffer = {0xAB, 0x48, 0x1F, 0xC2, 0x99, 0xD3};

        [TestMethod]
        public void TestUInt16BEChecksum()
        {
            Assert.AreEqual(new UInt16BEChecksum().Calculate(TestBuffer, 0), 0x466B);
        }

        [TestMethod]
        public void TestUInt16LEChecksum()
        {
            Assert.AreEqual(new UInt16LEChecksum().Calculate(TestBuffer, 0), 0x6A48);
        }

        [TestMethod]
        public void TestCRC32Checksum()
        {
            Assert.AreEqual(new CRC32().Calculate(TestBuffer), 0x8F92E4DC);
        }

        [TestMethod]
        public void TestNewLeafCRC32Type1()
        {
            Assert.AreEqual(new NewLeafCRC32Reflected().Calculate(TestBuffer), 0xE764E245);
        }

        [TestMethod]
        public void TestNewLeafCRC32Type2()
        {
            Assert.AreEqual(new NewLeafCRC32Normal().Calculate(TestBuffer), 0xDC89E85C);
        }
    }
}
