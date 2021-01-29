namespace Essay_Analysis_Tool.Windows
{
    partial class Editor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.mainEditor = new FastColoredTextBoxNS.FastColoredTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // mainEditor
            // 
            this.mainEditor.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.mainEditor.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\n^\\s*(case|default)\\s*[^:]*(" +
    "?<range>:)\\s*(?<range>[^;]+);";
            this.mainEditor.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.mainEditor.BackBrush = null;
            this.mainEditor.CharHeight = 14;
            this.mainEditor.CharWidth = 8;
            this.mainEditor.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mainEditor.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.mainEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainEditor.IsReplaceMode = false;
            this.mainEditor.Location = new System.Drawing.Point(0, 0);
            this.mainEditor.Name = "mainEditor";
            this.mainEditor.Paddings = new System.Windows.Forms.Padding(0);
            this.mainEditor.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.mainEditor.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("mainEditor.ServiceColors")));
            this.mainEditor.Size = new System.Drawing.Size(192, 162);
            this.mainEditor.TabIndex = 0;
            this.mainEditor.Zoom = 100;
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(192, 162);
            this.Controls.Add(this.mainEditor);
            this.Name = "Editor";
            this.Text = "Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Editor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.mainEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public FastColoredTextBoxNS.FastColoredTextBox mainEditor;
    }
}