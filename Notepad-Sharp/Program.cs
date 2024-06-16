//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE.
//
//  License: GNU Lesser General Public License (LGPLv3)
//
//  Email: Zachary.Pedigo1@gmail.com
//
//  Copyright (C) Zachary Pedigo, 2019.

using NotepadSharp.Core.Logging;
using NotepadSharp.Core.Services;
using NotepadSharp.Core.Utilities;
using System;
using System.Windows.Forms;

namespace NotepadSharp
{
    static class Program
    {
        static ServiceProvider serviceProvider;

        static void StartServices()
        {
            serviceProvider = new ServiceProvider();
            ILoggingService loggerServiceInstance = new RuntimeLoggerService();
#if DEBUG
            loggerServiceInstance = new DebugLoggingService(new TraceTextWriter());
#endif

            serviceProvider.AddService(typeof(ILoggingService), loggerServiceInstance);

            ServiceSingleton.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            StartServices();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
