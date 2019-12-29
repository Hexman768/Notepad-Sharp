namespace Essay_Analysis_Tool
{
    partial class LoggerView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoggerView));
            this.console = new FastColoredTextBoxNS.FastColoredTextBox();
            this.selectButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.console)).BeginInit();
            this.SuspendLayout();
            // 
            // console
            // 
            this.console.AutoCompleteBracketsList = new char[] {
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
            this.console.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.console.BackBrush = null;
            this.console.CharHeight = 14;
            this.console.CharWidth = 8;
            this.console.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.console.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.console.IsReplaceMode = false;
            this.console.Location = new System.Drawing.Point(12, 12);
            this.console.Name = "console";
            this.console.Paddings = new System.Windows.Forms.Padding(0);
            this.console.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.console.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("console.ServiceColors")));
            this.console.Size = new System.Drawing.Size(776, 389);
            this.console.TabIndex = 0;
            this.console.Zoom = 100;
            // 
            // selectButton
            // 
            this.selectButton.Location = new System.Drawing.Point(713, 407);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(75, 23);
            this.selectButton.TabIndex = 1;
            this.selectButton.Text = "Select All";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(632, 407);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 23);
            this.copyButton.TabIndex = 2;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // LoggerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 436);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.console);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoggerForm";
            this.Text = "Logger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoggerForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.console)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox console;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Button copyButton;
    }
}