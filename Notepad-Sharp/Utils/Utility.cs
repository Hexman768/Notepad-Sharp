using System;
using System.Windows.Forms;

namespace NotepadSharp.Utils
{
    static class Utility
    {
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
            + "JSON file (*.json)" + "|*.json|"
            + "Windows Batch file (*.bat)" + "|*.bat|"
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
