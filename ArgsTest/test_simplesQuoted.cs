using Xunit;

using Sprocket.Args;

namespace ArgsTest
{

    public class ArgsTest_Simples_Quoted : test_base
    {

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_NoCmd_NoOpt_With1Value_WithQuotes()
        {
            string input = "\"*.csv list filesspec *.map\"";

            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

                ParseResult res = p.Parse(input);

                ThrowExceptionWasExpected(typeof(InvalidCommandException).ToString());
            }
            catch (System.Exception e)
            {
                AssertExpectInvalidCommandException(e, "*.csv list filesspec *.map");
            }
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_NoCmd_WithOpt_With1Value_WithQuotes()
        {
            string input = "list \"all filesspec.*";
        
            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "all filesspec.*");
        }

        [Fact]
        public void Test_Simple_DefCmd_NoReqOpt_NoDefVal_NoMulti_NoCmd_NoOpt_With1Value_WithQuotes()
        {
            string input = "\"*.csv list filesspec *.map\"";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.csv list filesspec *.map");
        }

        [Fact]
        public void Test_Simple_DefCmd_NoReqOpt_NoDefVal_WithMulti_NoCmd_NoOpt_With2Values_WithQuotes()
        {
            string input = "\"*.csv list filesspec *.map\" *.txt";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='true' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.csv list filesspec *.map");
            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 1, "*.txt");
        }

        [Fact]
        public void Test_Simple_DefCmd_NoReqOpt_NoDefVal_WithMulti_NoCmd_NoOpt_With2Values_WithQuotesInQuotes()
        {
            string input = "\"*.csv 'list' filesspec *.map\" *.txt";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='true' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.csv 'list' filesspec *.map");
            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 1, "*.txt");
        }
    }
}