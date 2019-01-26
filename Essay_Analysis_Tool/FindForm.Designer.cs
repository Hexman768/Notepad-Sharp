namespace Essay_Analysis_Tool
{
    partial class findFunctionForm
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
            this.findFormLabel1 = new System.Windows.Forms.Label();
            this.findFormTextBox = new System.Windows.Forms.TextBox();
            this.findFormFindButton = new System.Windows.Forms.Button();
            this.findFormCancelButton = new System.Windows.Forms.Button();
            this.findFormDirectGroupBox = new System.Windows.Forms.GroupBox();
            this.findFormRadioButtonUp = new System.Windows.Forms.RadioButton();
            this.findFormRadioButtonDown = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.findFormDirectGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // findFormLabel1
            // 
            this.findFormLabel1.AutoSize = true;
            this.findFormLabel1.Location = new System.Drawing.Point(12, 9);
            this.findFormLabel1.Name = "findFormLabel1";
            this.findFormLabel1.Size = new System.Drawing.Size(59, 13);
            this.findFormLabel1.TabIndex = 0;
            this.findFormLabel1.Text = "Find what: ";
            // 
            // findFormTextBox
            // 
            this.findFormTextBox.Location = new System.Drawing.Point(68, 9);
            this.findFormTextBox.Name = "findFormTextBox";
            this.findFormTextBox.Size = new System.Drawing.Size(300, 20);
            this.findFormTextBox.TabIndex = 1;
            // 
            // findFormFindButton
            // 
            this.findFormFindButton.Location = new System.Drawing.Point(374, 9);
            this.findFormFindButton.Name = "findFormFindButton";
            this.findFormFindButton.Size = new System.Drawing.Size(75, 23);
            this.findFormFindButton.TabIndex = 2;
            this.findFormFindButton.Text = "Find Next";
            this.findFormFindButton.UseVisualStyleBackColor = true;
            this.findFormFindButton.Click += new System.EventHandler(this.findFormFindButton_Click);
            // 
            // findFormCancelButton
            // 
            this.findFormCancelButton.Location = new System.Drawing.Point(374, 38);
            this.findFormCancelButton.Name = "findFormCancelButton";
            this.findFormCancelButton.Size = new System.Drawing.Size(75, 23);
            this.findFormCancelButton.TabIndex = 3;
            this.findFormCancelButton.Text = "Cancel";
            this.findFormCancelButton.UseVisualStyleBackColor = true;
            // 
            // findFormDirectGroupBox
            // 
            this.findFormDirectGroupBox.Controls.Add(this.findFormRadioButtonDown);
            this.findFormDirectGroupBox.Controls.Add(this.findFormRadioButtonUp);
            this.findFormDirectGroupBox.Location = new System.Drawing.Point(256, 38);
            this.findFormDirectGroupBox.Name = "findFormDirectGroupBox";
            this.findFormDirectGroupBox.Size = new System.Drawing.Size(112, 47);
            this.findFormDirectGroupBox.TabIndex = 4;
            this.findFormDirectGroupBox.TabStop = false;
            this.findFormDirectGroupBox.Text = "Direction";
            // 
            // findFormRadioButtonUp
            // 
            this.findFormRadioButtonUp.AutoSize = true;
            this.findFormRadioButtonUp.Location = new System.Drawing.Point(6, 19);
            this.findFormRadioButtonUp.Name = "findFormRadioButtonUp";
            this.findFormRadioButtonUp.Size = new System.Drawing.Size(39, 17);
            this.findFormRadioButtonUp.TabIndex = 0;
            this.findFormRadioButtonUp.TabStop = true;
            this.findFormRadioButtonUp.Text = "Up";
            this.findFormRadioButtonUp.UseVisualStyleBackColor = true;
            // 
            // findFormRadioButtonDown
            // 
            this.findFormRadioButtonDown.AutoSize = true;
            this.findFormRadioButtonDown.Location = new System.Drawing.Point(51, 19);
            this.findFormRadioButtonDown.Name = "findFormRadioButtonDown";
            this.findFormRadioButtonDown.Size = new System.Drawing.Size(53, 17);
            this.findFormRadioButtonDown.TabIndex = 1;
            this.findFormRadioButtonDown.TabStop = true;
            this.findFormRadioButtonDown.Text = "Down";
            this.findFormRadioButtonDown.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 68);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(83, 17);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Match Case";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // findFunctionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 92);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.findFormDirectGroupBox);
            this.Controls.Add(this.findFormCancelButton);
            this.Controls.Add(this.findFormFindButton);
            this.Controls.Add(this.findFormTextBox);
            this.Controls.Add(this.findFormLabel1);
            this.Name = "findFunctionForm";
            this.Text = "Find";
            this.Load += new System.EventHandler(this.findFunctionForm_Load);
            this.findFormDirectGroupBox.ResumeLayout(false);
            this.findFormDirectGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label findFormLabel1;
        private System.Windows.Forms.TextBox findFormTextBox;
        private System.Windows.Forms.Button findFormFindButton;
        private System.Windows.Forms.Button findFormCancelButton;
        private System.Windows.Forms.GroupBox findFormDirectGroupBox;
        private System.Windows.Forms.RadioButton findFormRadioButtonDown;
        private System.Windows.Forms.RadioButton findFormRadioButtonUp;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}