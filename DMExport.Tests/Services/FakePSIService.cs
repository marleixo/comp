using System;
using DMExport.Library.Services;
using DMExport.Library.WorkflowService;
using DMExport.Library;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.LookupTableService;

namespace DMExport.Tests.Services
{
    public class FakePSIService : IPSIService
    {
        public string ServerUrl
        {
            get { return "http://localhost/pwa"; }
        }

        public WorkflowDataSet ReadEnterpriseProjectTypeList()
        {
            WorkflowDataSet eptDataSet = new WorkflowDataSet();
            for (int i = 0; i < 5; i++)
            {
                eptDataSet.Merge(ReadEnterpriseProjectType(Guid.NewGuid()));
            }
            return eptDataSet;
        }

        public WorkflowDataSet ReadWorkflowPhaseList()
        {
            WorkflowDataSet phasesDataSet = new WorkflowDataSet();
            for (int i = 0; i < 5; i++)
            {
                phasesDataSet.Merge(ReadWorkflowPhase(Guid.NewGuid()));
            }
            return phasesDataSet;
        }

        public WorkflowDataSet ReadWorkflowStageList()
        {
            WorkflowDataSet stagesDataSet = new WorkflowDataSet();
            for (int i = 0; i < 5; i++)
            {
                stagesDataSet.Merge(ReadWorkflowPhase(Guid.NewGuid()));
            }
            return stagesDataSet;
        }

        public CustomFieldDataSet ReadCustomFields()
        {
            Guid[] cfsUids = new Guid[5];
            for (int i = 0; i < cfsUids.Length; i++)
            {
                cfsUids[i] = Guid.NewGuid();
            }
            return ReadCustomFieldsByMdPropUids2(cfsUids);
        }

        public LookupTableDataSet ReadLookupTables()
        {
            Guid[] ltsUids = new Guid[5];
            for (int i = 0; i < ltsUids.Length; i++)
            {
                ltsUids[i] = Guid.NewGuid();
            }
            return ReadLookupTablesByUids(ltsUids);
        }

        public WorkflowDataSet ReadEnterpriseProjectType(Guid eptUid)
        {
            WorkflowDataSet eptDataSet = new WorkflowDataSet();
            WorkflowDataSet.EnterpriseProjectTypeRow row =
                eptDataSet.EnterpriseProjectType
                    .NewEnterpriseProjectTypeRow();
            row.ENTERPRISE_PROJECT_TYPE_NAME = "EPT";
            row.ENTERPRISE_PROJECT_TYPE_UID = eptUid;
            row.ENTERPRISE_PROJECT_WORKSPACE_TEMPLATE_NAME = "template";
            eptDataSet.EnterpriseProjectType.Rows.Add(row);
            return eptDataSet;
        }

        public WorkflowDataSet ReadWorkflowPhase(Guid phaseUid)
        {
            WorkflowDataSet phasesDataSet = new WorkflowDataSet();
            WorkflowDataSet.WorkflowPhaseRow row =
                phasesDataSet.WorkflowPhase.NewWorkflowPhaseRow();
            row.PHASE_NAME = "Phase";
            row.PHASE_UID = phaseUid;
            phasesDataSet.WorkflowPhase.Rows.Add(row);

            return phasesDataSet;
        }

        public WorkflowDataSet ReadWorkflowStage(Guid stageUid)
        {
            WorkflowDataSet stagesDataSet = new WorkflowDataSet();
            WorkflowDataSet.WorkflowPhaseRow phaseRow =
                stagesDataSet.WorkflowPhase.NewWorkflowPhaseRow();
            phaseRow.PHASE_NAME = "Phase";
            phaseRow.PHASE_UID = Guid.NewGuid();
            stagesDataSet.WorkflowPhase.Rows.Add(phaseRow);

            WorkflowDataSet.WorkflowStageRow row =
                stagesDataSet.WorkflowStage.NewWorkflowStageRow();
            row.STAGE_NAME = "Stage";
            row.STAGE_UID = stageUid;
            row.PHASE_UID = phaseRow.PHASE_UID;
            stagesDataSet.WorkflowStage.Rows.Add(row);

            return stagesDataSet;
        }

        public CustomFieldDataSet ReadCustomFieldsByMdPropUids2(Guid[] cfUids)
        {
            CustomFieldDataSet cfsDataSet = new CustomFieldDataSet();
            for (int i = 0; i < cfUids.Length; i++)
            {
                CustomFieldDataSet.CustomFieldsRow row =
                    cfsDataSet.CustomFields.NewCustomFieldsRow();
                row.MD_PROP_NAME = "Custom Field " + i;
                row.MD_PROP_UID = cfUids[i];
                row.MD_ENT_TYPE_UID = Guid.NewGuid();
                row.MD_PROP_TYPE_ENUM = 1;

                cfsDataSet.CustomFields.Rows.Add(row);
            }
            return cfsDataSet;
        }

        public LookupTableDataSet ReadLookupTablesByUids(Guid[] ltUids)
        {
            LookupTableDataSet ltsDataSet = new LookupTableDataSet();
            for (int i = 0; i < ltUids.Length; i++)
            {
                LookupTableDataSet.LookupTablesRow row =
                    ltsDataSet.LookupTables.NewLookupTablesRow();
                row.LT_NAME = "Lookup Table " + i;
                row.LT_UID = ltUids[i];

                ltsDataSet.LookupTables.Rows.Add(row);
            }
            return ltsDataSet;
        }

        public ISPList GetPDPList()
        {
            throw new NotImplementedException();
        }
    }
}


