﻿using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using VSLangProj;
using Task = System.Threading.Tasks.Task;

namespace NBKVSExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class cmdInternalReference
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("c43d508d-7ee1-4c0d-a41d-ae434f3d4702");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private static IVsThreadedWaitDialog4 twd;

        /// <summary>
        /// Initializes a new instance of the <see cref="cmdInternalReference"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private cmdInternalReference(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            CommandID menuCommandID = new(CommandSet, CommandId);
            MenuCommand menuItem = new(this.Execute, menuCommandID);
            
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static cmdInternalReference Instance
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
            // Switch to the main thread - the call to AddCommand in cmdInternalReference's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new cmdInternalReference(package, commandService);

            //Create thread wait dialog to display progress bar if it takes long time
            var twdFactory = (IVsThreadedWaitDialogFactory)await Community.VisualStudio.Toolkit.VS.Services.GetThreadedWaitDialogAsync();
            twd = twdFactory.CreateInstance();

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
            string message = "Internal references are added successfully.";
            string title = "Internal Reference Tool";
            int totalProjects = 0;
            int currentProjectIndex = 0;

            DTE DTE = Package.GetGlobalService(typeof(DTE)) as DTE;
            Solution pSolution = DTE.Solution;

            twd.StartWaitDialog(title, "Reading projects in solution...", "", null, "Processing projects...", 1, true, true);

            try
            {
                //Build list of all the projects
                List<VSProject> projects = (
                    from Project project in pSolution.Projects
                    where project.Object != null && project.Object is VSProject
                    select project.Object as VSProject
                    ).ToList();

                //Check dependency for each of the projects and change references
                totalProjects = projects.Count;

                if (totalProjects == 0)
                {
                    message = "No Projects loaded.";
                }

                foreach (VSProject project in projects)
                {
                    var text = $"Project {++currentProjectIndex}/{totalProjects}";
                    twd.UpdateProgress($"Processing project {project.Project.Name}", text, text, currentProjectIndex, totalProjects, true, out _);

                    foreach (VSProject refProject in projects)
                    {
                        Reference reference = project.References.Find(refProject.Project.Name);
                        if (refProject != project && reference is not null)
                        {
                            reference.Remove();
                            project.References.AddProject(refProject.Project);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            twd.EndWaitDialog();
            (twd as IDisposable).Dispose();

            VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
