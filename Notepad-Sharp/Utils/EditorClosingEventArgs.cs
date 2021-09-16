using NotepadSharp.Windows;
using System;

namespace NotepadSharp.Utils
{
    public class EditorClosingEventArgs : EventArgs
    {
        public EditorClosingEventArgs(Editor item)
        {
            if (item != null)
            {
                Item = item;
            }
        }

        public Editor Item { get; set; }
        public bool Cancel { get; set; }
    }
}
