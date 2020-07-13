using System;

namespace TranslationHelper.OnlineTranslators.XUA
{
    /// <summary>
    /// Exception to be thrown when the initialization of an Endpoint fails.
    /// </summary>
    public class EndpointInitializationException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public EndpointInitializationException() { }

        /// <summary>
        /// Constructs an exception with a message.
        /// </summary>
        /// <param name="message"></param>
        public EndpointInitializationException(string message) : base(message) { }

        /// <summary>
        /// Constructs an exception with a message and inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public EndpointInitializationException(string message, Exception inner) : base(message, inner) { }
    }
}
