using System.Collections.Generic;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// A collection of IntermediateCommandResult objects. Inherits from List<![CDATA[<IntermediateCommandResult>]]>
    /// </summary>
    internal class IntermediateCommandResultCollection : List<IntermediateCommandResult>
    {
    }

    /// <summary>
    /// This class is used by <see cref="Parser"/> when processing user input (e.g. the command line arguments provided)
    /// 
    /// This class is internal to prevent external use to avoid issues in the future if the internal 
    /// logic is changed.
    /// </summary>
    internal class IntermediateCommandResult
    {
        public string? UnprocessedText { get; set; }


        public Command? Cmd { get; set; }

        public IntermediateCommandResult()
        {
            this.UnprocessedText = null;
            this.Cmd = null;
        }

        public static IntermediateCommandResult CreateAsUnProcessedText(string unprocessedText)
        {
            IntermediateCommandResult icr = new();
            icr.UnprocessedText = unprocessedText;
            return icr;
        }

        public static IntermediateCommandResult CreateAsCommand(Command cmd)
        {
            IntermediateCommandResult icr = new();
            icr.Cmd = cmd;
            return icr;
        }
    }


}
