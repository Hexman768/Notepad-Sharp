using System;

namespace NotepadSharp.Core.Exceptions
{
    public class ServiceNotFoundException : System.Exception
    {
        public ServiceNotFoundException() : base()
        {
        }

        public ServiceNotFoundException(string message) : base(message)
        {
        }

        public ServiceNotFoundException(Type serviceType) : base("Required service not found: " + serviceType.FullName)
        {
        }
    }
}
