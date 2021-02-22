#nullable enable

namespace Sprocket.Args
{


    /// <summary>
    /// The Command object holds details of a parsed Command.  A Command may contain Options (<see cref="OptionCollection"/>) which may have one or more Values (<see cref="Option.Values"/>).
    /// </summary>
    public class Command
    {
        /// <summary>
        /// The name of the Command. The capitalization of the Name is the same as specified in the XML Definition regardless of the 
        /// casing of the string presented to <see cref="Processor.Parse"/>.
        /// </summary>
        /// <value>Name of the Command.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Collection of parsed Options belonging to this Command.  If there were no parsed Options the collection will be empty.
        /// </summary>
        /// <value>Collected of parsed Options</value>
        public OptionCollection Options { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.Command"/> with the specified name.
        /// </summary>
        /// <remarks>This is internal and not for direct consumption outside of this assembly.</remarks>
        /// <param name="name">Name of the command.</param>
        internal Command(string name)
        {
            this.Name = name;
            this.Options = new OptionCollection();
        }

        /// <summary>
        /// Adds an option to this Command's <see cref="Options"/> Collection.
        /// </summary>
        /// <remarks>This is internal and not for direct consumption outside of this assembly.</remarks>
        /// <param name="option">Instance of an Option object to add.</param>
        internal void AddOption(Option option)
        {
            this.Options.Add(option);
        }

        /// <summary>
        /// Returns the first Value of the first Option in this Command.
        /// </summary>
        /// <value>Value of the first Option or "" if the <see cref="Options"/> collection is empty.</value>
        public string Value
        {
            get
            {
                if (this.Options.Count > 0)
                {
                    return this.Options[0].Value;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}