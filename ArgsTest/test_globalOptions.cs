using Xunit;

using Sprocket.Args;

namespace ArgsTest
{
    public class ArgsTest_GlobalOptions : test_base
    {
        [Fact]
        public void Test_Global_NoCmd_NoDefault_WithGlobal_NoValue()
        {
            string input = @"--repeat";

            try
            {
                Processor p = new();
                p.LoadDefinitionsFromXML(@"
                        <option name='--repeat' />
                    ");

                ParseResult res = p.Parse(input);

                ThrowExceptionWasExpected(typeof(MissingValueException).ToString());

            }
            catch (System.Exception e)
            {
                AssertExpectedMissingValueException(e, "for option '--repeat'");
            }
        }

        [Fact]
        public void Test_Global_NoCmd_NoDefault_WithGlobal_WithValue()
        {

            string input = @"--repeat off";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                        <option name='--repeat' />
                    ");

            ParseResult res = p.Parse(input);

            AssertExpectNoCommand(res);
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "off");
        }

        [Fact]
        public void Test_Global_NoCmd_WithOptDefault_WithGlobal_NoValue()
        {
            string input = @"--repeat";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                        <option name='--repeat' default_value='always' />
                    ");

            ParseResult res = p.Parse(input);

            AssertExpectNoCommand(res);
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "always");
        }

        [Fact]
        public void Test_Global_NoCmd_NoDefault_With2Global_No1Value_With2Value()
        {
            string input = @"--repeat --link all";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list'>
                                <option name='filespec' required='true' default_value='*.zip' allow_multiple='false' />
                            </command>
                            <option name='--repeat' />
                            <option name='--link' default_value='*.pop' />
                        </xml>
                    ");

            ParseResult res = p.Parse(input);

            AssertExpectNoCommand(res);
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "");
            AssertExpectGlobalOptionValue(res, 1, "--link", 0, "all");
        }

        [Fact]
        public void Test_Global_NoCmd_NoDefault_With2Global_With1Value_With2Value()
        {
            string input = @"--repeat never --link all";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list'>
                                <option name='filespec' required='true' default_value='*.zip' allow_multiple='false' />
                            </command>
                            <option name='--repeat' />
                            <option name='--link' default_value='*.pop' />
                        </xml>
                    ");

            ParseResult res = p.Parse(input);

            AssertExpectNoCommand(res);
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "never");
            AssertExpectGlobalOptionValue(res, 1, "--link", 0, "all");
        }

        [Fact]
        public void Test_Global_WithCmd_NoRequiredOpt_NoDefault_CmdOptIncomplete()
        {
            string input = @"list --repeat on";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list'>
                                <option name='filespec' required='false' allow_multiple='false' />
                            </command>
                            <option name='--repeat' />
                        </xml>
                    ");

            ParseResult res = p.Parse(input);

            var cmd = AssertExpectCommand(res, 0, "list");
            if (cmd != null)
            {
                AssertExpectCommandWithNoOptions(res, cmd);
                AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "on");
            }
        }

        [Fact]
        public void Test_Global_WithCmd_WithRequiredOpt_NoDefault_CmdOptIncomplete()
        {
            string input = @"list --repeat on";

            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list'>
                                <option name='filespec' required='true' allow_multiple='false' />
                            </command>
                            <option name='--repeat' />
                        </xml>
                    ");

                ParseResult res = p.Parse(input);

                ThrowExceptionWasExpected(typeof(MissingValueException).ToString());

            }
            catch (System.Exception e)
            {
                AssertExpectedMissingValueException(e, "No value specified for option 'filespec'");
            }
        }

        [Fact]
        public void Test_Global_WithCmd_WithRequiredOpt_WithOptDefault_CmdOptIncomplete_A()
        {
            Test_Global_WithCmd_WithRequiredOpt_WithOptDefault_CmdOptIncomplete(@"list --repeat on");
        }

        [Fact]
        public void Test_Global_WithCmd_WithRequiredOpt_WithOptDefault_CmdOptIncomplete_B()
        {
            Test_Global_WithCmd_WithRequiredOpt_WithOptDefault_CmdOptIncomplete(@"list filespec --repeat on");
        }

        private void Test_Global_WithCmd_WithRequiredOpt_WithOptDefault_CmdOptIncomplete(string input)
        {
            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list'>
                                <option name='filespec' required='true' default_value='*.zip' allow_multiple='false' />
                            </command>
                            <option name='--repeat' />
                        </xml>
                    ");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.zip");
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "on");

        }

        [Fact]
        public void Test_Global_WithCmd_NoRequiredOpt_MultiplesAllowed()
        {
            string input = @"list --repeat on --repeat off";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list'>
                                <option name='filespec' required='true' default_value='*.zip' allow_multiple='false' />
                            </command>
                            <option name='--repeat' allow_multiple='true'/>
                        </xml>
                    ");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.zip");
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "on");
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 1, "off");
        }

        [Fact]
        public void Test_Global_WithCmd_NoRequiredOpt_MultiplesNotAllowed()
        {
            string input = @"list --repeat on --repeat off";

            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list'>
                                <option name='filespec' required='true' default_value='*.zip' allow_multiple='false' />
                            </command>
                            <option name='--repeat' />
                        </xml>
                    ");

                ParseResult res = p.Parse(input);

                ThrowExceptionWasExpected(typeof(DuplicateOptionException).ToString());
            }
            catch (System.Exception e)
            {
                AssertExpectDuplicateOptionException(e, "Multiple uses of --repeat not");
            }
        }

        [Fact]
        public void Test_Global_WithCmd_OptNoDefault_IsFlag_NoValueExpected()
        {
            string input = @"list --repeat";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list'  allow_multiple='false'>
                                <option name='filespec' required='true' default_value='*.zip' />
                            </command>
                            <option name='--repeat' is_flag='true' />
                            <option name='--link' default_value='*.pop' />
                        </xml>
                    ");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.zip");
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "true");
        }

        [Fact]
        public void Test_Global_WithCmd_OptWithDefault_IsFlag_NoValueExpected()
        {
            string input = @"list --repeat";

            Processor p = new();
            p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list'  allow_multiple='false'>
                                <option name='filespec' required='true' default_value='*.zip' />
                            </command>
                            <option name='--repeat' is_flag='true' default_value='on' />
                            <option name='--link' default_value='*.pop' />
                        </xml>
                    ");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.zip");
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "on");
        }

        [Fact]
        public void Test_Global_WithCmd_MultiCmdAllowed_OptNoDefault_IsFlag_NoValueExpected()
        {
            string input = @"list --repeat list";

            Processor p = new();

            p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list' allow_multiple='true'>
                                <option name='filespec' required='true' default_value='*.zip' />
                            </command>
                            <option name='--repeat' is_flag='true' />
                            <option name='--link' default_value='*.pop' />
                        </xml>
                    ");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.zip");
            AssertExpectCommandOptionValue(res, 1, "list", 0, "filespec", 0, "*.zip");
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "true");
        }

        [Fact]
        public void Test_Global_WithCmd_MultiCmdAllowed_WithDefault_IsFlag_NoValueExpected()
        {
            string input = @"list --repeat list";

            Processor p = new();
            p.LoadDefinitionsFromXML(@"
                        <xml>
                            <command name='list' allow_multiple='true'>
                                <option name='filespec' required='true' default_value='*.zip' />
                            </command>
                            <option name='--repeat' is_flag='true' default_value='on' />
                            <option name='--link' default_value='*.pop' />
                        </xml>
                    ");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "filespec", 0, "*.zip");
            AssertExpectCommandOptionValue(res, 1, "list", 0, "filespec", 0, "*.zip");
            AssertExpectGlobalOptionValue(res, 0, "--repeat", 0, "on");
        }



    }


}
