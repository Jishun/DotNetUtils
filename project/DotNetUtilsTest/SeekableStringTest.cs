using System;
using DotNetUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetUtilsTest
{
    [TestClass]
    public class SeekableStringTest
    {
        [TestMethod]
        public void ReadTo()
        {
            var str = new SeekableString("1234;5678");
            Assert.AreEqual("1234", str.ReadTo(true, null, ";"));
            str.Position = 0;
            Assert.AreEqual("1234", str.ReadTo(true, "/", ";"));
            str.Position = 0;
            Assert.AreEqual("1234;", str.ReadTo(false, "/", ";"));

            str = new SeekableString("1234/;5678");
            Assert.AreEqual("1234;5678", str.ReadTo(false, "/", ";"));
            str.Position = 0;
            Assert.AreEqual("1234;5678", str.ReadTo(false, "/", ";", ","));


            str = new SeekableString("1234;;5678");
            Assert.AreEqual("1234;;", str.ReadTo(false, "/", ";;"));
            str.Position = 0;
            Assert.AreEqual("1234", str.ReadTo(true, "/", ";;"));
            str.Position = 0;
            Assert.AreEqual("1234;;", str.ReadTo(true, "/", "5678"));
            str.Position = 0;
            Assert.AreEqual("1234;;5678", str.ReadTo(true, "/", "xxx"));
            str.Position = 0;
            Assert.AreEqual("", str.ReadTo(true, "/", "1234"));
            str.Position = 0;
            Assert.AreEqual("", str.ReadTo(true, "/", "1"));
            str.Position = 0;
            Assert.AreEqual("1234;;", str.ReadTo(true, null, "5"));
            str.Position = 0;
            Assert.AreEqual("12345678", str.ReadTo(true, ";;", "5"));
            str.Position = 0;
            Assert.AreEqual("1234;;567", str.ReadTo(true, ";;", "8"));
        }
    }
}
