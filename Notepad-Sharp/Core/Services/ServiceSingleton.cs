namespace NotepadSharp.Core.Services
{
    /// <summary>
    /// This is a Singleton class that returns an instance of a
    /// service that matches the given object type.
    /// </summary>
    public static class ServiceSingleton
    {
        volatile static System.IServiceProvider instance = new ServiceProvider();

        /// <summary>
        /// Instance of the <see cref="System.IServiceProvider"/>.
        /// </summary>
        public static System.IServiceProvider ServiceProvider
        {
            get
            {
                return instance;
            }
            set
            {
                if (null == value)
                {
                    throw new System.ArgumentNullException("value");
                }
                instance = value;
            }
        }

        /// <summary>
        /// Returns an instance of a service that matches
        /// the given object type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRequiredService<T>()
        {
            var service = instance.GetService(typeof(T));
            return (T)service;
        }
    }
}
