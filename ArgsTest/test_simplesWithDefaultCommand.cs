using Xunit;

using Sprocket.Args;

namespace ArgsTest
{

    public class ArgsTest_Simples_With_Default_Command : test_base
    {

        [Fact]
        public void Test_Simple_WithDefCmd_NoReqOpt_NoDefVal_NoMulti_NoCmd_NoOpt_NoValue()
        {
            string input = string.Empty;

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            Command? cmd = AssertExpectCommand(res, 0, "list");
            if (cmd != null)
            {
                AssertExpectCommandWithNoOptions(res, cmd);
            }
        }

        [Fact]
        public void Test_Simple_WithDefCmd_NoReqOpt_NoDefVal_NoMulti_NoCmd_NoOpt_Value()
        {
            string input = @"*.txt";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
        }

        [Fact]
        public void Test_Simple_WithDefCmd_NoReqOpt_WithDefVal_NoMulti_NoCmd_NoOpt_NoValue()
        {
            string input = string.Empty;

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='false' default_value='*.csv' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.csv");
        }

        [Fact]
        private void Test_Simple_WithDefCmd_NoReqOpt_WithDefVal_NoMulti_NoCmd_NoOpt_Value()
        {
            string input = @"*.txt";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='false' default_value='*.csv' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
        }

        [Fact]
        private void Test_Simple_WithDefCmd_WithReqOpt_NoDefVal_NoMulti_NoCmd_NoOpt_NoValue()
        {
            string input = string.Empty;

            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='true' allow_multiple='false' />
                    </command>");

                ParseResult res = p.Parse(input);

                ThrowExceptionWasExpected(typeof(MissingValueException).ToString());
            }
            catch (System.Exception e)
            {
                AssertExpectedMissingValueException(e, "filespec");
            }
        }

        [Fact]
        private void Test_Simple_WithDefCmd_WithReqOpt_WithDefVal_NoMulti_NoCmd_NoOpt_NoValue()
        {
            string input = string.Empty;

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='true' default_value='*.csv' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.csv");
        }

        [Fact]
        public void Test_Simple_WithDefCmd_WithReqOpt_WithDefVal_NoMulti_NoCmd_NoOpt_Value()
        {
            string input = @"*.txt";

            Processor p = new();
            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='true' default_value='*.csv' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");

        }
 
    }
}
