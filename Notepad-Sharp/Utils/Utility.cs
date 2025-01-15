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
            + "Assembly Program file (*.asm)" + "|*.asm|"
            + "All files (*.*)" + "|*.*";
            return dialog;
        }
    }
}
