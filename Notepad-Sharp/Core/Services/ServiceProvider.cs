using NotepadSharp.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace NotepadSharp.Core.Services
{
    sealed class ServiceProvider : IServiceProvider
    {
        private static Dictionary<Type, object> _serviceDict = new Dictionary<Type, object>();

        public void AddService(Type serviceType, object instance)
        {
            _serviceDict.Add(serviceType, instance);
        }

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

