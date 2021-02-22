using System;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// This exception is thrown when parsing the XML that defines available Commands and Options when a Command of the same name is defined more than once.
    /// </summary>
    public class DuplicateCommandException : ArgsException
    {
        /// <summary>
        /// String holding the name of the duplicated Command.
        /// </summary>
        /// <value>The name of the duplicated Command.</value>
        public string CommandName { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.DuplicateCommandException"/> with a specified Command name.
        /// </summary>
        /// <param name="CommandName">Name of the duplicated Command.</param>
        /// <example>
        /// This example will result in <see cref="DuplicateCommandException"/> being thrown by <see cref="Processor.LoadDefinitionsFromXML"/>.
        /// <code>
        /// <![CDATA[
        /// Processor p = new();
        /// p.LoadDefinitionsFromXML(@"
        ///         <xml>
        ///             <command name='list' is_default='true' allow_multiple='false'>
        ///                 <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
        ///            </command>
        ///                <command name='list' is_default='true' allow_multiple='false' />
        ///         </xml>
        ///     ");
        /// ]]>
        /// </code>
        /// </example>
        public DuplicateCommandException(String CommandName)
        : base($"Multiple uses of {CommandName} not supported")
        {
            this.CommandName = CommandName;
        }
    }
}
