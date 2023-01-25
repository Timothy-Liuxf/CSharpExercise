using JsonUtils.Frontend;
using System.Security.Cryptography.X509Certificates;

namespace TestSerializer
{
    [TestClass]
    public class TestDeserialize
    {
        private T DeserializeJson<T>(string json, T _)
        {
            return Serializer.Deserialize<T>(new StringReader(json));
        }

        [TestMethod]
        public void TestSingleInteger()
        {
            int finishCount = 1;
            var TestOnce = (Action f) =>
            {
                f();
                ++finishCount;
            };

            try
            {
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson(@"123", new int()), 123);
                    Assert.AreEqual(DeserializeJson(@"123", new uint()), 123u);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson(@"-123", new long()), -123L);
                    Assert.AreEqual(DeserializeJson(@"+123", new ulong()), +123UL);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson(@"123", new nint()), (nint)123);
                    Assert.AreEqual(DeserializeJson(@"123", new nuint()), (nuint)123U);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson(@"+123", new byte()), (byte)+123U);
                    Assert.AreEqual(DeserializeJson(@"-123", new sbyte()), (sbyte)-123);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson(@"0x789a", new int()), 0x789a);
                    Assert.AreEqual(DeserializeJson(@"0X789A", new int()), 0X789A);
                });
            }
            catch (Exception ex)
            {
                Assert.Fail($"Fail at the {finishCount}-th case. Exception: " + ex.ToString());
            }
        }

        [TestMethod]
        public void TestSingleEnum()
        {
            Assert.AreEqual(DeserializeJson(((int)TypeCode.Double).ToString(), new TypeCode()), TypeCode.Double);
            Assert.AreEqual(DeserializeJson($"0x{(int)TypeCode.Double:x}", new TypeCode()), TypeCode.Double);
        }

        [TestMethod]
        public void TestSingleFloatingPoint()
        {
            int finishCount = 1;
            var TestOnce = (Action f) =>
            {
                f();
                ++finishCount;
            };

            try
            {
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson(@"1.234", new double()), 1.234d);
                    Assert.AreEqual(DeserializeJson(@"1.234", new float()), 1.234f);
                    Assert.AreEqual(DeserializeJson(@"1.234", new decimal()), 1.234m);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson(@"+.234", new double()), +.234d);
                    Assert.AreEqual(DeserializeJson(@"-.234", new float()), -.234f);
                    Assert.AreEqual(DeserializeJson(@"+.234", new decimal()), +.234m);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson(@"1.3e-3", new double()), 1.3e-3d);
                    Assert.AreEqual(DeserializeJson(@"-1.3e3", new float()), -1.3e3f);
                    Assert.AreEqual(DeserializeJson(@"+1.3e+3", new decimal()), +1.3e+3m);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson(@"0XFFFF", new double()), (double)0XFFFF);
                    Assert.AreEqual(DeserializeJson(@"0xffff", new float()), (float)0xffff);
                    Assert.AreEqual(DeserializeJson(@"0xffff", new decimal()), (decimal)0xffff);
                });
            }
            catch (Exception ex)
            {
                Assert.Fail($"Fail at the {finishCount}-th case. Exception: " + ex.ToString());
            }
        }

        [TestMethod]
        public void TestSingleString()
        {
            Assert.AreEqual(DeserializeJson(@"""""", ""), "");
            Assert.AreEqual(DeserializeJson(@"""hello""", ""), "hello");
        }

        [TestMethod]
        public void TestSingleBoolean()
        {
            Assert.AreEqual(DeserializeJson(@"true", new bool()), true);
            Assert.AreEqual(DeserializeJson(@"false", new bool()), false);
        }

        [TestMethod]
        public void TestSingleNull()
        {
            Assert.IsNull(DeserializeJson<int?>(@"null", null));
            Assert.IsNull(DeserializeJson<string?>(@"null", null));
        }
    }
}
