using System;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// This exception is thrown when parsing the XML that defines available Commands and Options when a Options of the same name is defined more than once at the same level.
    /// </summary>
    public class DuplicateOptionException : ArgsException
    {
        /// <summary>
        /// The name of the Command the duplicated Option belongs to.  If the Option was defined outside of a Command, i.e. a Global Option, then the will be set to "".
        /// </summary>
        /// <value>The name of the Command the Option belongs to or "" if it is a Global Option.</value>
        public string CommandName { get; private set; }

        /// <summary>
        /// String holding the name of the duplicated Option.
        /// </summary>
        /// <value>The name of the duplicated Option.</value>

        public string OptionName { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.DuplicateOptionException"/> with specified Option and Command names.
        /// </summary>
        /// <param name="CommandName">Name of the Command where the Option was duplicated or "" if it was a Global Option.</param>
        /// <param name="OptionName">Name of the duplicated Option.</param>
        /// <example>
        /// This example will result in <see cref="DuplicateOptionException"/> being thrown by <see cref="Processor.LoadDefinitionsFromXML"/>.
        /// <code>
        /// <![CDATA[
        /// Processor p = new();
        /// p.LoadDefinitionsFromXML(@"
        ///     <xml>
        ///         <command name='list' is_default='true' allow_multiple='false'>
        ///             <option name='filespec' required='false' default_value='*.txt' allow_multiple='false' />
        ///         </command>
        ///         <option name='filespec' default_value='*.txt' allow_multiple='false' />
        ///         <option name='another' default_value='*.txt' allow_multiple='false' />
        ///         <option name='filespec' default_value='*.txt' allow_multiple='false' />
        ///     </xml>
        ///     ");
        /// ]]>
        /// </code>
        /// </example>
        public DuplicateOptionException(String CommandName, string OptionName)
        : base($"Multiple uses of {OptionName} not supported" + ((CommandName == "") ? "" : $" for command {CommandName}"))
        {
            this.CommandName = CommandName;
            this.OptionName = OptionName;
        }
    }
}