using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACSE.Core.Messages.Mail;
using ACSE.Core.Saves;
using ACSE.WinForms.Controls;

namespace ACSE.WinForms.MailEditorForm
{
    public partial class MailEditorForm : Form
    {
        private readonly MessageEditor messageEditor;

        public MailEditorForm()
        {
            // Load player mail
            // TODO: Move this logic to ACSE.Core
            var playerMail = new GcnPlayerMail[10];

            for (var i = 0; i < 10; i++)
            {
                //playerMail[i] = new GcnPlayerMail();
            }

            InitializeComponent();

            messageEditor = new MessageEditor();
            //messageEditor.SetMailReference();
        }
    }
}
