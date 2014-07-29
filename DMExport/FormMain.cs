using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using DMExport.Library;
using DMExport.Library.Entities;
using DMExport.Library.Helpers;
using DMExport.Library.Services;
using DMExport.Library.Services.Impl;

namespace DMExport
{
    public partial class FormMain : Form
    {
        #region Private Fields

        private TreeBuilder _treeBuilder;
        private string _serverUrl;

        #endregion

        #region Constructors

        public FormMain(string serverUrl)
        {
            _serverUrl = serverUrl;

            InitializeComponent();
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Event Handler after node is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            treeView.AfterCheck -= treeView_AfterCheck;

            bool isChecked = e.Node.Checked;

            // If is a child node then modify parent selection
            if (e.Node.Parent != null)
            {
                // If node is checked then check the parent
                if (isChecked)
                {
                    e.Node.Parent.Checked = true;

                    // Check dependent nodes
                    if (cbEnableAutoComplete.Checked)
                    {
                        ProcessNodeOnChecked(e.Node);
                    }
                }
                else
                {
                    // The node is unchecked: uncheck the parent
                    // Search for other checked nodes having the same parent; 
                    // If at least one exist then check the parent
                    e.Node.Parent.Checked = false;

                    for (int i = 0; i < e.Node.Parent.Nodes.Count; i++)
                    {
                        if (e.Node.Parent.Nodes[i].Checked)
                        {
                            e.Node.Parent.Checked = true;
                            break;
                        }
                    }
                }
            }
            // This is parent node
            else
            {
                // This is parent node
                for (int i = 0; i < e.Node.Nodes.Count; i++)
                {
                    e.Node.Nodes[i].Checked = e.Node.Checked;

                    if (e.Node.Checked && cbEnableAutoComplete.Checked)
                    {
                        ProcessNodeOnChecked(e.Node.Nodes[i]);
                    }
                }
            }

            treeView.AfterCheck += treeView_AfterCheck;
        }

        /// <summary>
        /// Browse button clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browse = new FolderBrowserDialog();
            DialogResult result = browse.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtExportLocation.Text = browse.SelectedPath;
            }
        }

        /// <summary>
        /// Export button clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtExportLocation.Text))
            {
                MessageBox.Show("Select the export location");
                return;
            }

            //if (string.IsNullOrEmpty(txtFeatureName.Text))
            //{
            //    MessageBox.Show("Introduce the feature name");
            //    return;
            //}

            EnableFormControls(false);

            backgroundWorkerExport = new BackgroundWorker();
            backgroundWorkerExport.DoWork += BackgroundWorkerExportDoExport;
            backgroundWorkerExport.RunWorkerCompleted += BackgroundWorkerExportExportCompleted;
            backgroundWorkerExport.ProgressChanged += BackgroundWorkerLoadProgressChanged;
            backgroundWorkerExport.WorkerReportsProgress = true;

            Cursor = Cursors.WaitCursor;
            lblStatus.Text = "The selected entities are being exported... Please wait until completed.";

            backgroundWorkerExport.RunWorkerAsync();
        }

        /// <summary>
        /// Cancel button clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Saves selected items to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveSelection_Click(object sender, EventArgs e)
        {
            // Get tree selected nodes
            var treeSelection = treeView.GetSelectedNodes();

            // Save to File
            XDocument xDocument = new XDocument();
            xDocument.Document.Add(new XElement("Root",
                new XElement(EntityType.Ept.ToString(), treeSelection.SelectedEpts
                    .Select(item => GetSaveItemNode(item))
                    .ToArray()),
                new XElement(EntityType.Phase.ToString(), treeSelection.SelectedPhases
                    .Select(item => GetSaveItemNode(item))
                    .ToArray()),
                new XElement(EntityType.Stage.ToString(), treeSelection.SelectedStages
                    .Select(item => GetSaveItemNode(item))
                    .ToArray()),
                new XElement(EntityType.CustomField.ToString(), treeSelection.SelectedCustomFields
                    .Select(item => GetSaveItemNode(item))
                    .ToArray()),
                new XElement(EntityType.LookupTable.ToString(), treeSelection.SelectedLookupTables
                    .Select(item => GetSaveItemNode(item))
                    .ToArray()),
                new XElement(EntityType.ProjectDetailPage.ToString(), treeSelection.SelectedPdps
                    .Select(item => GetSaveItemNode(item))
                    .ToArray())));

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DMExport files (*.dme)|*.dme";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var stream = saveFileDialog.OpenFile())
                {
                    using (TextWriter textWriter = new StreamWriter(stream))
                    {
                        xDocument.Save(textWriter);
                    }
                }
            }
        }

        /// <summary>
        /// Loads selected items from file to Tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadSelection_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "DMExport files (*.dme)|*.dme";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var stream = openFileDialog.OpenFile())
                {
                    if (stream != null)
                    {
                        using (TextReader textReader = new StreamReader(stream))
                        {
                            try
                            {
                                XDocument xDocument = XDocument.Load(textReader);
                                var treeSelection = new TreeHelper.TreeSelection();

                                xDocument.Root.Nodes()
                                    .ToList()
                                    .ForEach(parent =>
                                    {
                                        string nodeName = ((XElement)parent).Name.LocalName;
                                        EntityType entityType = (EntityType)Enum.Parse(typeof(EntityType), nodeName);

                                        switch (entityType)
                                        {
                                            case EntityType.Ept:
                                                treeSelection.SelectedEpts = FillSelectedNodesCollectionByGroup(parent as XElement, EntityType.Ept);
                                                break;
                                            case EntityType.Phase:
                                                treeSelection.SelectedPhases = FillSelectedNodesCollectionByGroup(parent as XElement, EntityType.Phase);
                                                break;
                                            case EntityType.Stage:
                                                treeSelection.SelectedStages = FillSelectedNodesCollectionByGroup(parent as XElement, EntityType.Stage);
                                                break;
                                            case EntityType.CustomField:
                                                treeSelection.SelectedCustomFields = FillSelectedNodesCollectionByGroup(parent as XElement, EntityType.CustomField);
                                                break;
                                            case EntityType.LookupTable:
                                                treeSelection.SelectedLookupTables = FillSelectedNodesCollectionByGroup(parent as XElement, EntityType.LookupTable);
                                                break;
                                            case EntityType.ProjectDetailPage:
                                                treeSelection.SelectedPdps = FillSelectedNodesCollectionByGroup(parent as XElement, EntityType.ProjectDetailPage);
                                                break;
                                        }
                                    });

                                bool enableAutoSelect = cbEnableAutoComplete.Checked;
                                cbEnableAutoComplete.Checked = false;
                                
                                treeView.DoSelectTreeNodes(treeSelection);

                                cbEnableAutoComplete.Checked = enableAutoSelect;
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show("Error importing selection." + "\n" + exception.Message,
                                    "Import Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Form Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            EnableFormControls(false);

            backgroundWorkerLoad.DoWork += BackgroundWorkerLoadDoLoad;
            backgroundWorkerLoad.RunWorkerCompleted += BackgroundWorkerLoadLoadCompleted;
            backgroundWorkerLoad.ProgressChanged += BackgroundWorkerLoadProgressChanged;
            backgroundWorkerLoad.WorkerReportsProgress = true;

            Cursor = Cursors.WaitCursor;
            lblStatus.Text = "The tree is loading... Please wait until completed.";

            backgroundWorkerLoad.RunWorkerAsync();
        }
        
        /// <summary>
        /// Form Closed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Closed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region BackgroundWorker Load Events

        private void BackgroundWorkerLoadDoLoad(object sender, DoWorkEventArgs e)
        {
            try
            {
                LoadDM();
            }
            catch (Exception exception)
            {
                e.Result = exception;
            }
        }

        private void BackgroundWorkerLoadLoadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatus.Text = "The tree is loaded.";
            Cursor = Cursors.Default;

            if (e.Result is Exception)
            {
                MessageBox.Show((e.Result as Exception).Message);
            }

            EnableFormControls(true);
        }

        private void BackgroundWorkerLoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetImidiateStatus(e.ProgressPercentage, (string)e.UserState);
        }

        #endregion

        #region BackgroundWorker Export Events

        private void BackgroundWorkerExportDoExport(object sender, DoWorkEventArgs e)
        {
            ExportDM();
        }

        private void BackgroundWorkerExportExportCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            EnableFormControls(true);

            lblStatus.Text = "Export completed.";
            lblImidiateStatus.Text = String.Empty;

            if (_treeBuilder.GetDataService().ExportWarningItems.Any())
            {
                // Show Warnings Window
                new FormResults(_treeBuilder.GetDataService()).ShowDialog();
            }
            else
            {
                MessageBox.Show("Completed");
            }
            Application.Exit();
        }

        #endregion

        /// <summary>
        /// Fills out a tree of entities
        /// </summary>
        private void LoadDM()
        {
            IDataService dataService = new DataService();
            IPSIService psiService = new PSIService(_serverUrl);
            IDependencyService dependencyService = new DependencyService(dataService, psiService);

            _treeBuilder = new TreeBuilder(psiService, dependencyService, dataService);
            _treeBuilder.BackgroundWorker = backgroundWorkerLoad;

            _treeBuilder.ProgressBarReporter = new ProgressBarReporter(_treeBuilder.LoadDMMessages, 0, 100);
            _treeBuilder.AssignProgressReporterCallback();
            
            _treeBuilder.LoadTree(treeView);
        }

        /// <summary>
        /// Performs Export of Entities
        /// </summary>
        private void ExportDM()
        {
            _treeBuilder.BackgroundWorker = backgroundWorkerExport;

            _treeBuilder.ProgressBarReporter = new ProgressBarReporter(_treeBuilder.ExportDMMessages, 0, 100);
            _treeBuilder.AssignProgressReporterCallback();
            
            // Try get export warnings
            _treeBuilder.GetDependencyService().CheckDependenciesOnExport(treeView);

            _treeBuilder.ExportFromTree(treeView, txtExportLocation.Text);
            //_treeBuilder.ExportPdps(treeView, txtExportLocation.Text); Callegario
            //_treeBuilder.ExportWebParts(treeView, txtExportLocation.Text); Callegario

            //_treeBuilder.ProgressBarReporter.ProgressUpdateProgress();
            //CreateFeature();

            //_treeBuilder.ProgressBarReporter.ProgressUpdateProgress();
            //CreateEntities();

            _treeBuilder.ProgressBarReporter.ProgressUpdateProgress();
        }

        /// <summary>
        /// Creates a Feature File
        /// </summary>
        private void CreateFeature()
        {
            string featureXml = string.Format(
            @"<?xml version=""1.0"" encoding=""utf-8""?>
                 <Feature Id=""{0}""
                          Title=""ImportDMFeature""
                          Description=""ImportDMFeature description""
                          Version=""14.0.0.0""
                          Scope=""Site""
                          ImageUrl=""P14SDK\P14SDK.gif""
                          ReceiverAssembly=""Microsoft.SDK.Project.Samples.ImportDemandManagement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=727d2570b57e8443""
                          ReceiverClass=""Microsoft.SDK.Project.Samples.ImportDemandManagement.FeatureReceiver""
                          xmlns=""http://schemas.microsoft.com/sharepoint/"">
                    <Properties>
                        <Property Key=""GloballyAvailable"" Value=""true"" />
                        <Property Key=""RegisterForms"" Value=""*.xsn"" />
                    </Properties>
                </Feature>", Guid.NewGuid());

            FileStream file = null;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(featureXml);
                string filePath = Path.Combine(txtExportLocation.Text,
                    "FeatureTemplate.xml");

                file = new FileStream(filePath, FileMode.Create,
                    FileAccess.Write);

                file.Write(data, 0, data.Length);
            }
            catch
            {
            }
            finally
            {
                file.Close();
            }
        }

        /// <summary>
        /// Creates entities file
        /// </summary>
        private void CreateEntities()
        {
            var xDocument = new XDocument();
            var featureNameNode = new XElement("Feature");
            var featureNameAttribute = new XAttribute("Name", txtFeatureName.Text.Trim());
            featureNameNode.Add(featureNameAttribute);

            if (xDocument.Document == null)
            {
                return;
            }

            xDocument.Document.Add(featureNameNode);
            xDocument.Save(Path.Combine(txtExportLocation.Text, "Entities.xml"));
        }

        /// <summary>
        /// Sets Progress Bar status
        /// </summary>
        /// <param name="percentage">Percents</param>
        /// <param name="message">Message</param>
        private void SetImidiateStatus(int percentage, string message)
        {
            progressBar.Value = percentage;

            lblImidiateStatus.Text = String.Format("{0}%. {1}", percentage, message);
            if (percentage == 100)
            {
                lblImidiateStatus.Text = String.Empty;
            }
        }

        /// <summary>
        /// When node is checked process all dependencies
        /// </summary>
        /// <param name="node">TreeNode to process</param>
        private void ProcessNodeOnChecked(TreeNode node)
        {
            if (node.Tag == null)
            {
                return;
            }

            var dependency = (Dependency)node.Tag;
            if (dependency == null)
            {
                return;
            }

            ProcessTreeViewNode(dependency);
        }

        /// <summary>
        /// Processes TreeNode group.
        /// Checks or unchecked certain treeview nodes
        /// </summary>
        /// <param name="dependency">Dependency Info</param>
        private void ProcessTreeViewNode(Dependency dependency)
        {
            if (dependency == null)
            {
                return;
            }

            dependency.Dependencies
                .ToList()
                .ForEach(dep =>
                {
                    var node = treeView.GetTreeNodeByName(dep.Info.Uid.ToString());
                    if (node == null)
                    {
                        return;
                    }
                     
                    node.Checked = true;
                    node.Parent.Checked = true;

                    ProcessNodeOnChecked(node);
                });
        }

        /// <summary>
        /// Enables or disables some form controls
        /// </summary>
        /// <param name="enable">enable for true</param>
        private void EnableFormControls(bool enable)
        {
            //txtFeatureName.Enabled = enable;
            btnExport.Enabled = enable;
            btnCancel.Enabled = enable;
            btnBrowse.Enabled = enable;
            cbEnableAutoComplete.Enabled = enable;
            treeView.Enabled = enable;
        }

        /// <summary>
        /// Gets "Info" XML node for save operation
        /// </summary>
        /// <param name="treeNode">TreeNode</param>
        /// <returns>XML Element</returns>
        private XElement GetSaveItemNode(TreeNode treeNode)
        {
            var xNode = new XElement("Info");
            xNode.Add(new XAttribute("Name", treeNode.Text));
            xNode.Add(new XAttribute("Uid", treeNode.Name));
            return xNode;
        }

        /// <summary>
        /// Calcualtes selected nodes by Group
        /// </summary>
        /// <param name="parentXmlNode">XML Node</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Collection of TreeNodes</returns>
        private List<TreeNode> FillSelectedNodesCollectionByGroup(XElement parentXmlNode, EntityType entityType)
        {
            List<TreeNode> destinationNodeCollection = new List<TreeNode>();
            
            foreach (XNode xNode in ((XElement)parentXmlNode).Nodes())
            {
                XElement xElement = ((XElement) xNode);
                if (xElement == null)
                {
                    continue;
                }

                XAttribute uidAttribute = xElement.Attribute("Uid");
                if (uidAttribute == null)
                {
                    continue;
                }

                TreeNode foundTreeNode = treeView.GetTreeNodeByName(uidAttribute.Value);
                if (foundTreeNode == null)
                {
                    XAttribute nameAttribute = xElement.Attribute("Name");
                    if (nameAttribute == null)
                    {
                        continue;
                    }

                    foundTreeNode = treeView.GetTreeNodeByText(nameAttribute.Value, entityType);
                    if (foundTreeNode != null)
                    {
                        destinationNodeCollection.Add(foundTreeNode);
                    }
                }
                else
                {
                    destinationNodeCollection.Add(foundTreeNode);
                }
            }

            return destinationNodeCollection;
        }
    }
}