using FastColoredTextBoxNS;
using WeifenLuo.WinFormsUI.Docking;

namespace NotepadSharp.Windows
{
    public partial class DocMap : DockContent
    {
        /// <summary>
        /// Returns the Target of the DocumentMap.
        /// </summary>
        public FastColoredTextBox Target
        {
            get
            {
                return documentMap1.Target;
            }
            set
            {
                documentMap1.Target = value;
            }
        }

        /// <summary>
        /// Constructs the <see cref="DocMap"/>.
        /// </summary>
        public DocMap()
        {
            InitializeComponent();
        }
    }
}
