using System;
using System.Collections.Generic;
using System.Text.Json;

#nullable enable

namespace Sprocket.Args
{

    /// <summary>
    /// A collection on <see cref="Sprocket.Args.Option"/> objects. Inherits from !<![CDATA[List<string>]]>.
    /// </summary>
    public class OptionCollection : List<Option>
    {
        /// <summary>
        /// Returns the first <see cref="Option"/> within this collection with <see cref="Option.Name"/>.  The match is case insensitive.
        /// </summary>
        /// <param name="optionName">Name of the option to find.</param>
        /// <value>Returns the first matching <see cref="Option"/> or null if not found.</value>
        public Option? this[string optionName]
        {
            get
            {
                foreach (Option opt in this)
                {
                    if (opt.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase))
                    {
                        return opt;
                    }

                }

                return null;
            }
        }

        /// <summary>
        /// Checks to see if any <see cref="Option"/> with the specified <see cref="Option.Name"/> exists within this collection.  The check is case insensitive.
        /// </summary>
        /// <param name="optionName">Name of the option to look for.</param>
        /// <returns>True if it exists, fasle if not.</returns>
        public bool Exists(string optionName)
        {
            foreach (Option opt in this)
            {
                if (opt.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Returns the first value of the <see cref="Option"/> within this collection with the specified <see cref="Option.Name"/>.  The check is case insensitive.
        /// If the option does not exist then the specified default value is returned.
        /// </summary>
        /// <param name="optionName">Name of the option.</param>
        /// <param name="defaultValue">Value to be returned if an option with that name is not found.</param>
        /// <returns>Value of the found option or defaultValue if not found.</returns>
        public string Value(string optionName, string defaultValue)
        {
            Option? opt = this[optionName];
            if (opt != null)
            {
                // returns the first value
                return opt.Value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns all the <see cref="Option.Values"/> for the specified <see cref="Option"/> within this collection.
        /// If the option does not exist then the specified default values are returned.
        /// </summary>
        /// <param name="optionName">Name of the option</param>
        /// <param name="defaultValues">Values to be returned if an option with that name is not found.</param>
        /// <returns>The values of the specified option or defaultValues if not found.</returns>
        public List<string> Values(string optionName, params string[] defaultValues)
        {
            Option? opt = this[optionName];
            if (opt != null)
            {
                // returns all values
                return opt.Values;
            }
            else
            {
                List<string> res = new();
                foreach (string value in defaultValues)
                {
                    res.Add(value);
                }
                return res;
            }
        }

        /// <summary>
        /// Adds a new <see cref="Option"/> object to the collection.
        /// </summary>
        /// <param name="newOption">The <see cref="Option"/> to add.</param>
        /// <remarks>This is internal and not for direct consumption outside of this assembly.</remarks>
        internal new void Add(Option newOption)
        {
            // If option with same name exsits then append values into existing item
            Option? existing = this[newOption.Name];
            if (existing != null)
            {
                foreach (string value in newOption.Values)
                {
                    existing.Values.Add(value);
                }
            }
            else
            {
                base.Add(newOption);
            }
        }

        /// <summary>
        /// Returns the count of the number of Options within in this collection with the specified name.  The match is case insensitive.
        /// </summary>
        /// <param name="optionName">The <see cref="Option.Name"/> of the <see cref="Option"/> to look for.</param>
        /// <returns>The number of Options within this collection with the specified name.</returns>
        public int CountWithName(string optionName)
        {
            int count = 0;
            foreach (Option opt in this)
            {
                if (opt.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            return count;
        }
    }

}