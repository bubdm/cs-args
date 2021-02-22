using System.Text.Json;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// Holds the results of parsing as returned by <see cref="Processor"/>.<see cref="Processor.Parse"/> when matching against 
    /// the Command and Options specification previously loaded by <see cref="Processor"/>.<see cref="Processor.LoadDefinitionsFromXML"/>.
    /// </summary>
    public class ParseResult
    {

        /// <summary>
        /// A collection of parsed Commands in order as they were in the parsed string.  If no Commands were found this collection will be empty.
        /// </summary>
        /// <value>Collection of Commands found.</value>
        public CommandCollection Commands { get; private set; }

        /// <summary>
        /// A collection of Global Options in order as there were in the parsed string.  If no Global Options are were found this collection will be empty.
        /// </summary>
        /// <value></value>
        public OptionCollection GlobalOptions { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ParseResult"/> with the specified Command Collection and Global Options.
        /// </summary>
        /// <param name="commands">A collection of parsed Commands.</param>
        /// <param name="globalOptions">A collection of parsed Global Options.</param>
        /// <remarks>This is internal and not for direct consumption outside of this assembly.</remarks>
        internal ParseResult(CommandCollection commands, OptionCollection globalOptions)
        {
            this.Commands = commands;
            this.GlobalOptions = globalOptions;
        }

        /// <summary>
        /// Serialize this collection to Json.  Uses .NET's System.Text.Json.
        /// </summary>
        /// <returns>Json formatted string.</returns>
        /// <param name="writeIndented">Set to true to return indented text.</param>
        /// <returns>Json formatted string.</returns>
        public string ToJson(bool writeIndented = false)
        {
            JsonSerializerOptions opts = new();
            opts.WriteIndented = writeIndented;
            return this.ToJson(opts);
        }

        /// <summary>
        /// Serialize this collection to Json.  Uses .NET's System.Text.Json.
        /// </summary>
        /// <param name="opts">Options to provide to the Json Serializer.</param>
        /// <returns>Json formatted string.</returns>
        public string ToJson(JsonSerializerOptions opts)
        {
            return JsonSerializer.Serialize(this, opts);
        }
    }
}
