namespace ProductJsonExporter
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnSelectFile;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            btnSelectFile = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            checkBox1 = new CheckBox();
            label2 = new Label();
            label3 = new Label();
            requesturl = new TextBox();
            SuspendLayout();
            // 
            // btnSelectFile
            // 
            btnSelectFile.Location = new Point(135, 167);
            btnSelectFile.Margin = new Padding(3, 4, 3, 4);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new Size(217, 54);
            btnSelectFile.TabIndex = 0;
            btnSelectFile.Text = "Excel Dosyası Seç";
            btnSelectFile.UseVisualStyleBackColor = true;
            btnSelectFile.Click += btnSelectFile_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(227, 100);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(254, 27);
            textBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(135, 103);
            label1.Name = "label1";
            label1.Size = new Size(86, 20);
            label1.TabIndex = 2;
            label1.Text = "StoreCode :";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(293, 77);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(18, 17);
            checkBox1.TabIndex = 3;
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(135, 74);
            label2.Name = "label2";
            label2.Size = new Size(143, 20);
            label2.TabIndex = 4;
            label2.Text = "Grup Fiyatı Aktif Mi?";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(135, 136);
            label3.Name = "label3";
            label3.Size = new Size(85, 20);
            label3.TabIndex = 5;
            label3.Text = "Request Url";
            // 
            // requesturl
            // 
            requesturl.Location = new Point(227, 133);
            requesturl.Name = "requesturl";
            requesturl.Size = new Size(254, 27);
            requesturl.TabIndex = 6;
            requesturl.Text = "http://192.168.89.99:9996";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(547, 308);
            Controls.Add(requesturl);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(checkBox1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(btnSelectFile);
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainForm";
            Text = "JSON Dönüştürücü";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox textBox1;
        private Label label1;
        private CheckBox checkBox1;
        private Label label2;
        private Label label3;
        private TextBox requesturl;
    }
}
