using System.Collections.Generic;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.Entities;
using DMExport.Library.LookupTableService;
using DMExport.Library.WorkflowService;

namespace DMExport.Library.Services
{
    public interface IDataService
    {
        WorkflowDataSet EnterpriseProjectTypes { get; }
        WorkflowDataSet WorkflowPhases { get; }
        WorkflowDataSet WorkflowStages { get; }
        LookupTableDataSet LookupTables { get; }
        CustomFieldDataSet CustomFields { get; }
        ISPList ProjectDetailPages { get; }
        IList<Dependency.DependencyInfo> ExportWarningItems { get; }
        IList<Dependency> Dependencies { get; }

        void AddExportWarningItems(IEnumerable<Dependency.DependencyInfo> items);
        void AddDependencyToStorage(Dependency dependency);
        void MakeDependencyGraph();

        void AssignEPTs(WorkflowDataSet workflowDataSet);
        void AssignPhases(WorkflowDataSet workflowDataSet);
        void AssignStages(WorkflowDataSet workflowDataSet);
        void AssignCustomFields(CustomFieldDataSet customFieldDataSet);
        void AssignLookupTables(LookupTableDataSet lookupTableDataSet);
        void AssignPDPs(ISPList pdpList);

        /// <summary>
        /// Scans all dependecies to create one-layer graph.
        /// </summary>
        /// <param name="dependency">Dependency instance</param>
        /// <param name="replacementDependencyList">Result List of Dependencies</param>
        void ProcessDependency(Dependency dependency, ICollection<Dependency> replacementDependencyList);
    }
}
