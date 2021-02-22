using System;
using System.Collections.ObjectModel;
using System.Xml;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// Collection of <see cref="CommandDef"/> objects as determined from the XML passed to <![CDATA[<see cref="Processor.LoadDefinitionsFromXML(string)"/>]]>.
    /// This class is internal to hide internal API implementation details from public use.
    /// </summary>
    internal class CommandDefCollection : KeyedCollection<string, CommandDef>
    {

        public CommandDef? GetDefault()
        {
            foreach (CommandDef cd in this)
            {
                if (cd.IsDefault)
                {
                    return cd;
                }
            }

            return null;
        }

        protected override string GetKeyForItem(CommandDef item)
        {
            return item.Name.ToUpperInvariant();
        }
    }

    /// <summary>
    /// Holds details of a specific command as determined from the XML passed to <![CDATA[<see cref="Processor.LoadDefinitionsFromXML(string)"/>]]>.
    /// This class is internal to hide internal API implementation details from public use.
    /// </summary>
    internal class CommandDef
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        /// <value>Name of the command</value>
        public string Name { get; private set; }

        /// <summary>
        /// Is this the default command.  A default command is used if the input (command line) 
        /// don't contain any commands.
        /// </summary>
        /// <value>Set to true if this is the default command.</value>
        public bool IsDefault { get; private set; }

        /// <summary>
        /// Is this command allows to be specified more that once in the input (command line)
        /// </summary>
        /// <value>If the then this command can be specified more than once the input (command line)</value>
        public bool AllowMultiple { get; private set; }

        /// <summary>
        /// The options relavent to this command
        /// </summary>
        /// <value>Collection of options.  If there are no options for a command this collection will be emppty.</value>
        public OptionDefCollection OptionsDefs { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="CommandDef"/> with the specified details.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isDefault"></param>
        /// <param name="allowMultiple"></param>
        internal CommandDef(string name, bool isDefault = false, bool allowMultiple = false)
        {
            this.OptionsDefs = new OptionDefCollection();
            this.Name = name;
            this.IsDefault = isDefault;
            this.AllowMultiple = allowMultiple;
        }

        /// <summary>
        /// Initialised a new instance of <see cref="CommandDef"/> based on the attributes present in the supplied XmlNode.
        /// </summary>
        /// <param name="node">XmlNode to process</param>
        /// <exception cref="XMLProcessingException">Thrown if the XmlNode does not contain a 'name' attribute.</exception>
        internal CommandDef(XmlNode node)
        {
            this.OptionsDefs = new OptionDefCollection();

            string? name = node.Attributes?["name"]?.Value;

            if (name == null || (name = name.Trim()) == "")
            {
                throw new XMLProcessingException("command missing name attribute", node);
            }

            this.Name = name;
            this.IsDefault = Assist.GetOptionalBooleanAttribute(node, "is_default", false);
            this.AllowMultiple = Assist.GetOptionalBooleanAttribute(node, "allow_multiple", false);

            //Allow for alternative '-' as well as '_' to avoid frustration
            this.IsDefault = Assist.GetOptionalBooleanAttribute(node, "is-default", this.IsDefault);
            this.AllowMultiple = Assist.GetOptionalBooleanAttribute(node, "allow-multiple", this.AllowMultiple);

            if (node.HasChildNodes)
            {
                foreach (XmlNode childNode in node)
                {
                    if ("option".Equals(childNode.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        var newOptionDef = new OptionDef(childNode);

                        this.AddOptionDef(newOptionDef);

                    }
                }
            }
        }

        /// <summary>
        /// Adds a details on an option to this command.
        /// </summary>
        /// <param name="option">The OptionDef to add to this Command.</param>
        public void AddOptionDef(OptionDef option)
        {
            this.OptionsDefs.Add(option);
        }
    }

}