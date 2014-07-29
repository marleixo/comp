using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.IO;
using System.Data;
using DMExport.Library.Entities;
using DMExport.Library.Helpers;
using DMExport.Library.Services;
using DMExport.Library.Services.Impl;
using Microsoft.SharePoint;
using Microsoft.Office.Project.Server.Library;
using Microsoft.SharePoint.Deployment;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Workflow;

using DMExport.Library.WorkflowService;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.LookupTableService;

using WebPart = Microsoft.SharePoint.WebPartPages.WebPart;
using System.Threading;

namespace DMExport.Library
{
    public class TreeBuilder
    {
        #region Members

        private readonly IPSIService _psiService;
        private readonly IDependencyService _dependencyService;
        private readonly IDataService _dataService;
        
        public BackgroundWorker BackgroundWorker { get; set; }
        public ProgressBarReporter ProgressBarReporter { get; set; }

        #region Load and Export Messages

        /// <summary>
        /// Load Tree Messages
        /// </summary>
        public IList<string> LoadDMMessages = new List<string>()
        {
            "Updating the tree.",
            "Reading entities.",
            "Reading enterprise project types.",
            "Reading phases.",
            "Reading stages.",
            "Reading custom fields.",
            "Reading lookup tables.",
            "Filling enterprise project types.",
            "Filling phases.",
            "Filling stages.",
            "Filling project detail pages.",
            "Filling custom fields.",
            "Filling lookup tables.",
            "Updating treenodes' tags.",
            "The tree is updated.",
            String.Empty,
        };

        /// <summary>
        /// Export Tree Messages
        /// </summary>
        public IList<string> ExportDMMessages = new List<string>()
        {
            "Expoting data.",
            "Retrieving workflow associations.",
            "Getting enterprise project types to export.",
            "Getting phases to export.",
            "Getting stages to export.",
            "Getting custom fields to export.",
            "Getting lookup tables to export.",
            "Saving enterprise project types.",
            "Saving phases.",
            "Saving stages.",
            "Saving custom fields.",
            "Saving lookup tables.",
            "Saving project detail pages.",
            "Saving web parts.",
            "Saving feature.",
            "Saving entities file.",
            String.Empty
        };

        #endregion

        #endregion

        #region Constructors

        public TreeBuilder(IPSIService psiService, IDependencyService dependencyService, IDataService dataService)
        {
            _psiService = psiService;
            _dependencyService = dependencyService;
            _dataService = dataService;
        }

        #endregion

        #region Getting Services

        /// <summary>
        /// Gets Dependency Service.
        /// </summary>
        /// <returns>Dependency Service instance</returns>
        public IDependencyService GetDependencyService()
        {
            return _dependencyService;
        }

        /// <summary>
        /// Gets Data Service.
        /// </summary>
        /// <returns>Data Service instance</returns>
        public IDataService GetDataService()
        {
            return _dataService;
        }

        #endregion

        #region Progress Reporter

        /// <summary>
        /// Assign Progress Reporter Callback Method
        /// </summary>
        public void AssignProgressReporterCallback()
        {
            ProgressBarReporter.OnProgressChanged += RespondProgress;
        }

        /// <summary>
        /// Trigger for Progress Reporter.
        /// </summary>
        public void UpdateProgress()
        {
            ProgressBarReporter.ProgressUpdateProgress();
        }

        /// <summary>
        /// Handler for Progress Reporter.
        /// Responds up to GUI.
        /// </summary>
        /// <param name="progress">Progress data</param>
        private void RespondProgress(Progress progress)
        {
            BackgroundWorker.ReportProgress(progress.Percent, progress.Message);
            Application.DoEvents();
        }

        #endregion

        public void LoadTree(TreeView treeView)
        {
            treeView.Invoke((MethodInvoker)delegate()
            {
                UpdateProgress();
                
                treeView.BeginUpdate();
                WorkflowDataSet wfDataSet;

                UpdateProgress();
                //_dataService.AssignPDPs(_psiService.GetPDPList()); Callegario

                UpdateProgress();
                _dataService.AssignEPTs(_psiService.ReadEnterpriseProjectTypeList());

                UpdateProgress();
                _dataService.AssignPhases(_psiService.ReadWorkflowPhaseList());

                UpdateProgress();
                _dataService.AssignStages(_psiService.ReadWorkflowStageList());

                UpdateProgress();
                _dataService.AssignCustomFields(_psiService.ReadCustomFields());

                UpdateProgress();
                _dataService.AssignLookupTables(_psiService.ReadLookupTables());

                // Read data
                wfDataSet = _dataService.EnterpriseProjectTypes;
                wfDataSet.Merge(_dataService.WorkflowPhases);
                wfDataSet.Merge(_dataService.WorkflowStages);

                UpdateProgress();
                // Create the parent node for epts and add epts
                CreateParentNodeAndAddEpts(treeView, wfDataSet);

                UpdateProgress();
                // Create the parent node for phases and add phases
                CreateParentNodeAndAddPhases(treeView, wfDataSet);

                UpdateProgress();
                // Create the parent node for stages and add stages
                CreateParentNodeAndAddStages(treeView, wfDataSet);

                UpdateProgress();
                // Create the parent node for pdps and add pdps
                CreateParentNodeAndAddPdps(treeView, _dataService.ProjectDetailPages); 

                UpdateProgress();
                // Create the parent node for custom fields 
                // and add custom fields 
                CreateParentNodeAndAddCustomFields(treeView, _dataService.CustomFields);

                UpdateProgress();
                // Create the parent node for lookup tables 
                // and add lookup tables 
                CreateParentNodeAndAddLookupTables(treeView, _dataService.LookupTables);

                UpdateProgress();
                // Update EPT-dependent nodes
                _dataService.MakeDependencyGraph();

                UpdateProgress();
                // Set TreeNodes Tags
                SetTreeNodeTags(treeView);
                
                treeView.CheckBoxes = true;
                treeView.EndUpdate();

                UpdateProgress();
            });
        }

        private void SetTreeNodeTags(TreeView treeView)
        {
            _dataService.Dependencies
                .ForEach(dependency =>
                 {
                     treeView
                         .GetTreeNodeByName(dependency.Info.Uid.ToString())
                         .Tag = dependency;
                 });
        }

        #region Creating tree nodes

        /// <summary>
        /// Creates and adds EPT nodes to tree.
        /// Fills Dependency repository.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="wfDataSet">Workflow Data Set</param>
        public void CreateParentNodeAndAddEpts(TreeView treeView, WorkflowDataSet wfDataSet)
        {
            TreeNode epts =
                new TreeNode(EnumHelper.GetEnumDescription(EntityType.Ept));

            wfDataSet.EnterpriseProjectType.Rows
                .OfType<WorkflowDataSet.EnterpriseProjectTypeRow>()
                .OrderBy(ept => ept.ENTERPRISE_PROJECT_TYPE_NAME)
                .ForEach(ept =>
                {
                    _dataService.AddDependencyToStorage(
                        _dependencyService.GetEptDependencies(ept.ENTERPRISE_PROJECT_TYPE_UID));
                    
                    TreeNode eptNode = new TreeNode()
                    {
                        Text = ept.ENTERPRISE_PROJECT_TYPE_NAME,
                        Name = ept.ENTERPRISE_PROJECT_TYPE_UID.ToString(),
                    };

                    epts.Nodes.Add(eptNode);
                });

            treeView.Nodes.Add(epts);
        }

        /// <summary>
        /// Creates and add Phases to tree.
        /// Fills Dependency repository.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="wfDataSet">Workflow DataSet</param>
        public void CreateParentNodeAndAddPhases(TreeView treeView, WorkflowDataSet wfDataSet)
        {
            TreeNode phases =
                new TreeNode(EnumHelper.GetEnumDescription(EntityType.Phase));

            wfDataSet.WorkflowPhase.Rows
                .OfType<WorkflowDataSet.WorkflowPhaseRow>()
                .OrderBy(phase => phase.PHASE_NAME)
                .ForEach(phase =>
                {
                    _dataService.AddDependencyToStorage(
                        new Dependency(phase.PHASE_NAME, phase.PHASE_UID, EntityType.Phase));
                    
                    TreeNode phaseNode = new TreeNode
                    {
                        Text = phase.PHASE_NAME,
                        Name = phase.PHASE_UID.ToString(),
                    };

                    phases.Nodes.Add(phaseNode);
                });

            treeView.Nodes.Add(phases);
        }
        
        /// <summary>
        /// Creates and adds Stage to tree.
        /// Fills Dependency repository.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="wfDataSet">Workflow DataSet</param>
        public void CreateParentNodeAndAddStages(TreeView treeView, WorkflowDataSet wfDataSet)
        {
            TreeNode stages =
                new TreeNode(EnumHelper.GetEnumDescription(EntityType.Stage));

            wfDataSet.WorkflowStage.Rows
                .OfType<WorkflowDataSet.WorkflowStageRow>()
                .OrderBy(stage => stage.STAGE_NAME)
                .ForEach(stage =>
                {
                    _dataService.AddDependencyToStorage(
                        _dependencyService.GetStageDependencies(stage.STAGE_UID));
                    
                    TreeNode stageNode = new TreeNode()
                    {
                        Name = stage.STAGE_UID.ToString(),
                        Text = stage.STAGE_NAME,
                    };

                    stages.Nodes.Add(stageNode);
                });

            treeView.Nodes.Add(stages);
        }

        /// <summary>
        /// Creates and adds PDP to tree.
        /// Fills Dependency repository.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="pdpList">List of PDPs</param>
        public void CreateParentNodeAndAddPdps(TreeView treeView, ISPList pdpList)
        {
            TreeNode pdps = new TreeNode(EnumHelper.GetEnumDescription(EntityType.ProjectDetailPage));

            //pdpList.Items
            //    .OrderBy(li => li["TitlePDP"].ToString())
            //    .ForEach(li =>
            //    {
            //        _dataService.AddDependencyToStorage(
            //            _dependencyService.GetPdpDependencies(li.UniqueId));
                    
            //        TreeNode pdp = new TreeNode
            //        {
            //            Name = li.UniqueId.ToString(),
            //            Text = li["TitlePDP"].ToString(),
            //        };

            //        pdps.Nodes.Add(pdp);
            //    });

            treeView.Nodes.Add(pdps);
        }
        
        /// <summary>
        /// Creates and adds Custom Field to tree.
        /// Fills Dependency repository.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="cfDataSet">CustomField DataSet</param>
        public void CreateParentNodeAndAddCustomFields(TreeView treeView, CustomFieldDataSet cfDataSet)
        {
            TreeNode cfs =
                new TreeNode(EnumHelper.GetEnumDescription(EntityType.CustomField));

            cfDataSet.CustomFields.Rows
                .OfType<CustomFieldDataSet.CustomFieldsRow>()
                .Where(cf => (cf.MD_PROP_NAME.ToLower() != "project impact") && (cf.MD_PROP_NAME.ToLower() != "relative importance"))
                .OrderBy(cf => cf.MD_PROP_NAME)
                .ForEach(cf =>
                {
                    _dataService.AddDependencyToStorage(
                        _dependencyService.GetCustomFieldDependencies(cf.MD_PROP_UID));
                    
                    TreeNode cfNode = new TreeNode
                    {
                        Name = cf.MD_PROP_UID.ToString(),
                        Text = cf.MD_PROP_NAME
                    };
                    
                    cfs.Nodes.Add(cfNode);
                });

            treeView.Nodes.Add(cfs);
        }
        
        /// <summary>
        /// Creates and adds LookupTable to tree.
        /// Fills Dependency repository.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="ltDataSet">LookupTable DataSet</param>
        public void CreateParentNodeAndAddLookupTables(TreeView treeView, LookupTableDataSet ltDataSet)
        {
            TreeNode lts =
                new TreeNode(EnumHelper.GetEnumDescription(EntityType.LookupTable));

            ltDataSet.LookupTables.Rows
                .OfType<LookupTableDataSet.LookupTablesRow>()
                .Where(lt => (lt.LT_NAME.ToLower() != "project impact") && (lt.LT_NAME.ToLower() != "relative importance"))
                .OrderBy(lt => lt.LT_NAME)
                .ForEach(lt =>
                {
                    _dataService.AddDependencyToStorage(
                        new Dependency(lt.LT_NAME, lt.LT_UID, EntityType.LookupTable));
                    
                    TreeNode ltNode = new TreeNode
                    {
                        Name = lt.LT_UID.ToString(),
                        Text = lt.LT_NAME
                    };
                    
                    lts.Nodes.Add(ltNode);
                });

            treeView.Nodes.Add(lts);
        }

        #endregion

        /// <summary>
        /// Exports the pdps with all their properties
        /// </summary>
        public void ExportPdps(TreeView treeView, string fileLocation)
        {
            UpdateProgress();
            
            ISPList pdpList = _psiService.GetPDPList();
            if (pdpList == null)
            {
                return;
            }

            // Export the site collection
            TreeNode pdpNodes = treeView.Nodes[(int)EntityType.ProjectDetailPage];

            List<SPExportObject> exportObjects = GetPdpsToExport(pdpNodes, pdpList.ID);
            if (!exportObjects.Any())
            {
                return;
            }
            
            SPExportSettings settings = new SPExportSettings();

            exportObjects
                .ForEach(exportObject => settings.ExportObjects.Add(exportObject));

            settings.SiteUrl = _psiService.ServerUrl;
            settings.OverwriteExistingDataFile = true;
            settings.FileLocation = fileLocation;
            settings.BaseFileName = "SiteCollection";
            settings.FileCompression = true;

            SPExport export = new SPExport(settings);
            try
            {
                export.Run();
            }
            catch
            {
            }
        }

        public List<SPExportObject> GetPdpsToExport(TreeNode pdpRootNode, Guid pdpListUid)
        {
            List<SPExportObject> pdpsToExport = new List<SPExportObject>();
            pdpRootNode.Nodes
                .OfType<TreeNode>()
                .Where(node => node.Checked)
                .ForEach(node =>
                {
                    SPExportObject exportObject =
                        new SPExportObject(new Guid(node.Name), SPDeploymentObjectType.File, pdpListUid, false);

                    pdpsToExport.Add(exportObject);
                });

            return pdpsToExport;
        }

        public List<TreeNode> GetPdpNodesToExport(TreeNode pdpRootNode, Guid pdpListUid)
        {
            return pdpRootNode.Nodes
                .OfType<TreeNode>()
                .Where(node => node.Checked)
                .ToList();
        }

        /// <summary>
        /// Exports epts, stages, phases, cfs, lts
        /// </summary>
        public void ExportFromTree(TreeView treeView, string fileLocation)
        {
            UpdateProgress();

            UpdateProgress();
            // Get epts and foreach selected ept read it
            SPWorkflowAssociationCollection associations; 
            //using (SPSite site = new SPSite(_psiService.ServerUrl)) Callegario
            //{
            //    using (SPWeb web = site.OpenWeb())
            //    {
            //        associations = web.WorkflowAssociations;
            //    }
            //}

            UpdateProgress();
            TreeNode eptNodes = treeView.Nodes[(int)EntityType.Ept];            
            WorkflowDataSet eptDataSet = GetEptsToExport(eptNodes);//Callegario , new SPWorkflowAssociationCollectionAdapert(associations));

            UpdateProgress();
            // Get phases and foreach selected phase read it
            TreeNode phasesNodes = treeView.Nodes[(int)EntityType.Phase];
            WorkflowDataSet phDataSet = GetPhasesToExport(phasesNodes);

            UpdateProgress();
            // Get stages and foreach selected stage read it
            TreeNode stagesNodes = treeView.Nodes[(int)EntityType.Stage];
            WorkflowDataSet stDataSet = GetStagesToExport(stagesNodes);

            UpdateProgress();
            // Read custom fields
            TreeNode cfsNodes =
                treeView.Nodes[(int)EntityType.CustomField];
            CustomFieldDataSet cfDataSet = GetCustomFieldsToExport(cfsNodes);

            UpdateProgress();
            // Read lookup tables
            TreeNode ltsNodes =
                treeView.Nodes[(int)EntityType.LookupTable];
            LookupTableDataSet ltDataSet = GetLookupTablesToExport(ltsNodes);

            try
            {
                UpdateProgress();
                stDataSet.WriteXml(
                    Path.Combine(fileLocation,
                        "WorkflowStages.xml"),
                    XmlWriteMode.WriteSchema);

                UpdateProgress();
                phDataSet.WriteXml(
                    Path.Combine(fileLocation,
                        "WorkflowPhases.xml"),
                    XmlWriteMode.WriteSchema);

                UpdateProgress();
                eptDataSet.WriteXml(
                    Path.Combine(fileLocation,
                        "WorkflowEpts.xml"),
                    XmlWriteMode.WriteSchema);

                UpdateProgress();
                cfDataSet.WriteXml(
                    Path.Combine(fileLocation,
                        "CustomFields.xml"),
                    XmlWriteMode.WriteSchema);

                UpdateProgress();
                ltDataSet.WriteXml(
                    Path.Combine(fileLocation,
                        "LookupTables.xml"),
                    XmlWriteMode.WriteSchema);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Exports PDP webparts
        /// </summary>
        /// <param name="treeView">Tree view</param>
        public void ExportWebParts(TreeView treeView, string fileLocation)
        {
            UpdateProgress();
            
            XDocument xDocument = new XDocument();
            XElement xPageWebParts = new XElement("PageWebParts");
            
            ISPList spList = _psiService.GetPDPList();
            if (spList != null)
            {
                List<TreeNode> pdpNodes = GetPdpNodesToExport(treeView.Nodes[(int)EntityType.ProjectDetailPage], spList.ID);
            
                foreach (TreeNode pdpNode in pdpNodes)
                {
                    Guid pdpGuid = new Guid(pdpNode.Name);
                    string pdpName = pdpNode.Text;

                    using (SPSite spSite = new SPSite(_psiService.ServerUrl))
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            // Try to get PDP spFile
                            SPFile spFile = spWeb.GetFile(pdpGuid);
                            if (spFile == null)
                            {
                                continue;
                            }

                            using (SPLimitedWebPartManager manager =
                                spFile.GetLimitedWebPartManager(PersonalizationScope.Shared))
                            {
                                foreach (WebPart webPart in manager.WebParts)
                                {
                                    // Export webPart

                                    using (MemoryStream memoryStream = new MemoryStream())
                                    {
                                        using (XmlTextWriter xmlTextWriter = 
                                            new XmlTextWriter(memoryStream, Encoding.Default))
                                        {
                                            manager.ExportWebPart(webPart, xmlTextWriter);
                                            xmlTextWriter.Flush();

                                            XElement xPageWebPart = new XElement("PageWebPart");
                                            XElement xWebPart = XElement.Parse(
                                                Encoding.Default.GetString(memoryStream.ToArray()));

                                            xPageWebPart.Add(xWebPart);

                                            XAttribute pageName = new XAttribute("PageName", pdpName);
                                            XAttribute webPartTitle = new XAttribute("WebPartTitle", webPart.Title);
                                            XAttribute zoneIndex = new XAttribute("ZoneIdx", webPart.ZoneIndex);
                                            XAttribute zoneId = new XAttribute("ZoneId", 
                                                String.IsNullOrEmpty(webPart.ZoneID) 
                                                    ? string.Empty 
                                                    : webPart.ZoneID);

                                            xPageWebPart.Add(webPartTitle);
                                            xPageWebPart.Add(pageName);
                                            xPageWebPart.Add(zoneId);
                                            xPageWebPart.Add(zoneIndex);

                                            xPageWebParts.Add(xPageWebPart);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            xDocument.Add(xPageWebParts);
            xDocument.Save(Path.Combine(fileLocation, "WebParts.xml"));
        }

        public LookupTableDataSet GetLookupTablesToExport(TreeNode lookupTablesRootNodes)
        {
            LookupTableDataSet ltDataSet = new LookupTableDataSet();
            List<Guid> ltsGuids = new List<Guid>();

            lookupTablesRootNodes.Nodes
                .OfType<TreeNode>()
                .Where(node => node.Checked)
                .ForEach(node => ltsGuids.Add(new Guid(node.Name)));

            ltDataSet.Merge(_psiService.ReadLookupTablesByUids(ltsGuids.ToArray()));
            return ltDataSet;
        }

        public CustomFieldDataSet GetCustomFieldsToExport(TreeNode cfsNodes)
        {
            CustomFieldDataSet cfDataSet = new CustomFieldDataSet();
            List<Guid> cfGuids = new List<Guid>();

            cfsNodes.Nodes
                .OfType<TreeNode>()
                .Where(node => node.Checked)
                .ForEach(node => cfGuids.Add(new Guid(node.Name)));

            cfDataSet.Merge(_psiService.ReadCustomFieldsByMdPropUids2(cfGuids.ToArray()));
            return cfDataSet;
        }

        public WorkflowDataSet GetStagesToExport(TreeNode stagesRootNode)
        {
            WorkflowDataSet stDataSet = new WorkflowDataSet();
            stagesRootNode.Nodes
                .OfType<TreeNode>()
                .Where(node => node.Checked)
                .ForEach(node => stDataSet.Merge(
                                     _psiService.ReadWorkflowStage(
                                         new Guid(node.Name))));
            return stDataSet;
        }

        public WorkflowDataSet GetPhasesToExport(TreeNode phasesRootNode)
        {
            WorkflowDataSet phDataSet = new WorkflowDataSet();
            phasesRootNode.Nodes
                .OfType<TreeNode>()
                .Where(node => node.Checked)
                .ForEach(node => phDataSet.Merge(
                                     _psiService.ReadWorkflowPhase(
                                         new Guid(node.Name))));
            return phDataSet;
        }

        public WorkflowDataSet GetEptsToExport(TreeNode eptRootNode) //Callegario, ISPWorkflowAssociationCollection associations)
        {
            WorkflowDataSet eptDataSet = new WorkflowDataSet();
            eptRootNode.Nodes
                .OfType<TreeNode>()
                .Where(node => node.Checked)
                .ForEach(node =>
                {
                    eptDataSet.Merge(
                        _psiService.ReadEnterpriseProjectType(
                            new Guid(node.Name)));

                    // Get the wf template name and store 
                    // in ept.wf_association_name 
                    // (wfassociationame/wftemplatename)
                //    eptDataSet.EnterpriseProjectType.Rows  Callegario
                //        .OfType<WorkflowDataSet.EnterpriseProjectTypeRow>()
                //        .Where(ept => !ept.IsWORKFLOW_ASSOCIATION_UIDNull())
                //        .ToList()
                //        .ForEach(ept =>
                //        {
                //            ISPWorkflowAssociation association =
                //                associations[ept.WORKFLOW_ASSOCIATION_UID];

                //            ept.WORKFLOW_ASSOCIATION_NAME +=
                //                string.Format("/{0}",
                //                    association.BaseTemplate.Name);
                //        });
                });

            return eptDataSet;
        }
    }
}
