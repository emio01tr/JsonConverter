namespace JsonConverter
{
    partial class ProgressForm
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
            formMessage = new Label();
            SuspendLayout();
            // 
            // formMessage
            // 
            formMessage.AutoSize = true;
            formMessage.Location = new Point(54, 66);
            formMessage.Name = "formMessage";
            formMessage.Size = new Size(292, 20);
            formMessage.TabIndex = 0;
            formMessage.Text = "Ürün gönderimi yapılıyor. Lütfen bekleyiniz";
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(411, 158);
            Controls.Add(formMessage);
            Name = "ProgressForm";
            Text = "ProgressForm";
            Load += ProgressForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label formMessage;
    }
}