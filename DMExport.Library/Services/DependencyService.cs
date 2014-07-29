using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.Entities;
using DMExport.Library.Helpers;
using DMExport.Library.LookupTableService;
using DMExport.Library.WorkflowService;
using EPTAssociationTool.Library.Model;
using EPTAssociationTool.Library.Services.Impl;

namespace DMExport.Library.Services
{
    public class DependencyService : IDependencyService
    {
        private readonly DataStorage _dataStorage;
        
        public DependencyService(DataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }
        
        /// <summary>
        /// Gets dependency for a stage
        /// </summary>
        /// <param name="stageUid">Workflow Stage UID</param>
        /// <returns>Dependency instance</returns>
        public Dependency GetStageDependencies(Guid stageUid)
        {
            var stageRow = _dataStorage.WorkflowStages.WorkflowStage.Rows
                .Cast<WorkflowDataSet.WorkflowStageRow>()
                .Where(item => item.STAGE_UID == stageUid)
                .First();

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

            ApplyPdpsDependencies(pdps);

            return new Dependency
                       {
                           Item = new Dependency.DependencyItem
                                      {
                                          Type = EntityType.Stage,
                                          Uid = stageUid,
                                          Name = stageRow.STAGE_NAME
                                      },

                           Phases = phases,
                           CustomFields = customFields.ToList(),
                           Pdps = pdps.ToList()
                       };
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
            return ApplyCustomFieldsDependencies(new List<Dependency>() { dependency }).FirstOrDefault();
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
        /// Fills a list of warnings during export.
        /// </summary>
        /// <param name="treeView">Treeview object</param>
        public void CheckDependenciesOnExport(TreeView treeView)
        {
            _dataStorage.ExportWarningItems = new List<Dependency.DependencyItem>();

            foreach (TreeNode upperNode in treeView.Nodes)
            {
                foreach (TreeNode node in from TreeNode node in upperNode.Nodes
                                          where node.Checked
                                          where node.Tag != null
                                          where (node.Tag is Dependency)
                                          select node)
                {
                    _dataStorage.ExportWarningItems
                        .AddRange(GetNodeUnselectedDependencies(treeView, node.Tag as Dependency));
                }
            }
        }

        /// <summary>
        /// Finds all dependencies for collection of custom fields
        /// </summary>
        /// <param name="customFieldsItems">Collection of Custom Fields</param>
        /// <returns>Collection of Custom Fields</returns>
        private IEnumerable<Dependency> ApplyCustomFieldsDependencies(IEnumerable<Dependency> customFieldsItems)
        {
            customFieldsItems
                .ToList()
                .ForEach(cf =>
                             {
                                 var customFieldRow = _dataStorage.CustomFields.CustomFields.FindByMD_PROP_UID(cf.Item.Uid);
                                 if (!customFieldRow.IsMD_LOOKUP_TABLE_UIDNull())
                                 {
                                     var ltRow = _dataStorage.LookupTables.LookupTables.FindByLT_UID(customFieldRow.MD_LOOKUP_TABLE_UID);
                                     cf.LookupTables = new List<Dependency> { new Dependency(ltRow.LT_NAME, ltRow.LT_UID, EntityType.LookupTable) };
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
            SharePointService sharePointService = new SharePointService(new SessionService());
            
            pdpsItems
                .ToList()
                .ForEach(pdpItem =>
                             {
                                 IEnumerable<PSWebPartInfo> pdpWebParts = sharePointService.GetPDPWebParts(pdpItem.Item.Uid, true);

                                 var customFieldUids = pdpWebParts
                                     .Where(webPart => webPart.CustomFieldUids != null)
                                     .SelectMany(webPart => webPart.CustomFieldUids);
                                 
                                 var customFields = _dataStorage.CustomFields.CustomFields.Rows
                                    .Cast<CustomFieldDataSet.CustomFieldsRow>()
                                    .Where(cfRow => customFieldUids.Contains(cfRow.MD_PROP_UID))
                                    .Select(cfRow => new Dependency(cfRow.MD_PROP_NAME, cfRow.MD_PROP_UID, EntityType.CustomField));

                                 pdpItem.CustomFields = customFields.ToList();

                                 ApplyCustomFieldsDependencies(customFields);
                             });
            
            return pdpsItems;
        }

        /// <summary>
        /// For Dependency get all non-checked nodes.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="dependency">Dependency</param>
        /// <returns>Collection of Dependency Item (Nodes)</returns>
        private IEnumerable<Dependency.DependencyItem> GetNodeUnselectedDependencies(TreeView treeView, Dependency dependency)
        {
            var items = new List<Dependency.DependencyItem>();
            if (dependency == null)
            {
                return items;
            }

            var properties = dependency.Phases
                .Union(dependency.LookupTables)
                .Union(dependency.CustomFields)
                .Union(dependency.Pdps);

            items.AddRange(GetNodeUnselectedDependencies(treeView, properties));
            return items;
        }

        /// <summary>
        /// For dependency select non-checked nodes.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="dependencyItems">Dependency items</param>
        /// <returns>Collection of Dependency Item (Nodes)</returns>
        private IEnumerable<Dependency.DependencyItem> GetNodeUnselectedDependencies(TreeView treeView, IEnumerable<Dependency> dependencyItems)
        {
            var items = new List<Dependency.DependencyItem>();
            if (dependencyItems == null)
            {
                return items;
            }

            items.AddRange((from dependencyItem in dependencyItems
                            select treeView.GetTreeNodeByName(dependencyItem.Item.Uid.ToString())
                            into treeNode
                                where !treeNode.Checked
                                select treeNode.Tag)
                               .OfType<Dependency>()
                               .Select(treeNodeDependency => ((Dependency)treeNodeDependency.Clone()).Item));

            dependencyItems
                .ToList()
                .ForEach(item => GetNodeUnselectedDependencies(treeView, item));

            return items;
        }
    }
}


