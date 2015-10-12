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

        [TestMethod]
        public void LineNumber()
        {
            var str = new SeekableString("123456789");
            str.ReadTo("3");
            Assert.AreEqual(0, str.Line);
            Assert.AreEqual(3, str.Column);
            Assert.AreEqual(3, str.Position);

            str = new SeekableString("1234\\06789\r\n01");
            str.ReadTo("6");
            str.ReadTo("0");
            Assert.AreEqual(1, str.Line);
            Assert.AreEqual(1, str.Column);
            Assert.AreEqual(0, str.PreviousLine);
            Assert.AreEqual(7, str.PreviousColumn);
            Assert.AreEqual(13, str.Position);

            str = new SeekableString("123\n456789");
            str.ReadTo("6");
            Assert.AreEqual(1, str.Line);
            Assert.AreEqual(3, str.Column);
            Assert.AreEqual(7, str.Position);

            str = new SeekableString("123\n4567\r89");
            str.ReadTo("8");
            Assert.AreEqual(2, str.Line);
            Assert.AreEqual(1, str.Column);

            str = new SeekableString("123\n4567\r89");
            str.ReadTo("56");
            Assert.AreEqual(1, str.Line);
            Assert.AreEqual(3, str.Column);
            Assert.AreEqual(7, str.Position);

            str = new SeekableString("123\\56567\r89");
            var ret = str.ReadTo(true, "\\","56");
            Assert.AreEqual("12356", ret);
            Assert.AreEqual(0, str.Line);
            Assert.AreEqual(8, str.Column);
            Assert.AreEqual(8, str.Position);
        }
    }
}
