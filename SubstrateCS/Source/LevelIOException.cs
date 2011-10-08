using System;
using System.Runtime.Serialization;

namespace Substrate
{
    /// <summary>
    /// The exception that is thrown when IO errors occur during level management operations.
    /// </summary>
    [Serializable]
    public class LevelIOException : SubstrateException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelIOException"/> class.
        /// </summary>
        public LevelIOException ()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelIOException"/> class with a custom error message.
        /// </summary>
        /// <param name="message">A custom error message.</param>
        public LevelIOException (string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelIOException"/> class with a custom error message and a reference to
        /// an InnerException representing the original cause of the exception.
        /// </summary>
        /// <param name="message">A custom error message.</param>
        /// <param name="innerException">A reference to the original exception that caused the error.</param>
        public LevelIOException (string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelIOException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected LevelIOException (SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
