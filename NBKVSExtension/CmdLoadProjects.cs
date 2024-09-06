using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace NBKVSExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CmdLoadProjects
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("3c4c5e54-4a34-492e-b20b-15c312fb2ef5");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdLoadProjects"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CmdLoadProjects(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CmdLoadProjects Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in CmdLoadProjects's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CmdLoadProjects(package, commandService);

            //Create thread wait dialog to display progress bar if it takes long time
            var twdFactory = (IVsThreadedWaitDialogFactory)await Community.VisualStudio.Toolkit.VS.Services.GetThreadedWaitDialogAsync();
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string title = "Load multiple projects Tool";

            using (frmLoadProjects form = new frmLoadProjects())
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK && form.projectList.Count > 0)
                {
                    DTE DTE = Package.GetGlobalService(typeof(DTE)) as DTE;
                    Solution pSolution = DTE.Solution;

                    foreach (string projectPath in form.projectList)
                    {
                        var text = $"Project {form.projectList.IndexOf(projectPath) + 1}/{form.projectList.Count}";
                        pSolution.AddFromFile(projectPath);
                    }
                }

            }
        }

    }
}
