using JsonUtils.Frontend;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.X509Certificates;

namespace TestSerializer
{
    [TestClass]
    public class TestDeserialize
    {
        private T DeserializeJson<T>(string json)
        {
            return Serializer.Deserialize<T>(new StringReader(json));
        }

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
                    Assert.AreEqual(DeserializeJson<int>(@"123"), 123);
                    Assert.AreEqual(DeserializeJson<uint>(@"123"), 123u);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson<long>(@"-123"), -123L);
                    Assert.AreEqual(DeserializeJson<ulong>(@"+123"), +123UL);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson<nint>(@"123"), (nint)123);
                    Assert.AreEqual(DeserializeJson<nuint>(@"123"), (nuint)123U);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson<byte>(@"+123"), (byte)+123U);
                    Assert.AreEqual(DeserializeJson<sbyte>(@"-123"), (sbyte)-123);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson<int>(@"0x789a"), 0x789a);
                    Assert.AreEqual(DeserializeJson<int>(@"0X789A"), 0X789A);
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
            Assert.AreEqual(DeserializeJson<TypeCode>(((int)TypeCode.Double).ToString()), TypeCode.Double);
            Assert.AreEqual(DeserializeJson<TypeCode>($"0x{(int)TypeCode.Double:x}"), TypeCode.Double);
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
                    Assert.AreEqual(DeserializeJson<double>(@"1.234"), 1.234d);
                    Assert.AreEqual(DeserializeJson<float>(@"1.234"), 1.234f);
                    Assert.AreEqual(DeserializeJson<decimal>(@"1.234"), 1.234m);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson<double>(@"+.234"), +.234d);
                    Assert.AreEqual(DeserializeJson<float>(@"-.234"), -.234f);
                    Assert.AreEqual(DeserializeJson<decimal>(@"+.234"), +.234m);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson<double>(@"1.3e-3"), 1.3e-3d);
                    Assert.AreEqual(DeserializeJson<float>(@"-1.3e3"), -1.3e3f);
                    Assert.AreEqual(DeserializeJson<decimal>(@"+1.3e+3"), +1.3e+3m);
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(DeserializeJson<double>(@"0XFFFF"), (double)0XFFFF);
                    Assert.AreEqual(DeserializeJson<float>(@"0xffff"), (float)0xffff);
                    Assert.AreEqual(DeserializeJson<decimal>(@"0xffff"), (decimal)0xffff);
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
            Assert.AreEqual(DeserializeJson<string>(@""""""), "");
            Assert.AreEqual(DeserializeJson<string>(@"""hello"""), "hello");
        }

        [TestMethod]
        public void TestSingleBoolean()
        {
            Assert.AreEqual(DeserializeJson<bool>(@"true"), true);
            Assert.AreEqual(DeserializeJson<bool>(@"false"), false);
        }

        [TestMethod]
        public void TestSingleNull()
        {
            Assert.IsNull(DeserializeJson<int?>(@"null"));
            Assert.IsNull(DeserializeJson<string?>(@"null"));
        }

        [TestMethod]
        public void TestFailedNull()
        {
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<int>(@"null");
            });
        }

        [TestMethod]
        public void TestFailedBoolean()
        {
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<int>(@"true");
                DeserializeJson<int>(@"false");
            });
        }

        [TestMethod]
        public void TestFailedString()
        {
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<int>(@"""string""");
            });
        }

        [TestMethod]
        public void TestFailedNumber()
        {
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<bool>(@"""123""");
            });
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<bool>(@"""-123.63""");
            });
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<bool>(@"""+1.3e3""");
            });
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<bool>(@"""0xffff""");
            });
        }

        [TestMethod]
        public void TestEmptyClassObject()
        {
            DeserializeJson(@"{}", new { });
        }

        [TestMethod]
        public void TestEmptyArrayuObject()
        {
            DeserializeJson<object[]>(@"[]");
        }

        [TestMethod]
        public void TestArrayObject()
        {
            var ans = new int[] { 1, 2, 3 };
            var arr = DeserializeJson<int[]>(@"[1, 2, 3]");
            Assert.IsTrue(ans.SequenceEqual(arr), $"Expect [1, 2, 3], got [{arr[0]}, {arr[1]}, {arr[2]}]");
        }

        private class TestClassObjectType
        {
            public enum JobType
            {
                None = 0,
                Programmer = 1,
            }

            [JsonSerializeOption("name", true)]
            public string Name { get; set; } = "";

            [JsonSerializeOption("age", true)]
            public int Age { get; set; } = 0;

            [JsonSerializeOption("married", true)]
            public bool Married { get; set; } = false;

            [JsonSerializeOption("lucky-number", true)]
            public int? LuckyNumber { get; set; } = 0;

            [JsonSerializeOption("job", true)]
            public JobType Job { get; set; } = JobType.None;

            [JsonSerializeOption("hair-count", false, -1)]
            public long HairCount { get; set; } = 0;

            public class ChildType
            {
                [JsonSerializeOption("name", true)]
                public string Name { get; set; } = "";

                [JsonSerializeOption("age", true)]
                public int Age { get; set; } = 0;
            }

            [JsonSerializeOption("children", true)]
            public ChildType[] Children { get; set; } = { };
        }

        [TestMethod]
        public void TestClassObject()
        {
            var deserialized = DeserializeJson<TestClassObjectType>(
@"{
    ""name"": ""Tom"",
    ""age"": 34,
    ""married"": true,
    ""lucky-number"": null,
    ""job"": 1,
    ""children"": [
        {
            ""name"": ""Mary"",
            ""age"": 5
        },
        {
            ""name"": ""Jack"",
            ""age"": 2
        },
    ]
}");
            Assert.AreEqual(deserialized.Name, "Tom");
            Assert.AreEqual(deserialized.Age, 34);
            Assert.AreEqual(deserialized.Married, true);
            Assert.IsNull(deserialized.LuckyNumber);
            Assert.AreEqual(deserialized.Job, TestClassObjectType.JobType.Programmer);
            Assert.AreEqual(deserialized.HairCount, -1);

            var children = deserialized.Children;
            Assert.AreEqual(children.Length, 2);

            var mary = children[0];
            var jack = children[1];

            Assert.AreEqual(mary.Name, "Mary");
            Assert.AreEqual(mary.Age, 5);

            Assert.AreEqual(jack.Name, "Jack");
            Assert.AreEqual(jack.Age, 2);
        }
    }
}
