using System;

namespace NotepadSharp.Core.Services
{
    public static class ServiceSingleton
    {
        volatile static IServiceProvider instance = new ServiceProvider();

        public static IServiceProvider ServiceProvider
        {
            get
            {
                return instance;
            }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException("value");
                }
                instance = value;
            }
        }

        public static T GetRequiredService<T>()
        {
            var service = instance.GetService(typeof(T));
            return (T)service;
        }
    }
}
