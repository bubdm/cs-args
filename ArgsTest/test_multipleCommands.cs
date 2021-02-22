using Xunit;

using Sprocket.Args;

namespace ArgsTest
{
    public class ArgsTest_MultipleCommands : test_base
    {
        [Fact]
        public void Test_MultiCmd_Allowed_NoOptsSupplied()
        {
            string input = @"list list";

            Processor p = new();

            string xml = @"
                    <command name='list' is_default='true' allow_multiple='true'>
                        <option name='filename' required='false' allow_multiple='false' />
                        <option name='action' required='false' allow_multiple='false' />
                        <option name='delete' required='false' allow_multiple='false' />
                    </command>";

            p.LoadDefinitionsFromXML(xml);

            ParseResult res = p.Parse(input);

            Command? cmd1 = AssertExpectCommand(res, 0, "list");
            if (cmd1 != null)
            {
                AssertExpectCommandWithNoOptions(res, cmd1);

                Command? cmd2 = AssertExpectCommand(res, 1, "list");
                if (cmd2 != null)
                {
                    AssertExpectCommandWithNoOptions(res, cmd2);
                }
            }
        }

        [Fact]
        public void Test_MultiCmd_Allowed_WithOptsSupplied()
        {
            string input = @"list filename=a.txt list filename=b";

            Processor p = new();

            string xml = @"
                    <command name='list' is_default='true' allow_multiple='true'>
                        <option name='filename' required='false' allow_multiple='false' />
                        <option name='action' required='false' allow_multiple='false' />
                        <option name='delete' required='false' allow_multiple='false' />
                    </command>";

            p.LoadDefinitionsFromXML(xml);

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filename", 0, "a.txt");
            AssertExpectCommandOptionValue(res, 1, "list", 0, "filename", 0, "b");
        }

        [Fact]
        public void Test_MultiCmd_NotAllowed_2ndCmdTakenAsOptionValue()
        {
            string input = @"list list";

            Processor p = new();

            string xml = @"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filename' required='false' allow_multiple='false' />
                    </command>";

            p.LoadDefinitionsFromXML(xml);

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filename", 0, "list");
        }

        [Fact]
        public void Test_MultiCmd_NotAllowed_2ndCmdNotAccepted_A()
        {
            Test_MultiCmd_NotAllowed_2ndCmdNotAccepted(@"list myfilename list");
        }

        [Fact]
        public void Test_MultiCmd_NotAllowed_2ndCmdNotAccepted_B()
        {
            Test_MultiCmd_NotAllowed_2ndCmdNotAccepted(@"list filename=a.txt list filename=b");
        }

        private void Test_MultiCmd_NotAllowed_2ndCmdNotAccepted(string input)
        {
            Processor p = new();

            try
            {
                string xml = @"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filename' required='false' allow_multiple='false' />
                    </command>";

                p.LoadDefinitionsFromXML(xml);

                ParseResult res = p.Parse(input);

                ThrowExceptionWasExpected(typeof(DuplicateCommandException).ToString());

            }
            catch (System.Exception e)
            {
                AssertExpectedDuplicateCommandException(e, "Multiple uses of list not supported");
            }
        }

    }
}