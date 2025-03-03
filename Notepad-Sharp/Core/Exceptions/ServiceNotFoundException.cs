namespace NotepadSharp.Core.Exceptions
{
    /// <summary>
    /// Represents an exception that is only thrown when a service can not be
    /// located in the application.
    /// </summary>
    public class ServiceNotFoundException : System.Exception
    {
        /// <summary>
        /// Constructs an instance of the <see cref="ServiceNotFoundException"/>.
        /// </summary>
        public ServiceNotFoundException() : base() { }

        /// <summary>
        /// Constructs an instance of the <see cref="ServiceNotFoundException"/>
        /// with a message.
        /// </summary>
        /// <param name="message"></param>
        public ServiceNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Constructs an instance of the <see cref="ServiceNotFoundException"/>
        /// with the type of service that can not be located in the application.
        /// </summary>
        /// <param name="serviceType"></param>
        public ServiceNotFoundException(System.Type serviceType) : base("Required service not found: " + serviceType.FullName) { }
    }
}
