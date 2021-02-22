using System;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// Base exception class for Sprocket.Args.  
    /// All exceptions thrown from Sprocket.Args are either of type ArgsException or inherit from it.
    /// </summary>
    public class ArgsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.ArgsException"/> with a specified error message.
        /// </summary>
        /// <param name="message">Message describing the exception.</param>
        public ArgsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Sprocket.Args.ArgsException"/> with a specified error message and a refernece to the inner exception.
        /// </summary>
        /// <param name="message">Message describing the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ArgsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}