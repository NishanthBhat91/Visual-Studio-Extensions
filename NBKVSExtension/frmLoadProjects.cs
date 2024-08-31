using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace NBKVSExtension
{
    public partial class frmLoadProjects : Form
    {
        public frmLoadProjects()
        {
            InitializeComponent();
        }

        private void frmLoadProjects_Load(object sender, EventArgs e)
        {
            InitializeControls();
        }

        private void InitializeControls()
        {
            txtFolder.Text = string.Empty;
            chkInternalReference.Checked = false;
            lstProjects.Items.Clear();
            lstProjects.Columns.Clear();
            lstProjects.CheckBoxes = false;
            lstProjects.Columns.Add(string.Empty);
            lstProjects.Items.Add("Select any folder to load projects");
            lstProjects.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void lnkSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //TODO
        }

        private void lnkClearSelection_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
			//TODO
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog folderBrowserDialog = new CommonOpenFileDialog())
            {
                folderBrowserDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                folderBrowserDialog.IsFolderPicker = true;
                CommonFileDialogResult result = folderBrowserDialog.ShowDialog();
                if (result == CommonFileDialogResult.Ok)
                {
                    txtFolder.Text = folderBrowserDialog.FileName;
                    LoadProjectsList(folderBrowserDialog.FileName);
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LoadProjectsList(string folderPath)
        {
            List<string> projects = GetAllProjectFiles(folderPath);
            
			//Load the projects to listview
        }

        static List<string> GetAllProjectFiles(string folderPath)
        {
            List<string> projectFiles = new List<string>();

            try
            {   string[] files = Directory.GetFiles(folderPath, "*.csproj", SearchOption.AllDirectories);
                projectFiles.AddRange(files);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"UnauthorizedAccessException: {e.Message}");
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"DirectoryNotFoundException: {e.Message}");
            }

            return projectFiles;
        }
    }
}
