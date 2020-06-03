using FastColoredTextBoxNS;
using WeifenLuo.WinFormsUI.Docking;

namespace Essay_Analysis_Tool.Windows
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
                if (value != null)
                {
                    documentMap1.Target = value;
                }
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
