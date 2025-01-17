using System;
using System.Windows.Forms;

namespace NotepadSharp.Utils
{
    /// <summary>
    /// This is a utility class that will hold useful methods that can be called
    /// anywhere throughout the tech stack.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Creates a save file dialog and returns it to the caller.
        /// </summary>
        /// <returns></returns>
        public static SaveFileDialog CreateSaveDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Normal text file (*.txt)|*.txt|"
            + "C# source file (*.cs)" + "|*.cs|"
            + "Hyper Text Markup Language File (*.html)" + "|*.html|"
            + "Javascript source file (*.js)" + "|*.js|"
            + "JSON file (*.json)" + "|*.json|"
            + "Lua source file (*.lua)" + "|*.lua|"
            + "PHP file (*.php)" + "|*.php|"
            + "Structured Query Language file (*.sql)" + "|*.sql|"
            + "Visual Basic file (*.vb)" + "|*.vb|"
            + "VBScript file (*.vbs)" + "|*.vbs|"
            + "All files (*.*)" + "|*.*";
            return dialog;
        }

        public static string GetExtension(string fName)
        {
            if (!fName.Contains("."))
            {
                return "";
            }
            char[] name = fName.ToCharArray();
            string ext = "";
            int token = fName.Length - 1;

            while (name[token] != '.')
            {
                ext += name[token].ToString();
                token -= 1;
            }

            name = ext.ToCharArray();
            Array.Reverse(name);
            token = 0;
            ext = "";
            while (token < name.Length)
            {
                ext += name[token].ToString();
                token += 1;
            }

            return ext;
        }
    }
}
