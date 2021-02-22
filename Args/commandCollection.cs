using System;
using System.Collections.Generic;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// Holds a collection of parsed <see cref="Command"/> objects.  This collection inherits from <![CDATA[List<Command>]]> to ensure that
    /// items are ordered in the same order as they appeared in the command line that was parsed.
    /// </summary>
    public class CommandCollection : List<Command>
    {
        /// <summary>
        /// Returns the first <see cref="Command"/> within this collection matching <see cref="Command.Name"/>.  The match is case insensitive.
        /// </summary>
        /// <value>Returns matching <see cref="Command"/> or null if none found.</value>
        public Command? this[string commandName]
        {
            get
            {
                foreach (Command cmd in this)
                {
                    if (cmd.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
                    {
                        return cmd;
                    }

                }

                return null;
            }
        }

        /// <summary>
        /// Checks to see if a command exists with the specified name.  The check is case insensitive.
        /// </summary>
        /// <param name="commandName">The <see cref="Command"/>.<see cref="Command.Name"/> to look for.</param>
        /// <returns>True if found, false if not.</returns>
        public bool Exists(string commandName)
        {
            foreach (Command cmd in this)
            {
                if (cmd.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

            }

            return false;
        }
    }
}
