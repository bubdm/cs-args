using System;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml;

#nullable enable

namespace Sprocket.Args
{

    /// <summary>
    /// The Processor class parses the input string against the previously loaded Commands and Options definition.
    /// </summary>
    public class Processor
    {
        /// <summary>
        /// Private collection of CommandDefs parsed from the XML via <see cref="LoadDefinitionsFromXML"/>.
        /// </summary>
        /// <value>A collection of <see cref="CommandDef"/> objects.</value>
        private CommandDefCollection CommandDefs { get; set; }

        /// <summary>
        /// Private collection of OptionsDefs parsed from the XML via <see cref="LoadDefinitionsFromXML"/>.
        /// </summary>
        /// <value>A collection of <see cref="OptionDef"/> objects.</value>
        private OptionDefCollection OptionDefs { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Processor"/>.
        /// </summary>
        public Processor()
        {
            this.CommandDefs = new CommandDefCollection();
            this.OptionDefs = new OptionDefCollection();
        }


        /// <summary>
        /// Processes the XML that defines the allowed commands and options.  A definition must be loaded prior to calling <see cref="Processor"/>.<see cref="Processor.Parse"/>.
        /// </summary>
        /// <param name="xml">The XML defining allowed commands and options.</param>
        /// <exception cref="XMLProcessingException">Thrown if the XML is invalid or if there are issues with the definition (for example, a duplicated command name).</exception>
        /// <example>
        /// <code>
        /// Processor p = new();
        /// p.LoadDefinitionsFromXML(@"<![CDATA[<xml>]]>
        ///         <![CDATA[<command name='delete'>]]>
        ///             <![CDATA[<option name='filespec' />]]>
        ///             <![CDATA[<option name='-confirm' default_value='yes' />]]>
        ///         <![CDATA[</command>]]>
        ///         <![CDATA[<command name='restore'>]]>
        ///             <![CDATA[<option name='filespec' />]]>
        ///             <![CDATA[<option name='-backup' default_value='no' />]]>
        ///         <![CDATA[</command>]]>
        ///         <![CDATA[<option name='--language' />]]>
        ///     <![CDATA[</xml>]]>");
        /// </code>
        /// </example>
        public void LoadDefinitionsFromXML(string xml)
        {
            try
            {
                XmlDocument x = new();

                //trim all lines in xml, so whitespaces are not recorded in position counts if an error is reported
                var lines = xml.Split(new[] { "\r\n", "\n\r", "\r", "\n" }, StringSplitOptions.None);

                StringBuilder sb = new();
                foreach (string line in lines)
                {
                    sb.AppendLine(line.Trim());
                }

                xml = sb.ToString();

                //load the xml
                x.LoadXml(xml);

                // anything loaded
                if (x.HasChildNodes)
                {
                    XmlNodeList nodes = x.ChildNodes;

                    // if only one node then check for the possability this is a top level <xml> (or order node) containing 1 or more <command> child nodes
                    if (1 == nodes.Count)
                    {
                        XmlNode? top = x.FirstChild;

                        // We are expecting one or more commands as child nodes within a top level node: -
                        //
                        //    <xml>
                        //        <command>|<option>
                        //            ... ...
                        //        </command>|<option>
                        //    <xml>
                        //
                        //  or just a single command in a single node: -
                        //
                        //    <command>|<option>
                        //        ... ...
                        //    </command>|<option>
                        //
                        // If the FirstChild is not <command> then assume the first node is <xml> (or other name) and skip into its child nodes
                        if (null != top && top.HasChildNodes
                                && !"command".Equals(top.Name, StringComparison.OrdinalIgnoreCase)
                                && !"option".Equals(top.Name, StringComparison.OrdinalIgnoreCase)
                         )
                        {
                            nodes = top.ChildNodes;
                        }
                    }

                    // traverse through child nodes which are expected to be <command> or <option> nodes
                    // <option> nodes at this level are considered global
                    foreach (XmlNode node in nodes)
                    {
                        if (node.Name.Equals("command", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                this.AddCommandDef(new CommandDef(node));
                            }
                            catch (System.Exception ex)
                            {

                                throw new XMLProcessingException(ex.Message, node);
                            }

                        }
                        else if (node.Name.Equals("option", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                OptionDef newOptionDef = new OptionDef(node);
                                if (newOptionDef.Required)
                                {
                                    throw new XMLProcessingException($"Global Option '{newOptionDef.Name}' Cannot Be Flagged As Required", node);
                                }
                                else
                                {
                                    this.OptionDefs.Add(newOptionDef);
                                }
                            }
                            catch (System.Exception ex)
                            {

                                throw new XMLProcessingException(ex.Message, node);
                            }
                        }
                        else
                        {
                            throw new XMLProcessingException($"Error processing definitions from XML.  Invalid node name of '{node.Name}'", node);
                        }
                    }
                }
            }
            catch (XMLProcessingException)
            {
                throw;
            }
            catch (System.Xml.XmlException ex)
            {
                throw new XMLProcessingException(ex, xml);
            }
            catch (System.Exception ex)
            {
                throw new XMLProcessingException(@"Error loading definitions from XML: " + ex.Message, ex);
            }

            //check that no global options have the same name as commands
            foreach (OptionDef optDef in this.OptionDefs)
            {
                if (this.CommandDefs.Contains(optDef.Name.ToUpperInvariant()))
                {
                    throw new XMLProcessingException($"A Command and Global Option have the same name of {optDef.Name}");
                }
            }

        }

        /// <summary>
        /// Adds a CommandDef to this class's private CommandDefCollection <see cref="CommandDefs"/>.
        /// </summary>
        /// <param name="commandDef"><see cref="CommandDef"/> to add to the private collection.</param>
        private void AddCommandDef(CommandDef commandDef)
        {
            if (commandDef.IsDefault && CommandDefs.GetDefault() != null)
            {
                throw new ArgsException("Multiple default commands are not allowed");
            }
            else
            {
                CommandDefs.Add(commandDef);
            }
        }

        /// <summary>
        /// Parses the string into Command, Options and Values.  Parsing is performed against the previously loaded Command and Options definition as passed to <see cref="LoadDefinitionsFromXML"/>.
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>Instance of <see cref="ParseResult"/></returns>
        /// <exception cref="ArgsException">Generic issue with parsing the string.  
        /// This exception will also be raised in this method is called before loading the Command and Options definition via <see cref="LoadDefinitionsFromXML"/>.
        /// </exception>
        /// <exception cref="DuplicateOptionException">Thrown when an option is specified more that once for the same Command or Gobal Option where that option has not been configured to allow multiple values.</exception>
        /// <exception cref="InvalidCommandException">Thrown when a unknown word is encounted within the string being parsed.</exception>
        /// <exception cref="MissingValueException">Thrown when an option is specified within the string being parsed but its value was not supplied.</exception>
        /// <example>
        /// <code>
        /// Processor p = new();
        /// p.LoadDefinitionsFromXML(@"<![CDATA[<xml>]]>
        ///         <![CDATA[<option name='filename' />]]>
        ///         <![CDATA[<option name='directory' />]]>
        ///     <![CDATA[</xml>]]>");
        /// string inputFromUser = @"filename = me.txt";
        /// ParseResult res = p.Parse(inputFromUser);
        /// foreach (Option opt in res.GlobalOptions)
        /// {
        ///     Console.WriteLine($"{opt.Name} : {opt.Value}");
        /// }
        /// </code>
        /// Expected output
        /// <code>
        /// filename : me.txt
        /// </code>
        /// </example>
        public ParseResult Parse(string s)
        {
            if (null == s)
            {
                s = string.Empty;
            }
            else
            {
                s = s.Trim();
            }

            if (this.CommandDefs != null)
            {
                Parser p = new Parser(this.CommandDefs, this.OptionDefs, s);
                return p.Parse();
            }
            else
            {
                throw new ArgsException("Call to parse before configuring definitions");
            }
        }
    }
}