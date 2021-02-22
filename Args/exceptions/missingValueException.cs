using System;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// This exception is thown when parsing the command line if a mandatory value has not been supplied for an Option.
    /// </summary>
    public class MissingValueException : ArgsException
    {
        /// <summary>
        /// The name of the Command the Option belongs to.  If it is a Global Option then this will be set to "".
        /// </summary>
        /// <value>Name of the Command or "" if this is a Global Option</value>
        public string CommandName { get; private set; }

        /// <summary>
        /// The name of the Option a value has not been supplied for
        /// </summary>
        /// <value>The Option's name.</value>
        public string OptionName { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.MissingValueException"/> with specified Command and Option names.
        /// </summary>
        /// <param name="commandName">The name of the Command or "" for Global Options..</param>
        /// <param name="optionName">The name of the Option.</param>
        /// <example>
        /// This example will result in <see cref="MissingValueException"/> being thrown by <see cref="Processor.Parse"/> as the command "remove"
        /// has a mandatory option "filename" but no value has been supplied after the word "remove".
        /// <code>
        /// <![CDATA[
        /// Processor p = new();
        /// p.LoadDefinitionsFromXML(@"
        ///             <xml>
        ///                 <command name='remove' is_default='false' allow_multiple='false'>
        ///                     <option name='filename' required='true' allow_multiple='false' />
        ///                 </command>
        ///             </xml>
        ///          ");
        /// ParseResult? res = p.Parse("*remove");
        /// ]]>
        /// </code>
        /// </example>

        public MissingValueException(string commandName, string optionName)
                : base($"No value specified for option '{optionName}'" + ((!commandName.Equals("##global", StringComparison.InvariantCultureIgnoreCase) ? $" for command '{commandName}'" : "")))
        {
            this.CommandName = commandName;
            this.OptionName = optionName;
        }
    }
}
