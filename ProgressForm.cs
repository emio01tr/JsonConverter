using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JsonConverter
{
    public partial class ProgressForm : Form
    {
        private CancellationTokenSource _cts;
        public ProgressForm()
        {
            InitializeComponent();
        }

        private async void ProgressForm_Load(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            var cancellationToken = _cts.Token;
            var firstMessage = formMessage.Text;
            int dotcount = 0;
            try
            { 
                while (!_cts.IsCancellationRequested)
                {
                    dotcount++;
                    if (dotcount == 4)
                    {
                        dotcount = 1;
                        formMessage.Text = firstMessage;
                    }
                    formMessage.Text += ".";
                    await Task.Delay(300, cancellationToken);
                }
            }catch(OperationCanceledException ex)
            {
                //
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cts.Cancel();
            base.OnFormClosing(e);
        }
    }
}
