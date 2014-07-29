using System;
using System.Collections.Generic;
using System.Linq;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.Entities;
using DMExport.Library.Helpers;
using DMExport.Library.LookupTableService;
using DMExport.Library.WorkflowService;

namespace DMExport.Library.Services.Impl
{
    /// <summary>
    /// Stores entities from PS
    /// </summary>
    public class DataService : IDataService
    {
        #region Fields and Properties

        // Data from PWA
        public WorkflowDataSet EnterpriseProjectTypes { get; set; }
        public WorkflowDataSet WorkflowPhases { get; set; }
        public WorkflowDataSet WorkflowStages { get; set; }
        public LookupTableDataSet LookupTables { get; set; }
        public CustomFieldDataSet CustomFields { get; set; }
        public ISPList ProjectDetailPages { get; set; }

        private IList<Dependency.DependencyInfo> _exportWarningItems;
        /// <summary>
        /// Export Warning Items
        /// </summary>
        public IList<Dependency.DependencyInfo> ExportWarningItems
        {
            get { return _exportWarningItems; }
        }

        private IList<Dependency> _dependencies;
        /// <summary>
        /// All Dependencies
        /// </summary>
        public IList<Dependency> Dependencies
        {
            get { return _dependencies; }
        }

        #endregion

        #region Constructors

        public DataService()
        {
            _exportWarningItems = new List<Dependency.DependencyInfo>();
            _dependencies = new List<Dependency>();

            EnterpriseProjectTypes = new WorkflowDataSet();
            WorkflowPhases = new WorkflowDataSet();
            WorkflowStages = new WorkflowDataSet();
            CustomFields = new CustomFieldDataSet();
            LookupTables = new LookupTableDataSet();
        }

        #endregion

        /// <summary>
        /// Adds Dependency to Data DataService
        /// </summary>
        /// <param name="dependency">Dependency Item</param>
        public void AddDependencyToStorage(Dependency dependency)
        {
            _dependencies.Add(dependency);
        }

        /// <summary>
        /// Adds Items to Warning items Collection
        /// </summary>
        /// <param name="items">Collection of Items</param>
        public void AddExportWarningItems(IEnumerable<Dependency.DependencyInfo> items)
        {
            items.ForEach(item => ExportWarningItems.Add(item));
        }    

        /// <summary>
        /// Cleans up the dependencies to get one-layer graph.
        /// </summary>
        public void MakeDependencyGraph()
        {
            var replacementDependencyList = new List<Dependency>();

            _dependencies
                .ForEach(dependency => ProcessDependency(dependency, replacementDependencyList));

            _dependencies = (from dependency in replacementDependencyList
                             group dependency by dependency.Info into g 
                             select new {Info = g.Key, Dependencies = g.ToList()})
                .Select(item => new Dependency(item.Info, item.Dependencies.SelectMany(d => d.Dependencies).Distinct()))
                .ToList();

            _dependencies
                .ForEach(dependency => dependency.Dependencies.ForEach(child => child.ClearDependencies()));                
        }

        /// <summary>
        /// Assigns EPTs.
        /// </summary>
        /// <param name="workflowDataSet">Workflow DataSet</param>
        public void AssignEPTs(WorkflowDataSet workflowDataSet)
        {
            EnterpriseProjectTypes = workflowDataSet;
        }

        /// <summary>
        /// Assigns Phases.
        /// </summary>
        /// <param name="workflowDataSet">Workflow DataSet</param>
        public void AssignPhases(WorkflowDataSet workflowDataSet)
        {
            WorkflowPhases = workflowDataSet;
        }

        /// <summary>
        /// Assigns Stages.
        /// </summary>
        /// <param name="workflowDataSet">Workflow DataSet</param>
        public void AssignStages(WorkflowDataSet workflowDataSet)
        {
            WorkflowStages = workflowDataSet;
        }

        /// <summary>
        /// Assigns Custom Fields.
        /// </summary>
        /// <param name="customFieldDataSet">Custom Fields DataSet</param>
        public void AssignCustomFields(CustomFieldDataSet customFieldDataSet)
        {
            CustomFields = customFieldDataSet;
        }

        /// <summary>
        /// Assigns Lookup Tables.
        /// </summary>
        /// <param name="lookupTableDataSet">Lookup Table DataSet</param>
        public void AssignLookupTables(LookupTableDataSet lookupTableDataSet)
        {
            LookupTables = lookupTableDataSet;
        }

        /// <summary>
        /// Assigns PDPs
        /// </summary>
        /// <param name="pdpList">List of PDPs</param>
        public void AssignPDPs(ISPList pdpList)
        {
            ProjectDetailPages = pdpList;
        }

        /// <summary>
        /// Scans all dependecies to create one-layer graph.
        /// </summary>
        /// <param name="dependency">Dependency instance</param>
        /// <param name="replacementDependencyList">Result List of Dependencies</param>
        public void ProcessDependency(Dependency dependency, ICollection<Dependency> replacementDependencyList)
        {
            dependency.Dependencies
                .ForEach(item =>
                 {
                     var replacementDependency = new Dependency(dependency.Info);
                     replacementDependency.AddDependency(item.Clone() as Dependency);
                     replacementDependencyList.Add(replacementDependency);

                     ProcessDependency(item, replacementDependencyList);
                 });
        }
    }
}


