using System.Collections.ObjectModel;
using System.Xml;

#nullable enable

namespace Sprocket.Args
{
    internal class OptionDefCollection : KeyedCollection<string, OptionDef>
    {
        protected override string GetKeyForItem(OptionDef item)
        {
            return item.Name.ToUpperInvariant();
        }
    }


    /// <summary>
    /// Holds details of a option as determined from the XML passed to <![CDATA[<see cref="Processor.LoadDefinitionsFromXML(string)"/>]]>.
    /// This class is internal to hide internal API implementation details from public use.
    /// </summary>
    internal class OptionDef
    {
        public string Name { get; private set; }
        public bool Required { get; private set; }
        public string? DefaultValue { get; private set; }
        public bool AllowMultiple { get; private set; }
        public bool IsFlag { get; private set; }

        internal OptionDef()
        {
            this.Name = "";
            this.Required = false;
            this.AllowMultiple = false;
        }

        internal OptionDef(XmlNode node)
        {
            string? name = node.Attributes?["name"]?.Value;

            if (name == null || (name = name.Trim()) == "")
            {
                throw new XMLProcessingException("option missing name attribute", node);
            }

            this.Name = name;
            this.Required = Assist.GetOptionalBooleanAttribute(node, "required", false);
            this.AllowMultiple = Assist.GetOptionalBooleanAttribute(node, "allow_multiple", false);
            this.DefaultValue = Assist.GetOptionalStringAttribute(node, "default_value", null);
            this.IsFlag = Assist.GetOptionalBooleanAttribute(node, "is_flag", false);

            //Allow for alternative '-' as well as '_' to avoid frustration
            this.AllowMultiple = Assist.GetOptionalBooleanAttribute(node, "allow-multiple", this.AllowMultiple);
            this.DefaultValue = Assist.GetOptionalStringAttribute(node, "default-value", this.DefaultValue);
            this.IsFlag = Assist.GetOptionalBooleanAttribute(node, "is-flag", this.IsFlag);

            // Flag fields cannot have multiple values.  If a flag field has been entered then it is automatically given the value of "true".
            if (this.IsFlag && this.AllowMultiple)
            {
                throw new XMLProcessingException("Flag fields do not support multiple values", node);
            }
        }


    }
}