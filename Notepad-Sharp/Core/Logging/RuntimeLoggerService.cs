namespace NotepadSharp.Core.Logging
{
    /// <summary>
    /// This class is the logger service for runtime usage.
    /// Any messages that need to be relayed to the user will be 
    /// in the form of a <see cref="MessageBox"/>.
    /// </summary>
    public class RuntimeLoggerService : ILoggingService
    {
        /// <summary>
        /// Writes error message to log file.
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            Write("Error", message);
        }

        /// <summary>
        /// Writes informational message to log file.
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            Write("Info", message);
        }

        /// <summary>
        /// Writes warning message to log file.
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message)
        {
            Write("Warning", message);
        }

        private static void Write(string type, string message)
        {
            string _path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            try
            {
                using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(System.IO.Path.Combine(_path, "Log.txt")))
                {
                    streamWriter.WriteLine($"{System.DateTime.Now.ToLongTimeString()} {System.DateTime.Now.ToLongDateString()} [{type}]: {message}");
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Error");
            }
        }
    }
}
