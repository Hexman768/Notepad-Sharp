using System.Drawing;

namespace NotepadSharp.Utils
{
    /// <summary>
    /// This class represents the theme of any control within the Program. 
    /// This class instance has no knowledge of the type of control it represents.
    /// </summary>
    class Theme
    {
        public bool _isDefaultTheme;

        private object _editorTheme;
        private Color _backColor;

        public Theme()
        {
            _isDefaultTheme = true;
        }

        public object EditorTheme
        {
            get
            {
                return _editorTheme;
            }
            set
            {
                if (null != value)
                {
                    _editorTheme = value;
                }
            }
        }

        public Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                if (null != value)
                {
                    _backColor = value;
                }
            }
        }
    }
}
