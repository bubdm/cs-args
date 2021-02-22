using Xunit;

using Sprocket.Args;

namespace ArgsTest
{
    public class ArgsTest_Flags : test_base
    {
        [Fact]
        public void Test_Flag_Optional_NotSupplied()
        {
            string input = @"list";

            Processor p = new();
            p.LoadDefinitionsFromXML(@"
                    <command name='list'>
                        <option name='-all' is_flag='true' />
                    </command>
            ");

            ParseResult res = p.Parse(input);

            AssertExpectCommand(res, 0, "list");
            Assert.False(res.Commands[0].Options.Exists("-all"));

        }

        [Fact]
        public void Test_Flag_Invalid_Multi_Condition()
        {
            try
            {
                Processor p = new();
                p.LoadDefinitionsFromXML(@"
                    <command name='list'>
                        <option name='-all' is_flag='true' allow_multiple='true' />
                    </command>
                ");

                ThrowExceptionWasExpected(typeof(XMLProcessingException).ToString());
            }
            catch (System.Exception e)
            {
                AssertExpectedXMLProcessingException(e, "Flag fields do not support multiple values");
            }

        }


        [Fact]
        public void Test_Flag_Optional_Supplied()
        {
            string input = @"list -all";

            Processor p = new();
            p.LoadDefinitionsFromXML(@"
                    <command name='list'>
                        <option name='-all' is_flag='true' />
                    </command>
            ");

            ParseResult res = p.Parse(input);

            AssertExpectCommandOptionValue(res, 0, "list", 0, "-all", 0, "true");
        }

        [Fact]
        public void Test_Flag_Optional_Supplied_InvalidValueSupplied()
        {
            string input = @"list -all false";

            try
            {


                Processor p = new();
                p.LoadDefinitionsFromXML(@"
                    <command name='list'>
                        <option name='-all' is_flag='true' />
                    </command>
                ");

                ParseResult res = p.Parse(input);

                ThrowExceptionWasExpected(typeof(InvalidCommandException).ToString());
            }
            catch (System.Exception e)
            {
                //it's a flag field.  No value expected.  Any words after are expected to be command or option keywords
                AssertExpectInvalidCommandException(e, "false");
            }

        }

    }
}