using System;
using System.Xml;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// Internal class containing miscellaneous methods.
    /// This class is internal to prevent external use to avoid issues in the future if the internal 
    /// logic is changed.
    /// </summary>
    internal class Assist
    {

        /// <summary>
        /// If present convert the specific XML node attribe to a boolean value. If not present then return the default value.
        /// </summary>
        /// <param name="node">The XmlNode to check for the attribute.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="defaultValue">The value to return if the specified attribute does not exist in the XmlNode.</param>
        /// <returns>The value of the attribute expressed as a boolean or the defaultValue.</returns>
        /// <remarks>
        /// The value of the attribute, if present, is expected to be the word "true" or "false".
        /// </remarks>
        /// <exception cref="XMLProcessingException">Thrown if the value of the attribute, if present, is not set to the words "true" or "false".</exception>
        public static bool GetOptionalBooleanAttribute(XmlNode node, string name, bool defaultValue)
        {
            string? value = node.Attributes?[name]?.Value;

            if (value == null)
            {
                return defaultValue;
            }

            value = value.Trim();

            if ("true".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if ("false".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                throw new XMLProcessingException($@"Invalid value for attribute {name}.  Must be 'true' or 'false'", node);
            }

        }

        /// <summary>
        /// If present returns the value of the XML node attribte. If not presenbt then return the defaultValue.
        /// </summary>
        /// <param name="node">The XmlNode to check for the attribute.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="defaultValue">The value to return if the specified attribute does not exist in the XmlNode.</param>
        /// <returns>The value of the attribute expressed or the defaultValue.</returns>
        public static string? GetOptionalStringAttribute(XmlNode node, string name, string? defaultValue)
        {
            return node.Attributes?[name]?.Value ?? defaultValue;
        }
    }
}
