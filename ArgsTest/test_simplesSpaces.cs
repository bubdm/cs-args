using Xunit;

using Sprocket.Args;

namespace ArgsTest
{

    public class ArgsTest_Simples_Spaces : test_base
    {
        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_With1Value_WithEqual()
        {
            string input = @"list=*.txt";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_With1Value_WithEqualSpaced_A()
        {
            Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_With1Value_WithEqualSpaced(@"list = *.txt");
        }
        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_With1Value_WithEqualSpaced_B()
        {
            Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_With1Value_WithEqualSpaced(@"list =*.txt");
        }
        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_With1Value_WithEqualSpaced_C()
        {
            Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_With1Value_WithEqualSpaced(@"list= *.txt");
        }
        private void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOpt_With1Value_WithEqualSpaced(string input)
        {
            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_With1Value_WithEqual()
        {
            string input = @"list filespec=*.txt";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_With1Value_WithEqualSpaced_A()
        {
            Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_With1Value_WithEqualSpaced(@"list filespec=*.txt");
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_With1Value_WithEqualSpaced_B()
        {
            Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_With1Value_WithEqualSpaced(@"list filespec = *.txt");
        }

        [Fact]
        public void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_With1Value_WithEqualSpaced_C()
        {
            Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_With1Value_WithEqualSpaced(@"list filespec= *.txt");
        }


        private void Test_Simple_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOpt_With1Value_WithEqualSpaced(string input)
        {
            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
        }


    }
}