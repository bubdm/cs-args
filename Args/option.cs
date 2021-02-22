using System.Collections.Generic;
using System.Text.Json;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// The Option object holds details of a parsed Option with the default value or the value that was parsed.
    /// An Option may have one or more (<see cref="Option.Values"/>).
    /// </summary>
    public class Option
    {
        /// <summary>
        /// The name of the Option. The capitalization of the Name is the same as specified in the XML Definition regardless of the 
        /// casing of the string presented to <see cref="Processor.Parse"/>.
        /// </summary>
        /// <value>Name of the Option.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Returns a <![CDATA[List<string>]]> of all values assigned to this Option.  Options can support multiple values (allow_multiple XML attribute).  
        /// If the option is only expected to have one value, or you wish to access its first value, you can use the <see cref="Value"/> property.
        /// </summary>
        /// <remarks>
        /// All Options have a value. However, flag options (is_flag XML attribute) if parsed will always have the value of "true" (string).
        /// </remarks>
        /// <example>
        /// The following example shows a definition where <i>'-all'</i> is an available option for the <i>'list'</i> command.  The user has supplied the command line
        /// arguments <i>"list -all"</i>.  No value is expected or required after <i>'-all'</i>.
        /// The expected output from this example is a string with the value of "true".
        /// <code>
        /// string input = @"list -all";
        /// Processor p = new();
        /// p.LoadDefinitionsFromXML(@"<![CDATA[<command name='list'>]]>
        ///             <![CDATA[<option name='-all' is_flag='true' />]]>
        ///             <![CDATA[</command>]]>
        /// ");
        /// ParseResult res = p.Parse(input);
        /// string? s = res.Commands["list"]?.Options["-all"]?.Value;
        /// Console.WriteLine(s);
        /// </code>
        /// </example>
        public List<string> Values { get; private set; }


        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.Option"/> with the specified name and value.
        /// </summary>
        /// <remarks>This is internal and not for direct consumption outside of this assembly.</remarks>
        /// <param name="name">Name of the option.</param>
        /// <param name="value">Value to assign to the option.</param>
        internal Option(string name, string value)
        {
            this.Name = name;
            this.Values = new();
            this.Values.Add(value);
        }

        /// <summary>
        /// Returns the first value of the option.  Options can support multiple values (allow_multiple XML attribute).  If multiple values exist this property returns the first.
        /// </summary>
        /// <value>Value of the first value assigned to this option.</value>
        public string Value
        {
            get
            {
                return this.Values[0];
            }
        }
    }
}