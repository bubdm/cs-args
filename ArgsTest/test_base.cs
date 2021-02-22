using System;
using Xunit;
using Xunit.Sdk;

using Sprocket.Args;

namespace ArgsTest
{

    public class test_base
    {
        public bool ThrowExceptionWasExpected(string expectedExceptionType)
        {
            throw new XunitException($"No exception was raised.  Expected exception of type {expectedExceptionType}");
        }


        public void AssertExpectedMissingValueException(System.Exception e, string containingText)
        {

            if (e is MissingValueException)
            {
                string msg = ((MissingValueException)e).Message;

                if (!msg.Contains(containingText, StringComparison.OrdinalIgnoreCase))
                {
                    throw new AssertActualExpectedException(@$"ArgsMissingValueException containing text ""{containingText}""",
                    msg, "ArgsMissingValueException Contaning Text");
                }
            }
            else
            {
                throw e;
            }
        }

        public void AssertExpectedXMLProcessingException(System.Exception e, string containingText)
        {

            if (e is XMLProcessingException)
            {
                string msg = ((XMLProcessingException)e).Message;

                if (!msg.Contains(containingText, StringComparison.OrdinalIgnoreCase))
                {
                    throw new AssertActualExpectedException(@$"ArgsXMLProcessingException containing text ""{containingText}""",
                    msg, "ArgsXMLProcessingException Contaning Text");
                }
            }
            else
            {
                throw e;
            }
        }

        
        public void AssertExpectedDuplicateCommandException(System.Exception e, string containingText)
        {

            if (e is DuplicateCommandException)
            {
                string msg = ((DuplicateCommandException)e).Message;

                if (!msg.Contains(containingText, StringComparison.OrdinalIgnoreCase))
                {
                    throw new AssertActualExpectedException(@$"ArgsDuplicateCommandException containing text ""{containingText}""",
                    msg, "ArgsDuplicateCommandException Contaning Text");
                }
            }
            else
            {
                throw e;
            }
        }


        public void AssertExpectDuplicateOptionException(System.Exception e, string containingText)
        {

            if (e is DuplicateOptionException)
            {
                string msg = ((DuplicateOptionException)e).Message;

                if (!msg.Contains(containingText, StringComparison.OrdinalIgnoreCase))
                {
                    throw new AssertActualExpectedException(@$"ArgsDuplicateOptionException containing text ""{containingText}""",
                    msg, "ArgsDuplicateOptionException Contaning Text");
                }
            }
            else
            {
                throw e;
            }
        }

        public void AssertExpectInvalidCommandException(System.Exception e, string containingText)
        {

            if (e is InvalidCommandException)
            {
                string msg = ((InvalidCommandException)e).Message;

                if (!msg.Contains(containingText, StringComparison.OrdinalIgnoreCase))
                {
                    throw new AssertActualExpectedException(@$"ArgsInvalidCommandException containing text ""{containingText}""",
                    msg, "ArgsInvalidCommandException Contaning Text");
                }
            }
            else
            {
                throw e;
            }
        }

        public void AssertExpectNoCommand(ParseResult res)
        {
            Assert.Empty(res.Commands);
        }

        public void AssertExpectGlobalOptionValue(ParseResult res, int optIdx, string withOptionName, int valueIdx, string withValue)
        {
            Assert.True(optIdx < res.GlobalOptions.Count);
            Assert.Equal(withOptionName, res.GlobalOptions[optIdx].Name);

            Option opt = res.GlobalOptions[optIdx];

            Assert.True(valueIdx < opt.Values.Count);
            Assert.Equal(withValue, opt.Values[valueIdx], true);

            //check Value [first value] when accessed from Option.Value
            if (optIdx == 0 && valueIdx == 0)
            {
                Assert.Equal(withValue, opt.Value, true);
            }
        }

        public void AssertExpectCommandOptionValue(ParseResult res, int cmdIndex, string withCommandName, int optIdx, string withOptionName, int valueIdx, string withValue)
        {
            Command? cmd = this.AssertExpectCommand(res, cmdIndex, withCommandName);

            if (cmd != null)
            {
                Assert.True(optIdx < cmd.Options.Count);

                if (optIdx < cmd.Options.Count)
                {
                    Assert.Equal(withOptionName, cmd.Options[optIdx].Name, true);
                    if (cmd.Options[optIdx].Name.Equals(withOptionName, StringComparison.OrdinalIgnoreCase))
                    {
                        Option opt = cmd.Options[optIdx];

                        Assert.True(valueIdx < opt.Values.Count);

                        if (valueIdx < opt.Values.Count)
                        {
                            // check the value via the 1st found option
                            Assert.Equal(withValue, opt.Values[valueIdx], true);

                            if (optIdx == 0 && valueIdx == 0)
                            {
                                //check Value [of first option's first value] when accessed from Command.Value
                                Assert.Equal(withValue,cmd.Value,true);

                                //check Value [first value] when accessed from Option.Value
                                Assert.Equal(withValue,opt.Value,true);
                            }
                        }
                    }

                }
            }
        }


        public Command? AssertExpectCommand(ParseResult res, int atIndex, string withName)
        {
            Assert.False(res.Commands == null);

            if (res.Commands != null)
            {
                Assert.Equal(withName, res.Commands[atIndex].Name, true);
                Assert.True(res.Commands.Count > atIndex);
                if (res.Commands.Count > atIndex && res.Commands[atIndex].Name.Equals(withName, StringComparison.OrdinalIgnoreCase))
                {
                    return res.Commands[atIndex];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        public void AssertExpectCommandWithNoOptions(ParseResult res, Command cmd)
        {
            Assert.Empty(cmd.Options);
        }
    }
}