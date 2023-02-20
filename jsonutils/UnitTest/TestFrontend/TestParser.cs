namespace TestFrontend
{
    [TestClass]
    public class TestParser
    {
        private static void ParseJsonString(string json)
        {
            new FrontEnd(new StringReader(json)).Parse();
        }

        [TestMethod]
        public void TestSingleNumber()
        {
            ParseJsonString(@"3.1415926");
        }

        [TestMethod]
        public void TestSingleBoolean()
        {
            ParseJsonString(@"true");
            ParseJsonString(@"false");
        }

        [TestMethod]
        public void TestSingleString()
        {
            ParseJsonString(@"""string""");
        }

        [TestMethod]
        public void TestSingleNull()
        {
            ParseJsonString(@"""null""");
        }

        [TestMethod]
        public void TestEmptyObject()
        {
            ParseJsonString(@"{}");
        }

        [TestMethod]
        public void TestEmptyArray()
        {
            ParseJsonString(@"[]");
        }

        [TestMethod]
        public void TestBadKey()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"{ 114: 514 }");
            });
        }

        [TestMethod]
        public void TestMissingBrace()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"{ ""key"": ""value"" ");
                ParseJsonString(@"{ ""key"": ""value"", ");
            });
        }

        [TestMethod]
        public void TestMissingBracket()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"[ 123, 888 ");
                ParseJsonString(@"[ 123, 888, ");
            });
        }

        [TestMethod]
        public void TestMissingValue()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"{ ""key"": }");
                ParseJsonString(@"{ ""key"": , }");
            });
        }

        [TestMethod]
        public void TestMissingColon()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"{ ""key"" 888 }");
            });
        }

        [TestMethod]
        public void TestBadProperty()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"{ ""key"" }");
            });
        }

        [TestMethod]
        public void TestBadArrayElement()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"[ ""key"": 888 ]");
            });
        }

        [TestMethod]
        public void TestExtraContent()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"{},");
            });
        }

        [TestMethod]
        public void TestLineComment()
        {
            ParseJsonString(@"{}// hahaha");
        }

        [TestMethod]
        public void TestExtraLineComment()
        {
            ParseJsonString(@"{}//");
        }

        [TestMethod]
        public void TestEmptyContent()
        {
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"");
            });
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"  ");
            });
            Assert.ThrowsException<SyntaxErrorException>(() =>
            {
                ParseJsonString(@"//{}");
            });
        }

        [TestMethod]
        public void TestAll()
        {
            try
            {
                var json =
    @" // Head comment
{
    // comment
    ""key1"": true,    // key1
    ""key2"": false,   // key2
    ""key3"": -1.6e+103,
    ""key4"": ""strVal"",
    ""key5"": null,
    ""key6"":          // comment after colon
        null,
    ""key7""           // comment after key
            :
        null           // comment before comma
        ,
    ""arr"": [         // arr
        {
            ""key11""   :59,""key12"" :  666 ,""key13"":""""    // arr key1
            ,""key14"":0    ,    ""key15"":[],""key16"" : {}  ,
            ""key17"":{""key21"":{} ,},""key18"":[11,0x99ff,22,0X000,]
        },
        {   // empty object
        },
        {
        },
    ],
    'single': true,     // single quote string
}
// tailing comment
";
                ParseJsonString(json);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
