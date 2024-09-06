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
        private int projectCount = 0;
        private int selectedProjectCount = 0;
        private bool skipListItemCheckEvent = false;

        public List<string> projectList { get; set; } = new List<string>();
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
            projectCount = 0;
            selectedProjectCount = 0;
        }

        private void lnkSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lstProjects.Items.Count > 0)
            {
                skipListItemCheckEvent = true;
                foreach (ListViewItem item in lstProjects.Items)
                {
                    item.Checked = true;
                }
                selectedProjectCount = projectCount;
                toolStripProjectStatus.Text = string.Format("{0} out of {1} projects selected", selectedProjectCount, projectCount);
                skipListItemCheckEvent = false;
            }
        }

        private void lnkClearSelection_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lstProjects.Items.Count > 0)
            {
                skipListItemCheckEvent = true;
                foreach (ListViewItem item in lstProjects.Items)
                {
                    item.Checked = false;
                }
                selectedProjectCount = 0;
                toolStripProjectStatus.Text = string.Format("{0} out of {1} projects selected", selectedProjectCount, projectCount);
                skipListItemCheckEvent = false;
            }
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
            foreach (ListViewItem item in lstProjects.CheckedItems)
            {
                projectList.Add(item.SubItems[2].Text);
            }

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
            projectCount = projects.Count;

            lstProjects.Items.Clear();
            lstProjects.Columns.Clear();
            selectedProjectCount = 0;
            skipListItemCheckEvent = true;

            if (projects.Count > 0)
            {
                lstProjects.CheckBoxes = true;
                lstProjects.Columns.Add(string.Empty, -2);
                lstProjects.Columns.Add("Project Name", -2);
                lstProjects.Columns.Add("Path", -2);

                foreach (string project in projects)
                {
                    ListViewItem item = new ListViewItem();
                    item.Checked = false;
                    item.SubItems.Add(project.Substring(project.LastIndexOf('\\') + 1).Replace(".csproj", ""));
                    item.SubItems.Add(project);
                    lstProjects.Items.Add(item);
                }

                lstProjects.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstProjects.Columns[lstProjects.Columns.Count - 1].Width = -2;
                toolStripProjectStatus.Text = string.Format("{0} projects loaded", projectCount);
            }
            else
            {
                lstProjects.CheckBoxes = false;
                lstProjects.Columns.Add(string.Empty);
                lstProjects.Items.Add("Selected folder does not have any projects.");
                lstProjects.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }

            skipListItemCheckEvent = false;
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

        private void lstProjects_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (skipListItemCheckEvent)
            {
                return;
            }

            if(e.Item.Checked)
            {
                selectedProjectCount++;
            }
            else
            {
                selectedProjectCount--;
            }


            toolStripProjectStatus.Text = string.Format("{0} out of {1} projects selected", selectedProjectCount, projectCount);
        }
    }
}
