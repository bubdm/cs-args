using Xunit;

using Sprocket.Args;

namespace ArgsTest
{

    public class ArgsTest_Simples_No_Default_Command : test_base
    {
        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_NoCmd_NoOpt_NoValue()
        {
            string input = string.Empty;

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            Assert.NotNull(res.Commands);
            AssertExpectNoCommand(res);
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_NoCmd_NoOpt_Value()
        {
            string input = @"*.txt";

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
                AssertExpectInvalidCommandException(e, "*.txt");
            }
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_NoCmd_WithOpt_NoValue()
        {
            string input = @"filespec";

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
                AssertExpectInvalidCommandException(e, "filespec");
            }
        }


        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_NoValue()
        {
            string input = @"list";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
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
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_With1Value()
        {
            string input = @"list *.txt";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);


            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_With2Values()
        {
            string input = @"list *.txt *.map";

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
                AssertExpectInvalidCommandException(e, "*.map");
            }
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_NoValue()
        {
            string input = @"list filespec";

            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false'/>
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
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_With1Value()
        {
            string input = @"list filespec *.txt";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_With2Values()
        {
            string input = @"list filespec *.txt *.map";

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
                AssertExpectInvalidCommandException(e, "*.map");
            }
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_WithMulti_WithCmd_WithOpt_With2Values()
        {
            string input = @"list filespec *.txt *.map";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='true' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 1, "*.map");
        }

[Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_WithMulti_WithCmd_WithOpt_With2Values_WithFlagOpt()
        {
            string input = @"list --silent filespec *.txt *.map";

            Processor p = new();

                p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='true' />
                        <option name='--silent' is_flag='true' />
                    </command>");

                ParseResult res = p.Parse(input);
                
                AssertExpectCommandOptionValue(res, 0, "list", 0, "--silent", 0, "true");
                AssertExpectCommandOptionValue(res, 0, "list", 1, "filespec", 0, "*.txt");
                    AssertExpectCommandOptionValue(res, 0, "list", 1, "filespec", 1, "*.map");
        }
    }
}