using JsonUtils.Frontend;

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
        public void TestAll()
        {
            try
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
                ParseJsonString(json);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
