using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alfresco_Upload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AlfrescoUpload alfupload = new AlfrescoUpload(textBox3.Text, textBox4.Text, "/Sites/" + textBox2.Text + "/documentLibrary", textBox1.Text);
            alfupload.clickHandler();
        }
    }
}
