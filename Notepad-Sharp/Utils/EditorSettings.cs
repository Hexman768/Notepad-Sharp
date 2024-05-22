using FastColoredTextBoxNS;
using System.Drawing;
using System.Windows.Forms;

namespace NotepadSharp.Utils
{
    public static class EditorSettings
    {
        public static Font Font = new Font("Consolas", 9.75f);
        public static DockStyle DockStyle = DockStyle.Fill;
        public static BorderStyle BorderStyle = BorderStyle.Fixed3D;
        public static int LeftPadding = 17;
        public static HighlightingRangeType HighlightingRangeType = HighlightingRangeType.VisibleRange;
        public static string Tag = "";
        public static Color ChangedLineColor = Color.FromArgb(255, 230, 230, 255);
        public static Color CurrentLineColor = Color.FromArgb(100, 210, 210, 255);
        public static Style SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));
    }
}
