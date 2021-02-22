using Xunit;

using Sprocket.Args;

namespace ArgsTest
{

    public class ArgsTest_Quotes : test_base
    {
        private readonly string xml = @"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filename' required='false' allow_multiple='false' />
                        <option name='action' required='false' allow_multiple='false' />
                        <option name='delete' required='false' allow_multiple='false' />
                    </command>";


        [Fact]
        public void Test_Quotes_OptionNameAsValue_WithOptNames_NoCompetingKeywords_A()
        {
            Test_Quotes_OptionNameAsValue_WithOptNames_NoCompetingKeywords(@"list filename=hello.txt action=purge");
        }

        [Fact]
        public void Test_Quotes_OptionNameAsValue_WithOptNames_NoCompetingKeywords_B()
        {
            Test_Quotes_OptionNameAsValue_WithOptNames_NoCompetingKeywords(@"list filename=hello.txt purge");
        }

        [Fact]
        public void Test_Quotes_OptionNameAsValue_WithOptNames_NoCompetingKeywords_C()
        {
            Test_Quotes_OptionNameAsValue_WithOptNames_NoCompetingKeywords(@"list hello.txt purge");
        }

        private void Test_Quotes_OptionNameAsValue_WithOptNames_NoCompetingKeywords(string input)
        {
            Processor p = new();

            p.LoadDefinitionsFromXML(this.xml);

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filename", 0, "hello.txt");
            AssertExpectCommandOptionValue(res, 0, "list", 1, "action", 0, "purge");
        }


        [Fact]
        public void Test_Quotes_OptionNameAsValue_WithOptNames_OptKeyWordTreatedAsValue_A()
        {
            Test_Quotes_OptionNameAsValue_WithOptNames_OptKeyWordTreatedAsValue(@"list filename=hello.txt action=delete");
        }

        [Fact]
        public void Test_Quotes_OptionNameAsValue_WithOptNames_OptKeyWordTreatedAsValue_B()
        {
            Test_Quotes_OptionNameAsValue_WithOptNames_OptKeyWordTreatedAsValue(@"list filename=hello.txt action delete");
        }

        private void Test_Quotes_OptionNameAsValue_WithOptNames_OptKeyWordTreatedAsValue(string input)
        {
            Processor p = new();

            p.LoadDefinitionsFromXML(this.xml);

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filename", 0, "hello.txt");
            AssertExpectCommandOptionValue(res, 0, "list", 1, "action", 0, "delete");
        }


        [Fact]
        public void Test_Quotes_OptionNameAsValue_WithOptNames_TreatAsValue_A()
        {
            Test_Quotes_OptionNameAsValue_WithOptNames_TreatAsValue(@"list 'filename'");
        }

        [Fact]
        public void Test_Quotes_OptionNameAsValue_WithOptNames_TreatAsValue_B()
        {
            Test_Quotes_OptionNameAsValue_WithOptNames_TreatAsValue("list \"filename\"");
        }

        private void Test_Quotes_OptionNameAsValue_WithOptNames_TreatAsValue(string input)
        {
            Processor p = new();

            p.LoadDefinitionsFromXML(this.xml);

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filename", 0, "filename");
        }


    }
}