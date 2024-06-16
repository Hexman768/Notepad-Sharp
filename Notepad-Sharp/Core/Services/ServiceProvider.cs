using NotepadSharp.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace NotepadSharp.Core.Services
{
    /// <summary>
    /// This is the one-stop service provider class for the
    /// entire application. This object will return an instance
    /// of any service that is used in the project.
    /// </summary>
    sealed class ServiceProvider : IServiceProvider
    {
        private static Dictionary<Type, object> _serviceDict = new Dictionary<Type, object>();

        /// <summary>
        /// Adds a service instance to the service dictionary.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="instance"></param>
        public void AddService(Type serviceType, object instance)
        {
            _serviceDict.Add(serviceType, instance);
        }

        /// <summary>
        /// Returns an instance of a service that matches the given type.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        /// <exception cref="ServiceNotFoundException"></exception>
        public object GetService(Type serviceType)
        {
            lock (_serviceDict)
            {
                object instance;
                lock (_serviceDict)
                {
                    if (!_serviceDict.TryGetValue(serviceType, out instance))
                    {
                        throw new ServiceNotFoundException(serviceType);
                    }
                }
                return instance;
            }
        }
    }
}

