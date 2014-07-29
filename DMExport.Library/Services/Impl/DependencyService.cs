using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.Entities;
using DMExport.Library.Helpers;
using DMExport.Library.WorkflowService;

namespace DMExport.Library.Services.Impl
{
    public class DependencyService : IDependencyService
    {
        #region Fields and Properties

        private readonly IDataService _dataStorage;
        private readonly IEptAssociationService _eptAssociationService;

        #endregion

        #region Constructors

        public DependencyService(IDataService dataStorage, IPSIService psiService)
        {
            _dataStorage = dataStorage;
            _eptAssociationService = new EptAssociationService(psiService.ServerUrl); 
        }

        #endregion

        /// <summary>
        /// Gets dependency for a stage
        /// </summary>
        /// <param name="stageUid">Workflow Stage UID</param>
        /// <returns>Dependency instance</returns>
        public Dependency GetStageDependencies(Guid stageUid)
        {
            var stageRow = _dataStorage.WorkflowStages.WorkflowStage.FindBySTAGE_UID(stageUid);
            if (stageRow == null)
            {
                return null;
            }

            var dependency = new Dependency(stageRow.STAGE_NAME, stageUid, EntityType.Stage);
            
            var phases = new List<Dependency>
                             {
                                 _dataStorage.WorkflowStages.WorkflowStage.Rows
                                     .Cast<WorkflowDataSet.WorkflowStageRow>()
                                     .Where(item => item.STAGE_UID == stageUid)
                                     .Select(item => new Dependency(item.PHASE_NAME, item.PHASE_UID, EntityType.Phase))
                                     .First()
                             };

            var customFields = stageRow
                .GetWorkflowStageCustomFieldsRows()
                .Select(cf => new Dependency(cf.MD_PROP_NAME, cf.MD_PROP_UID, EntityType.CustomField));

            ApplyCustomFieldsDependencies(customFields);

            var pdps = stageRow
                .GetWorkflowStagePDPsRows()
                .Select(pdp => new Dependency(pdp.PDP_NAME, pdp.PDP_UID, EntityType.ProjectDetailPage));

            //ApplyPdpsDependencies(pdps); Callegario

            dependency.AddDependencies(phases.Union(customFields).Union(pdps));
            return dependency;
        }

        /// <summary>
        /// Gets Custom Field Dependency
        /// </summary>
        /// <param name="cfUid">Custom Field UID</param>
        /// <returns>Dependency instance</returns>
        public Dependency GetCustomFieldDependencies(Guid cfUid)
        {
            var customFieldRow = _dataStorage.CustomFields.CustomFields.FindByMD_PROP_UID(cfUid);
            if (customFieldRow == null)
            {
                return null;
            }

            var dependency = new Dependency(customFieldRow.MD_PROP_NAME, customFieldRow.MD_PROP_UID, EntityType.CustomField);
            return ApplyCustomFieldsDependencies(new List<Dependency> { dependency }).FirstOrDefault();
        }

        /// <summary>
        /// Gets dependency for a PDP
        /// </summary>
        /// <param name="pdpUid">PDP UID</param>
        /// <returns>Dependency instance</returns>
        public Dependency GetPdpDependencies(Guid pdpUid)
        {
            var pdpItem = _dataStorage.ProjectDetailPages.Items.Where(item => item.UniqueId == pdpUid).FirstOrDefault();
            if (pdpItem == null)
            {
                return null;
            }

            var dependency = new Dependency(pdpItem["TitlePDP"].ToString(), pdpItem.UniqueId, EntityType.ProjectDetailPage);
            return ApplyPdpsDependencies(new List<Dependency>() { dependency }).FirstOrDefault();
        }

        /// <summary>
        /// Gets dependency for a EPT
        /// </summary>
        /// <param name="eptUid">EPT UID</param>
        /// <returns>Dependency instance</returns>
        public Dependency GetEptDependencies(Guid eptUid)
        {
            var ept = _dataStorage.EnterpriseProjectTypes.EnterpriseProjectType.FindByENTERPRISE_PROJECT_TYPE_UID(eptUid);
            if (ept == null)
            {
                return null;
            }

            var dependency = new Dependency(ept.ENTERPRISE_PROJECT_TYPE_NAME, ept.ENTERPRISE_PROJECT_TYPE_UID, EntityType.Ept);

            if (!_eptAssociationService.IsEptAssociationToolInstalled)
            {
                if (ept.IsWORKFLOW_ASSOCIATION_UIDNull())
                {
                    //dependency.AddDependencies(ApplyPdpsDependencies( Callegario
                    //    ept.GetEnterpriseProjectTypePDPsRows()
                    //        .Select(item => new Dependency(item.PDP_NAME, item.PDP_UID, EntityType.ProjectDetailPage))));
                }
            }
            else
            {
                // Tool is installed
                // Check if EPT is workflow-controlled

                if (!_eptAssociationService.IsEptWorkflowControlled(eptUid))
                {
                    // EPT is NOT Workflow-controlled
                    dependency.AddDependencies(ApplyPdpsDependencies(
                        ept.GetEnterpriseProjectTypePDPsRows()
                            .Select(item => new Dependency(item.PDP_NAME, item.PDP_UID, EntityType.ProjectDetailPage)))); 
                }
                else
                {
                    ApplyCustomFieldsDependencies(
                        dependency
                            .Dependencies
                            .Where(item => item.Info.Type == EntityType.CustomField));
                    
                    // EPT is Workflow-controlled
                    // Get Data from EPTAssociationTool
                    dependency.AddDependencies(_eptAssociationService.GetWorkflowControlledEptDependencies(eptUid));

                    // Issue: if EPT is workflow-controlled in EPTAssociationTool
                    // but still isn't in PWA.
                    // Should add at least one pdp
                    if (ept.IsWORKFLOW_ASSOCIATION_UIDNull())
                    {
                        ISPListItem firstPpd = _dataStorage.ProjectDetailPages.Items.FirstOrDefault();
                        if (firstPpd != null)
                        {
                            dependency.AddDependency(
                                new Dependency(firstPpd["TitlePDP"].ToString(), firstPpd.UniqueId, EntityType.ProjectDetailPage));
                        }
                    }
                }
            }

            return dependency;
        }

        /// <summary>
        /// Fills a list of warnings during export.
        /// </summary>
        /// <param name="treeView">Treeview object</param>
        public void CheckDependenciesOnExport(TreeView treeView)
        {
            treeView.Nodes
                .Cast<TreeNode>()
                .ForEach(upperNode =>
                {
                    foreach (TreeNode node in from TreeNode node in upperNode.Nodes
                             where node.Checked
                             where node.Tag != null
                             where (node.Tag is Dependency)
                             select node)
                    {
                       _dataStorage
                           .AddExportWarningItems(
                                GetNodeUnselectedDependencies(treeView, node.Tag as Dependency));
                    }
                });
        }

        #region Private Methods

        /// <summary>
        /// Finds all dependencies for collection of custom fields
        /// </summary>
        /// <param name="customFieldsItems">Collection of Custom Fields</param>
        /// <returns>Collection of Custom Fields</returns>
        private IEnumerable<Dependency> ApplyCustomFieldsDependencies(IEnumerable<Dependency> customFieldsItems)
        {
            customFieldsItems
                .ForEach(cf =>
                {
                    var customFieldRow = _dataStorage.CustomFields.CustomFields.FindByMD_PROP_UID(cf.Info.Uid);
                    if (customFieldRow != null && !customFieldRow.IsMD_LOOKUP_TABLE_UIDNull())
                    {
                        var ltRow = _dataStorage.LookupTables
                            .LookupTables
                            .FindByLT_UID(customFieldRow.MD_LOOKUP_TABLE_UID);
                        
                        cf.AddDependencies(
                            new List<Dependency> { new Dependency(ltRow.LT_NAME, ltRow.LT_UID, EntityType.LookupTable) });
                     }
                });

            return customFieldsItems;
        }

        /// <summary>
        /// Finds all dependencies for collection of PDPs
        /// </summary>
        /// <param name="pdpsItems">Collection of PDPs</param>
        /// <returns>collection of PDPs</returns>
        private IEnumerable<Dependency> ApplyPdpsDependencies(IEnumerable<Dependency> pdpsItems)
        {
            pdpsItems
                .ForEach(pdpItem =>
                 {
                     var customFieldUids = _eptAssociationService.GetPdpCustomFieldsUids(pdpItem.Info.Uid);
                     
                     var customFields = _dataStorage.CustomFields.CustomFields.Rows
                         .Cast<CustomFieldDataSet.CustomFieldsRow>()
                         .Where(cfRow => customFieldUids.Contains(cfRow.MD_PROP_UID))
                         .Select(cfRow => new Dependency(cfRow.MD_PROP_NAME, cfRow.MD_PROP_UID, EntityType.CustomField));

                     pdpItem.AddDependencies(customFields);

                     ApplyCustomFieldsDependencies(customFields);
                 });
            
            return pdpsItems;
        }

        /// <summary>
        /// For Dependency get all non-checked nodes.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="dependency">Dependency</param>
        /// <returns>Collection of Dependency Info (Nodes)</returns>
        private IEnumerable<Dependency.DependencyInfo> GetNodeUnselectedDependencies(TreeView treeView, Dependency dependency)
        {
            var items = new List<Dependency.DependencyInfo>();
            if (dependency == null)
            {
                return items;
            }

            items.AddRange(GetNodeUnselectedDependencies(treeView, dependency.Dependencies));
            return items;
        }

        /// <summary>
        /// For dependency select non-checked nodes.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="dependencyItems">Dependency items</param>
        /// <returns>Collection of Dependency Info (Nodes)</returns>
        private IEnumerable<Dependency.DependencyInfo> GetNodeUnselectedDependencies(TreeView treeView, IEnumerable<Dependency> dependencyItems)
        {
            var items = new List<Dependency.DependencyInfo>();
            if (dependencyItems == null)
            {
                return items;
            }

            items.AddRange((from dependencyItem in dependencyItems
                            select treeView.GetTreeNodeByName(dependencyItem.Info.Uid.ToString())
                            into treeNode
                                where treeNode != null    
                                where !treeNode.Checked
                                select treeNode.Tag)
                            .OfType<Dependency>()
                            .Select(treeNodeDependency => ((Dependency)treeNodeDependency.Clone()).Info));

            dependencyItems
                .ForEach(item => GetNodeUnselectedDependencies(treeView, item));

            return items;
        }

        #endregion
    }
}


