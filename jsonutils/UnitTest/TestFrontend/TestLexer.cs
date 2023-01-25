namespace TestFrontend
{
    [TestClass]
    public class TestLexer
    {
        private static void LexJsonString(string json)
        {
            new FrontEnd(new StringReader(json)).Lex();
        }

        [TestMethod]
        public void TestEmptyObject()
        {
            LexJsonString(@"{}");
        }

        [TestMethod]
        public void TestEmptyArray()
        {
            LexJsonString(@"[]");
        }

        [TestMethod]
        public void TestString()
        {
            LexJsonString(@"""""");
            LexJsonString(@"""string""");
        }

        [TestMethod]
        public void TestNull()
        {
            LexJsonString(@"null");
        }

        [TestMethod]
        public void TestBoolean()
        {
            LexJsonString(@"true");
            LexJsonString(@"false");
        }

        [TestMethod]
        public void TestFloatingPointNumber()
        {
            int finishCount = 1;
            var TestOnce = (string json) =>
            {
                LexJsonString(json);
                ++finishCount;
            };

            try
            {
                TestOnce(@"5");
                TestOnce(@"+5");
                TestOnce(@"-5");
                TestOnce(@"0");
                TestOnce(@"129");
                TestOnce(@"-259");
                TestOnce(@"0.126");
                TestOnce(@"+0.126");
                TestOnce(@"-0.126");
                TestOnce(@"18.375000");
                TestOnce(@"+27.2360");
                TestOnce(@"-114.514");
                TestOnce(@".386");
                TestOnce(@"+.398");
                TestOnce(@"-.7890");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Fail at the {finishCount}-th case. Exception: " + ex.ToString());
            }
        }

        [TestMethod]
        public void TestScienceNumber()
        {
            int finishCount = 1;
            var TestOnce = (string json) =>
            {
                LexJsonString(json);
                ++finishCount;
            };

            try
            {
                TestOnce(@"1e3");
                TestOnce(@"1E3");
                TestOnce(@"+1e3");
                TestOnce(@"-1e3");
                TestOnce(@"8e1");
                TestOnce(@"8.3e4");
                TestOnce(@"8.380e+103");
                TestOnce(@"8.308000e-236");
                TestOnce(@"3.1415e0");
                TestOnce(@"8.380E+103");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Fail at the {finishCount}-th case. Exception: " + ex.ToString());
            }
        }

        [TestMethod]
        public void TestHexNumber()
        {
            int finishCount = 1;
            var TestOnce = (string json) =>
            {
                LexJsonString(json);
                ++finishCount;
            };

            try
            {
                TestOnce(@"0x0");
                TestOnce(@"0X0");
                TestOnce(@"0x0000");
                TestOnce(@"0X0000");
                TestOnce(@"0x1234");
                TestOnce(@"0X1234");
                TestOnce(@"0x0fff");
                TestOnce(@"0X0FFF");
                TestOnce(@"0x1234567890abcdef");
                TestOnce(@"0X1234567890ABCDEF");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Fail at the {finishCount}-th case. Exception: " + ex.ToString());
            }
        }

        [TestMethod]
        public void TestFailedNumber()
        {
            int finishCount = 1;
            var TestOnce = (string json) =>
            {
                Assert.ThrowsException<SyntaxErrorException>(() =>
                {
                    LexJsonString(json);
                });
                ++finishCount;
            };

            try
            {
                TestOnce(@"00");
                TestOnce(@"+00");
                TestOnce(@"-00");
                TestOnce(@"00.3");
                TestOnce(@"+00.3");
                TestOnce(@"-00.3");
                TestOnce(@"e8");
                TestOnce(@"+e8");
                TestOnce(@"-e8");
                TestOnce(@"E8");
                TestOnce(@"+E8");
                TestOnce(@"-E8");
                TestOnce(@"e");
                TestOnce(@"E");
                TestOnce(@"3e");
                TestOnce(@"3e3.6");
                TestOnce(@"3e.6");
                TestOnce(@"0e1");
                TestOnce(@"18e3");
                TestOnce(@"-0x1234");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Fail at the {finishCount}-th case. Exception: " + ex.ToString());
            }
        }

        [TestMethod]
        public void TestClassObject()
        {
            var json =
@"{
    ""key"": ""value""
}";
            LexJsonString(json);
        }

        [TestMethod]
        public void TestArrayObject()
        {
            var json =
@"[
    { ""key"": ""value"" }
]";
            LexJsonString(json);
        }

        [TestMethod]
        public void TestTrailingComma()
        {
            var json =
@"{
    ""key"": [ ""value"", ],
}";
            LexJsonString(json);
        }

        [TestMethod]
        public void TestBadToken()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                LexJsonString(@"treuh");
                LexJsonString(@"flase");
                LexJsonString(@"nul");
                LexJsonString(@"blablabla");
            });
        }

        [TestMethod]
        public void TestMissingQuote()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                LexJsonString(@"""blabla");
            });
        }

        [TestMethod]
        public void TestAll()
        {
            var json =
@"{
    ""key1"": true,
    ""key2"": false,
    ""key3"": -1.6e+103,
    ""key4"": ""strVal"",
    ""key5"": null,
    ""arr"": [
        {
            ""key11""   :59,""key12"" :  666 ,""key13"":""""
            ,""key14"":0    ,    ""key15"":[],""key16"" : {}  ,
            ""key17"":{""key21"":{} ,},""key18"":[11,0x99ff,22,0X000,]
        },
        {
        }
    ]
}";
            LexJsonString(json);
        }
    }
}
