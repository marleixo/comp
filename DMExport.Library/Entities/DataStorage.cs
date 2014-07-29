using System.Collections.Generic;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.LookupTableService;
using DMExport.Library.WorkflowService;

namespace DMExport.Library.Entities
{
    /// <summary>
    /// Stores entities from PS
    /// </summary>
    public class DataStorage
    {
        public WorkflowDataSet EnterpriseProjectTypes { get; set; }
        public WorkflowDataSet WorkflowPhases { get; set; }
        public WorkflowDataSet WorkflowStages { get; set; }
        public LookupTableDataSet LookupTables { get; set; }
        public CustomFieldDataSet CustomFields { get; set; }
        public ISPList ProjectDetailPages { get; set; }

        public List<Dependency.DependencyItem> ExportWarningItems { get; set; }
    }
}


