using Xunit;

using Sprocket.Args;

namespace ArgsTest
{
    public class ArgsTest_IncompleteInput : test_base
    {
        [Fact]
        private void Test_IncompleteMandatoryOption_1cmd_1opt()
        {
            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='true' allow_multiple='false' />
                    </command>");

                ParseResult? res = p.Parse(string.Empty);

                ThrowExceptionWasExpected(typeof(MissingValueException).ToString());

            }
            catch (System.Exception e)
            {
                AssertExpectedMissingValueException(e, "filespec");
            }
        }

        [Fact]
        private void Test_IncompleteMandatoryOption_1cmd_2opt()
        {
            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='true' allow_multiple='false' />
                        <option name='color' required='true' allow_multiple='false' />
                    </command>");

                ParseResult? res = p.Parse("*.txt");

                ThrowExceptionWasExpected(typeof(MissingValueException).ToString());

            }
            catch (System.Exception e)
            {
                AssertExpectedMissingValueException(e, "color");
            }
        }

        [Fact]
        private void Test_IncompleteMandatoryOption_2cmd_2ndIncomplete()
        {
            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <xml>
                        <command name='list' is_default='true' allow_multiple='false'>
                            <option name='filespec' required='false' allow_multiple='false' />
                        </command>
                        <command name='remove' is_default='false' allow_multiple='false'>
                            <option name='filename' required='true' allow_multiple='false' />
                        </command>
                    </xml>
                    ");

                ParseResult? res = p.Parse("*.txt remove");

                ThrowExceptionWasExpected(typeof(MissingValueException).ToString());

            }
            catch (System.Exception e)
            {
                AssertExpectedMissingValueException(e, "filename");
            }
        }

    }
}
