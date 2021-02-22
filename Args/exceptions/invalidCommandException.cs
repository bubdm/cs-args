using System;

#nullable enable

namespace Sprocket.Args
{

    /// <summary>
    /// This exception is thrown when parsing the command line if a Command is unknown.
    /// </summary>
    public class InvalidCommandException : ArgsException
    {
        /// <summary>
        /// String holding the word encountered that could not be matched to a Command.
        /// </summary>
        /// <value>The unknown word found.</value>
        public string CommandName { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.InvalidCommandException"/> with specified Command name.
        /// </summary>
        /// <param name="commandName">The unknown Command name.</param>
        /// <example>
        /// This example will result in <see cref="InvalidCommandException"/> being thrown by <see cref="Processor.Parse"/> due to the word "delete" not
        /// being a known command. The Command "list" is not set as a default nor does the option "filespec" allow multiple values.  The word "delete"
        /// therefore cannot be assigned to anything and an excaption is raised.
        /// <code>
        /// <![CDATA[
        /// string input = @"list *.txt delete *.map";
        /// Processor p = new();
        /// p.LoadDefinitionsFromXML(@"
        ///                 <command name='list' is_default='false' allow_multiple='false'>
        ///                     <option name='filespec' required='false' allow_multiple='false' />
        ///                 </command>");
        /// ParseResult res = p.Parse(input);
        /// ]]>
        /// </code>
        /// </example>
        public InvalidCommandException(String commandName)
        : base($"Invalid command '{commandName}'")
        {
            this.CommandName = commandName;
        }
    }
}
