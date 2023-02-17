namespace EncryptAndDecrypt
{
    partial class Form1
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
            label1 = new Label();
            txtOrigin = new TextBox();
            txtEncript = new TextBox();
            label2 = new Label();
            btnEncrypt = new Button();
            btnDecrypt = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(68, 49);
            label1.Name = "label1";
            label1.Size = new Size(64, 24);
            label1.TabIndex = 0;
            label1.Text = "原文：";
            // 
            // txtOrigin
            // 
            txtOrigin.Location = new Point(138, 46);
            txtOrigin.Multiline = true;
            txtOrigin.Name = "txtOrigin";
            txtOrigin.Size = new Size(1061, 301);
            txtOrigin.TabIndex = 1;
            // 
            // txtEncript
            // 
            txtEncript.Location = new Point(138, 389);
            txtEncript.Multiline = true;
            txtEncript.Name = "txtEncript";
            txtEncript.Size = new Size(1061, 301);
            txtEncript.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(68, 387);
            label2.Name = "label2";
            label2.Size = new Size(64, 24);
            label2.TabIndex = 2;
            label2.Text = "密文：";
            // 
            // btnEncrypt
            // 
            btnEncrypt.Location = new Point(1220, 49);
            btnEncrypt.Name = "btnEncrypt";
            btnEncrypt.Size = new Size(112, 34);
            btnEncrypt.TabIndex = 4;
            btnEncrypt.Text = "加密";
            btnEncrypt.UseVisualStyleBackColor = true;
            btnEncrypt.Click += btnEncrypt_Click;
            // 
            // btnDecrypt
            // 
            btnDecrypt.Location = new Point(1220, 387);
            btnDecrypt.Name = "btnDecrypt";
            btnDecrypt.Size = new Size(112, 34);
            btnDecrypt.TabIndex = 5;
            btnDecrypt.Text = "解密";
            btnDecrypt.UseVisualStyleBackColor = true;
            btnDecrypt.Click += btnDecrypt_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1376, 739);
            Controls.Add(btnDecrypt);
            Controls.Add(btnEncrypt);
            Controls.Add(txtEncript);
            Controls.Add(label2);
            Controls.Add(txtOrigin);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtOrigin;
        private TextBox txtEncript;
        private Label label2;
        private Button btnEncrypt;
        private Button btnDecrypt;
    }
}