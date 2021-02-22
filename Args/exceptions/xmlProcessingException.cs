using System;
using System.Text;
using System.Xml;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// Thrown when an error or problem is encountered parsing the XML that defines available Commands and Options.
    /// See also <seealso cref="Sprocket.Args.Processor.LoadDefinitionsFromXML"/>.
    /// </summary>
    public class XMLProcessingException : ArgsException
    {
        /// <summary>
        /// If set, this contains the line from the XML containing the problem reported.
        /// </summary>
        /// <value>Line from the XML relating to the exception.</value>
        public string? DefinitionErrorLine { get; protected set; }

        /// <summary>
        /// If set, this contains the System.Xml.XmlNode relating to the exception.
        /// </summary>
        /// <value>The System.Xml.XmlNode relating to the exception.</value>
        public XmlNode? XmlNode { get; protected set; }


        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.XMLProcessingException"/> with a specified error message.
        /// </summary>
        /// <param name="message">Message describing the exception.</param>
        public XMLProcessingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.XMLProcessingException"/> with a specified error message and a reference to the inner exception.
        /// </summary>
        /// <param name="message">Message describing the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public XMLProcessingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }


        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.XMLProcessingException"/> based on details from .NET's System.Xml.XmlException.
        /// </summary>
        /// <remarks>
        /// If xmlException.LineNumber is set it will populate property <see cref="DefinitionErrorLine"/> with that line from the xml.
        /// </remarks>
        /// <param name="xmlException">The System.Xml.XmlException that was thrown.</param>
        /// <param name="xml">The XML being processed.</param>
        public XMLProcessingException(System.Xml.XmlException xmlException, string xml)
            : base(xmlException.Message, xmlException)
        {
            if (xmlException.LineNumber > 0)
            {
                var lines = xml.Split(new[] { "\r\n", "\n\r", "\r", "\n" }, StringSplitOptions.None);

                this.DefinitionErrorLine = lines[xmlException.LineNumber - 1];
            }

        }

        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.XMLProcessingException"/> with a specified error message and System.Xml.XmlNode.
        /// </summary>
        /// <param name="message">Message describing the exception.</param>
        /// <param name="node">The System.Xml.XmlNode relating to the exception.</param>
        public XMLProcessingException(string message, XmlNode node)
            : base(message)
        {
            this.XmlNode = node;
        }

        /// <summary>
        /// Creates a string containing the error message and, if known, the XML where the error was detected.
        /// </summary>
        /// <returns>String contaning the error message and other details</returns>
        public new string ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine(this.Message);

            if (this.DefinitionErrorLine != null)
            {
                sb.AppendLine(this.DefinitionErrorLine);
            }

            if (this.XmlNode != null)
            {
                using (var sw = new System.IO.StringWriter())
                {
                    using (var xw = new System.Xml.XmlTextWriter(sw))
                    {
                        xw.Formatting = System.Xml.Formatting.Indented;
                        xw.Indentation = 2;
                        this.XmlNode.WriteTo(xw);
                    }

                    sb.AppendLine(sw.ToString());
                }
            }

            return sb.ToString();
        }
    }
}