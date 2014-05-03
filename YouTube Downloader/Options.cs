using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouTube_Downloader
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFolderDialog ofd = new OpenFolderDialog();

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                txtDefaultDirectory.Text = ofd.Folder;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();            
        }

        private void Options_Load(object sender, EventArgs e)
        {
            

            if (SettingsEx.DefaultDirectory != null)
                txtDefaultDirectory.Text = SettingsEx.DefaultDirectory;
            else
                txtDefaultDirectory.Text = "";
            
            chbAutoConvert.Checked = SettingsEx.AutoConvert;
            cbAutoDelete.Checked = SettingsEx.AutoDelete;
        }

        private void Options_FormClosing(object sender, FormClosingEventArgs e)
        {
            SettingsEx.DefaultDirectory = txtDefaultDirectory.Text;
            SettingsEx.SaveToDirectories.Insert(0, txtDefaultDirectory.Text);
            SettingsEx.AutoConvert = chbAutoConvert.Checked;
            SettingsEx.AutoDelete = cbAutoDelete.Checked;
            SettingsEx.Save();
            Application.Restart();
        }
    }
}
