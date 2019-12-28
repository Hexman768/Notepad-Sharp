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

using Essay_Analysis_Tool.Business;
using System;
using System.Windows.Forms;

namespace Essay_Analysis_Tool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainView view = new MainView();
            MainViewController controller = new MainViewController(view);
            view.ShowDialog();
        }
    }
}
