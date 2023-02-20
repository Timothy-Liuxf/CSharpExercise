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
                    Assert.AreEqual(123, DeserializeJson<int>(@"123"));
                    Assert.AreEqual(123u, DeserializeJson<uint>(@"123"));
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(-123L, DeserializeJson<long>(@"-123"));
                    Assert.AreEqual(+123UL, DeserializeJson<ulong>(@"+123"));
                });
                TestOnce(() =>
                {
                    Assert.AreEqual((nint)123, DeserializeJson<nint>(@"123"));
                    Assert.AreEqual((nuint)123U, DeserializeJson<nuint>(@"123"));
                });
                TestOnce(() =>
                {
                    Assert.AreEqual((byte)+123U, DeserializeJson<byte>(@"+123"));
                    Assert.AreEqual((sbyte)-123, DeserializeJson<sbyte>(@"-123"));
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(0x789a, DeserializeJson<int>(@"0x789a"));
                    Assert.AreEqual(0X789A, DeserializeJson<int>(@"0X789A"));
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
            Assert.AreEqual(TypeCode.Double, DeserializeJson<TypeCode>(((int)TypeCode.Double).ToString()));
            Assert.AreEqual(TypeCode.Double, DeserializeJson<TypeCode>($"0x{(int)TypeCode.Double:x}"));
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
                    Assert.AreEqual(1.234d, DeserializeJson<double>(@"1.234"));
                    Assert.AreEqual(1.234f, DeserializeJson<float>(@"1.234"));
                    Assert.AreEqual(1.234m, DeserializeJson<decimal>(@"1.234"));
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(+.234d, DeserializeJson<double>(@"+.234"));
                    Assert.AreEqual(-.234f, DeserializeJson<float>(@"-.234"));
                    Assert.AreEqual(+.234m, DeserializeJson<decimal>(@"+.234"));
                });
                TestOnce(() =>
                {
                    Assert.AreEqual(1.3e-3d, DeserializeJson<double>(@"1.3e-3"));
                    Assert.AreEqual(-1.3e3f, DeserializeJson<float>(@"-1.3e3"));
                    Assert.AreEqual(+1.3e+3m, DeserializeJson<decimal>(@"+1.3e+3"));
                });
                TestOnce(() =>
                {
                    Assert.AreEqual((double)0XFFFF, DeserializeJson<double>(@"0XFFFF"));
                    Assert.AreEqual((float)0xffff, DeserializeJson<float>(@"0xffff"));
                    Assert.AreEqual((decimal)0xffff, DeserializeJson<decimal>(@"0xffff"));
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
            Assert.AreEqual("", DeserializeJson<string>(@""""""));
            Assert.AreEqual("hello", DeserializeJson<string>(@"""hello"""));
            Assert.AreEqual("hel\'lo", DeserializeJson<string>(@"""hel'lo"""));
            Assert.AreEqual("", DeserializeJson<string>(@"''"));
            Assert.AreEqual("hello", DeserializeJson<string>(@"'hello'"));
            Assert.AreEqual("hel\"lo", DeserializeJson<string>(@"'hel""lo'"));
        }

        [TestMethod]
        public void TestSingleBoolean()
        {
            Assert.AreEqual(true, DeserializeJson<bool>(@"true"));
            Assert.AreEqual(false, DeserializeJson<bool>(@"false"));
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
            Assert.AreEqual(3, arr.Length);
            Assert.IsTrue(arr[0].SequenceEqual(new[] { 0, 1, 2 }));
            Assert.IsTrue(arr[1].SequenceEqual(new[] { 3, 4 }));
            Assert.IsTrue(arr[2].SequenceEqual(new[] { 5 }));
        }

        [TestMethod]
        public void TestNullableValueTypesArray()
        {
            {
                var arr = DeserializeJson<int?[]>(@"[0, null, 2]");
                Assert.AreEqual(3, arr.Length);
                Assert.AreEqual(0, arr[0]);
                Assert.AreEqual(null, arr[1]);
                Assert.AreEqual(2, arr[2]);
            }
            {
                var arr = DeserializeJson<bool?[]>(@"[ true, null, false, ]");
                Assert.AreEqual(3, arr.Length);
                Assert.AreEqual(true, arr[0]);
                Assert.AreEqual(null, arr[1]);
                Assert.AreEqual(false, arr[2]);
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
            Assert.AreEqual(true, obj.Key1);
            Assert.AreEqual(null, obj.Key2);
        }

        [TestMethod]
        public void TestEscapingCharacter()
        {
            var str = DeserializeJson<string>(@"""0123\""0123\/\\\b\f\n\r\t\u007B\u7A23""");
            Assert.AreEqual("0123\"0123/\\\b\f\n\r\t\u007B\u7A23", str);
        }

        [TestMethod]
        public void TestFailedscapingCharacter()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                DeserializeJson<string>(@"""0123\d""");
            });
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                DeserializeJson<string>(@"""\u62RA""");
            });
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                DeserializeJson<string>(@"""\u6B2""");
            });
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
            Assert.AreEqual("Tom", deserialized.Name);
            Assert.AreEqual(34, deserialized.Age);
            Assert.AreEqual(true, deserialized.Married);
            Assert.IsNull(deserialized.LuckyNumber);
            Assert.AreEqual(TestClassObjectType.JobType.Programmer, deserialized.Job);
            Assert.AreEqual(-1, deserialized.HairCount);
            Assert.AreEqual(3728.28m, deserialized.Salary);
            Assert.AreEqual(1.3e5m, deserialized.Assets);
            Assert.IsTrue(deserialized.Expenditures.SequenceEqual(new decimal?[] { 20.34m, 16m, null, 0.00m, 35.39m }));
            Assert.AreEqual(21.05d, deserialized.BMI);

            var children = deserialized.Children;
            Assert.AreEqual(3, children.Length);

            var mary = children[0]!;
            var jack = children[1]!;

            Assert.AreEqual("Mary", mary.Name);
            Assert.AreEqual(5, mary.Age);

            Assert.AreEqual("Jack", jack.Name);
            Assert.AreEqual(2, jack.Age);

            Assert.IsNull(children[2]);
        }

        [TestMethod]
        public void TestClassObjectWithComment()
        {
            var deserialized = DeserializeJson<TestClassObjectType>(
@"
// Json file
{
    // Information of Tom

    ""name"": ""Tom"",      // Name
    ""age"": 34,            // Age
    ""married""             // Married Key
                :           // Colon
            true            // Value
            ,               // Comma
    ""lucky-number"": null, // Lucky number
    'job': 1,
    'salary': 3728.28,
    'assets': 1.3e5,
    'bmi': 21.05,
    'expenditures': [20.34, 16, null, 0.00, 35.39],
    'children':           // Children
    [                       // Begin children array
        {
            ""name"": ""Mary"",
            ""age"": 5
        },                  // Mary children information
        {
            ""name"": ""Jack"",
            ""age"": 2
        },                  // Jack children information
        null,               // End children array
    ],
}                           // End object
// Trailing comment
");
            Assert.AreEqual("Tom", deserialized.Name);
            Assert.AreEqual(34, deserialized.Age);
            Assert.AreEqual(true, deserialized.Married);
            Assert.IsNull(deserialized.LuckyNumber);
            Assert.AreEqual(TestClassObjectType.JobType.Programmer, deserialized.Job);
            Assert.AreEqual(-1, deserialized.HairCount);
            Assert.AreEqual(3728.28m, deserialized.Salary);
            Assert.AreEqual(1.3e5m, deserialized.Assets);
            Assert.IsTrue(deserialized.Expenditures.SequenceEqual(new decimal?[] { 20.34m, 16m, null, 0.00m, 35.39m }));
            Assert.AreEqual(21.05d, deserialized.BMI);

            var children = deserialized.Children;
            Assert.AreEqual(3, children.Length);

            var mary = children[0]!;
            var jack = children[1]!;

            Assert.AreEqual("Mary", mary.Name);
            Assert.AreEqual(5, mary.Age);

            Assert.AreEqual("Jack", jack.Name);
            Assert.AreEqual(2, jack.Age);

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
