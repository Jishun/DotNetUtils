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

        [TestMethod]
        public void LineBreak()
        {
            var str = new SeekableString("4\r\n;5678", LineBreakOption.CrLfToLf);
            var o = str.ReadTo(false, null, ";");
            Assert.AreEqual("4\n;", o);

            str = new SeekableString("4\r\n;5678", LineBreakOption.CrLfToCr);
            o = str.ReadTo(true, null, ";");
            Assert.AreEqual("4\r", o);

            str = new SeekableString("4\r\n;5678", LineBreakOption.LfToCr);
            o = str.ReadTo(true, null, ";");
            Assert.AreEqual("4\r\n", o);

            str = new SeekableString("4\n;5678", LineBreakOption.LfToCr);
            o = str.ReadTo(true, null, ";");
            Assert.AreEqual("4\r", o);

            str = new SeekableString("4\n;5678", LineBreakOption.LfToCrLf);
            o = str.ReadTo(true, null, ";");
            Assert.AreEqual("4\r\n", o);

            str = new SeekableString("4\r;5678", LineBreakOption.CrToCrLf);
            o = str.ReadTo(true, null, ";");
            Assert.AreEqual("4\r\n", o);

            str = new SeekableString("4\r;5678", LineBreakOption.CrToLf);
            o = str.ReadTo(true, null, ";");
            Assert.AreEqual("4\n", o);

            str = new SeekableString("4\r\n5\r;5678", LineBreakOption.CrLfToLf|LineBreakOption.CrToLf);
            o = str.ReadTo(true, null, ";");
            Assert.AreEqual("4\n5\n", o);

            str = new SeekableString("4\r5\n;5678", LineBreakOption.LfToCrLf | LineBreakOption.CrToCrLf);
            o = str.ReadTo(true, null, ";");
            Assert.AreEqual("4\r\n5\r\n", o);
        }


        [TestMethod]
        public void PeekChar()
        {
            var str = new SeekableString("1234");
            Assert.AreEqual((int)'1', str.ReadChar());
            Assert.AreEqual((int)'2', str.PeekChar());
            Assert.AreEqual((int)'2', str.ReadChar());
        }
    }
}
