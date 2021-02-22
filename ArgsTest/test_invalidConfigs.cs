using Xunit;

using Sprocket.Args;

namespace ArgsTest
{
    public class ArgsTest_InvalidConfigs : test_base
    {

        [Fact]
        public void Test_InvalidXML()
        {
            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <xml>
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
                    </xml>
            ");

                ThrowExceptionWasExpected(typeof(XMLProcessingException).ToString());

            }
            catch (System.Exception e)
            {
                AssertExpectedXMLProcessingException(e, "does not match the end tag");
            }
        }

        [Fact]
        public void Test_MultipleTopLevelNodes()
        {
            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <command name='list' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
                    </command>
                    <command name='other' is_default='true' allow_multiple='false'>
                        <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
                    </command>
            ");

                ThrowExceptionWasExpected(typeof(XMLProcessingException).ToString());
            }
            catch (System.Exception e)
            {
                AssertExpectedXMLProcessingException(e, "There are multiple root elements");
            }
        }


        [Fact]
        public void Test_DuplicateCommandNames()
        {
            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <xml>
                        <command name='list' is_default='true' allow_multiple='false'>
                            <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
                        </command>
                        <command name='list' is_default='false' allow_multiple='false' />
                    </xml>
            ");

                ThrowExceptionWasExpected(typeof(XMLProcessingException).ToString());

            }
            catch (System.Exception e)
            {
                AssertExpectedXMLProcessingException(e, "An item with the same key has already been added");
            }
        }

        [Fact]
        public void Test_DuplicateGlobalOptionNames()
        {
            Processor p = new();

            try
            {

                p.LoadDefinitionsFromXML(@"
                    <xml>
                        <command name='list' is_default='true' allow_multiple='false'>
                            <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
                        </command>
                        <option name='filespec' default_value='*.txt' allow_multiple='false' />
                        <option name='another' default_value='*.txt' allow_multiple='false' />
                        <option name='filespec' default_value='*.txt' allow_multiple='false' />
                    </xml>
                    ");

                ThrowExceptionWasExpected(typeof(XMLProcessingException).ToString());

            }
            catch (System.Exception e)
            {

                AssertExpectedXMLProcessingException(e, "An item with the same key has already been added");
            }
        }

        [Fact]
        public void Test_GlobalOptionWithSameNameAsACommand()
        {
            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <xml>
                        <command name='list' is_default='true' allow_multiple='false'>
                            <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
                        </command>
                        <option name='filespec' default_value='*.txt' allow_multiple='false' />
                        <option name='list' default_value='*.txt' allow_multiple='false' />
                    </xml>
                    ");

                ThrowExceptionWasExpected(typeof(XMLProcessingException).ToString());

            }
            catch (System.Exception e)
            {
                AssertExpectedXMLProcessingException(e, "A Command and Global Option have the same name");
            }
        }

        [Fact]
        public void Test_GlobalOptionWithRequiredSet()
        {
            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <xml>
                        <command name='list' is_default='true' allow_multiple='false'>
                            <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
                        </command>
                        <option name='filespec' default_value='*.txt' allow_multiple='false' />
                        <option name='revise' required='true' default_value='*.txt' allow_multiple='false' />
                    </xml>
                    ");

                ThrowExceptionWasExpected(typeof(XMLProcessingException).ToString());
            }
            catch (System.Exception e)
            {
                AssertExpectedXMLProcessingException(e, "Global Option 'revise' Cannot Be Flagged As Required");
            }
        }

        [Fact]
        public void Test_MultipleDefaultCommands()
        {
            Processor p = new();

            try
            {
                p.LoadDefinitionsFromXML(@"
                    <xml>
                        <command name='list' is_default='true' allow_multiple='false'>
                            <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
                        </command>
                        <command name='clone' is_default='true' allow_multiple='false'>
                            <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
                        </command>
                    </xml>
                ");

                ThrowExceptionWasExpected(typeof(XMLProcessingException).ToString());

            }
            catch (System.Exception e)
            {
                AssertExpectedXMLProcessingException(e, "Multiple default commands are not allowed");
            }
        }

    }
}