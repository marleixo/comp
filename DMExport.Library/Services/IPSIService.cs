using System;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.LookupTableService;
using DMExport.Library.WorkflowService;

namespace DMExport.Library.Services
{
    public interface IPSIService
    {
        string ServerUrl { get; }

        WorkflowDataSet ReadEnterpriseProjectTypeList();
        WorkflowDataSet ReadWorkflowPhaseList();
        WorkflowDataSet ReadWorkflowStageList();
        CustomFieldDataSet ReadCustomFields();
        LookupTableDataSet ReadLookupTables();
        WorkflowDataSet ReadEnterpriseProjectType(Guid eptUid);
        WorkflowDataSet ReadWorkflowPhase(Guid phaseUid);
        WorkflowDataSet ReadWorkflowStage(Guid stageUid);
        CustomFieldDataSet ReadCustomFieldsByMdPropUids2(Guid[] cfUids);
        LookupTableDataSet ReadLookupTablesByUids(Guid[] ltUids);

        ISPList GetPDPList();
    }
}
