namespace VRC_Fast_Picture_Copy
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            FolderLabel = new Label();
            FileLabel = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("Yu Gothic UI", 30F);
            button1.Location = new Point(12, 55);
            button1.Name = "button1";
            button1.Size = new Size(356, 71);
            button1.TabIndex = 0;
            button1.Text = "最新のものをコピー";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // FolderLabel
            // 
            FolderLabel.AutoSize = true;
            FolderLabel.Font = new Font("Yu Gothic UI", 13F);
            FolderLabel.Location = new Point(12, 2);
            FolderLabel.Name = "FolderLabel";
            FolderLabel.Size = new Size(65, 25);
            FolderLabel.TabIndex = 1;
            FolderLabel.Text = "フォルダ";
            // 
            // FileLabel
            // 
            FileLabel.AutoSize = true;
            FileLabel.Font = new Font("Yu Gothic UI", 13F);
            FileLabel.Location = new Point(12, 27);
            FileLabel.Name = "FileLabel";
            FileLabel.Size = new Size(63, 25);
            FileLabel.TabIndex = 2;
            FileLabel.Text = "ファイル";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(386, 130);
            Controls.Add(FileLabel);
            Controls.Add(FolderLabel);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Main";
            Text = "VRC Fast Picture Copy";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label FolderLabel;
        private Label FileLabel;
    }
}
