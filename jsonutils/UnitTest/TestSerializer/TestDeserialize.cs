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

        [TestMethod]
        public void TestArrayOfArray()
        {
            var arr = DeserializeJson<int[][]>(@"[[0, 1, 2], [3, 4], [5]]");
            Assert.AreEqual(arr.Length, 3);
            Assert.IsTrue(arr[0].SequenceEqual(new[] { 0, 1, 2 }));
            Assert.IsTrue(arr[1].SequenceEqual(new[] { 3, 4 }));
            Assert.IsTrue(arr[2].SequenceEqual(new[] { 5 }));
        }

        [TestMethod]
        public void TestNullableValueTypesArray()
        {
            {
                var arr = DeserializeJson<int?[]>(@"[0, null, 2]");
                Assert.AreEqual(arr.Length, 3);
                Assert.AreEqual(arr[0], 0);
                Assert.AreEqual(arr[1], null);
                Assert.AreEqual(arr[2], 2);
            }
            {
                var arr = DeserializeJson<bool?[]>(@"[ true, null, false, ]");
                Assert.AreEqual(arr.Length, 3);
                Assert.AreEqual(arr[0], true);
                Assert.AreEqual(arr[1], null);
                Assert.AreEqual(arr[2], false);
            }
        }

        class TestNullableValueTypesType
        {
            [JsonSerializeOption(key: "key1", required: true)]
            public bool? Key1 { get; set; } = false;

            [JsonSerializeOption(key: "key2", required: true)]
            public bool? Key2 { get; set; } = false;
        }

        [TestMethod]
        public void TestNullableValueTypes()
        {
            var obj = DeserializeJson<TestNullableValueTypesType>(@"{""key1"": true, ""key2"": null}");
            Assert.AreEqual(obj.Key1, true);
            Assert.AreEqual(obj.Key2, null);
        }

        private class TestClassObjectType
        {
            public enum JobType
            {
                None = 0,
                Programmer = 1,
            }

            [JsonSerializeOption(key: "name", required: true)]
            public string Name { get; set; } = "";

            [JsonSerializeOption(key: "age", required: true)]
            public int Age { get; set; } = 0;

            [JsonSerializeOption(key: "married", required: true)]
            public bool Married { get; set; } = false;

            [JsonSerializeOption(key: "lucky-number", required: true)]
            public int? LuckyNumber { get; set; } = 0;

            [JsonSerializeOption(key: "job", required: true)]
            public JobType Job { get; set; } = JobType.None;

            [JsonSerializeOption(key: "hair-count", required: false, defaultValue: -1)]
            public long HairCount { get; set; } = 0;

            [JsonSerializeOption(key: "salary", required: true)]
            public decimal Salary { get; set; } = 0.0m;

            [JsonSerializeOption(key: "assets", required: true)]
            public decimal Assets { get; set; } = 0.0m;

            [JsonSerializeOption(key: "expenditures", required: true)]
            public decimal?[] Expenditures { get; set; } = { };

            [JsonSerializeOption(key: "bmi", required: true)]
            public double BMI { get; set; } = 0.0d;

            public class ChildType
            {
                [JsonSerializeOption(key: "name", required: true)]
                public string Name { get; set; } = "";

                [JsonSerializeOption(key: "age", required: true)]
                public int Age { get; set; } = 0;
            }

            [JsonSerializeOption(key: "children", required: true)]
            public ChildType?[] Children { get; set; } = { };
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
    ""salary"": 3728.28,
    ""assets"": 1.3e5,
    ""bmi"": 21.05,
    ""expenditures"": [20.34, 16, null, 0.00, 35.39],
    ""children"": [
        {
            ""name"": ""Mary"",
            ""age"": 5
        },
        {
            ""name"": ""Jack"",
            ""age"": 2
        },
        null,
    ],
}");
            Assert.AreEqual(deserialized.Name, "Tom");
            Assert.AreEqual(deserialized.Age, 34);
            Assert.AreEqual(deserialized.Married, true);
            Assert.IsNull(deserialized.LuckyNumber);
            Assert.AreEqual(deserialized.Job, TestClassObjectType.JobType.Programmer);
            Assert.AreEqual(deserialized.HairCount, -1);
            Assert.AreEqual(deserialized.Salary, 3728.28m);
            Assert.AreEqual(deserialized.Assets, 1.3e5m);
            Assert.IsTrue(deserialized.Expenditures.SequenceEqual(new decimal?[] { 20.34m, 16m, null, 0.00m, 35.39m }));
            Assert.AreEqual(deserialized.BMI, 21.05d);

            var children = deserialized.Children;
            Assert.AreEqual(children.Length, 3);

            var mary = children[0]!;
            var jack = children[1]!;

            Assert.AreEqual(mary.Name, "Mary");
            Assert.AreEqual(mary.Age, 5);

            Assert.AreEqual(jack.Name, "Jack");
            Assert.AreEqual(jack.Age, 2);

            Assert.IsNull(children[2]);
        }

        class TestMissingRequiredType
        {
            [JsonSerializeOption(key: "key", required: true)]
            public string Key { get; set; } = "";
        }

        [TestMethod]
        public void TestMissingRequired()
        {
            Assert.ThrowsException<JsonKeyNotExistException>(() =>
            {
                DeserializeJson<TestMissingRequiredType>(@"{}");
            });
        }

        [TestMethod]
        public void TestArrayTypeError()
        {
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<object>(@"[]");
            });
        }

        [TestMethod]
        public void TestArrayElementTypeError()
        {
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<object[]>(@"[1, 2, 3]");
            });
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<int[]>(@"[1, 2, {}]");
            });
        }

        class TestPropertiesTypeErrorType
        {
            [JsonSerializeOption(key: "key", required: true)]
            public string Key { get; set; } = "";
        }

        [TestMethod]
        public void TestPropertiesTypeError()
        {
            Assert.ThrowsException<TypeErrorException>(() =>
            {
                DeserializeJson<TestPropertiesTypeErrorType>(@"{""key"": 5}");
            });
        }
    }
}
