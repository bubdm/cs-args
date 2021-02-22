using Xunit;

using Sprocket.Args;

namespace ArgsTest
{

    public class ArgsTest_MultipleOptions : test_base
    {
        [Fact]
        public void Test_MultiOpts4Cmd_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_NoOptName_With2Values()
        {
            string input = @"list *.txt *.map";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                        <option name='renameto' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
            AssertExpectCommandOptionValue(res, 0, "list", 1, "renameto", 0, "*.map");
        }

        [Fact]
        public void Test_MultiOpts4Cmd_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOptName_OutofOrder_With2Values()
        {
            string input = @"list *.txt filespec=*.map";

            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                        <option name='renameto' required='false' allow_multiple='false' />
                    </command>");

                ParseResult res = p.Parse(input);

                ThrowExceptionWasExpected(typeof(DuplicateOptionException).ToString());
            }
            catch (System.Exception e)
            {
                AssertExpectDuplicateOptionException(e, "Multiple uses of filespec not supported for command list");
            }
        }

        [Fact]
        public void Test_MultiOpts4Cmd_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOptName_InOrder_With2Values_A()
        {
            Test_MultiOpts4Cmd_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOptName_InOrder_With2Values(@"list filespec=*.txt *.map");
        }


        [Fact]
        public void Test_MultiOpts4Cmd_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOptName_InOrder_With2Values_B()
        {
            Test_MultiOpts4Cmd_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOptName_InOrder_With2Values(@"list *.txt renameto=*.map");
        }


        [Fact]
        public void Test_MultiOpts4Cmd_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOptName_InOrder_With2Values_C()
        {
            Test_MultiOpts4Cmd_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOptName_InOrder_With2Values(@"list filespec=*.txt renameto=*.map");
        }


        
        private void Test_MultiOpts4Cmd_NoDefCmd_NoReqOpt_NoDefVal_NoMulti_WithCmd_WithOptName_InOrder_With2Values(string input)
        {
            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='false' allow_multiple='false'>
                        <option name='filespec' required='false' allow_multiple='false' />
                        <option name='renameto' required='false' allow_multiple='false' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.txt");
            AssertExpectCommandOptionValue(res, 0, "list", 1, "renameto", 0, "*.map");
        }

        [Fact]
        public void Test_Simples_Repeated_Interlaced_MultipleOptions()
        {
            string input = @"list name=adam name=alice person=one name=eve person=two";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='name' required='false' allow_multiple='true' />
                        <option name='person' required='false' allow_multiple='true' />
                    </command>");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "name", 0, "adam");
            AssertExpectCommandOptionValue(res, 0, "list", 0, "name", 1, "alice");
            AssertExpectCommandOptionValue(res, 0, "list", 0, "name", 2, "eve");
            AssertExpectCommandOptionValue(res, 0, "list", 1, "person", 0, "one");
            AssertExpectCommandOptionValue(res, 0, "list", 1, "person", 1, "two");
        }
    }
}