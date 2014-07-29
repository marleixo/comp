using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.Entities;
using DMExport.Library.LookupTableService;
using DMExport.Library.Services;
using DMExport.Library.Services.Impl;
using DMExport.Library.WorkflowService;
using DMExport.Tests.Entities;
using Microsoft.Office.Project.Server.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DMExport.Tests
{
    [TestClass]
    public class DataServiceTests
    {
        private readonly IDataService _service;
        
        public DataServiceTests()
        {
            _service = new DataService();
        }
        
        [TestMethod]
        public void DataService_AssignEPTsTest()
        {
            const string ept1Name = "Ept1";
            const string ept1Description = "Ept1 description";
            var ept1Uid = new Guid("11111111-1111-1111-1111-111111111111");

            const string ept2Name = "Ept2";
            const string ept2Description = "Ept2 description";
            var ept2Uid = new Guid("11111111-1111-1111-1111-111111111112");

            var workflowDataSet = new WorkflowDataSet();

            var ept = workflowDataSet.EnterpriseProjectType.NewEnterpriseProjectTypeRow();
            ept.SetENTERPRISE_PROJECT_PLAN_TEMPLATE_UIDNull();
            ept.SetWORKFLOW_ASSOCIATION_UIDNull();
            ept.SetWORKFLOW_ASSOCIATION_NAMENull();

            ept.IS_DEFAULT_PROJECT_TYPE = true;
            ept.ENTERPRISE_PROJECT_TYPE_DESCRIPTION = ept1Description;
            ept.ENTERPRISE_PROJECT_TYPE_NAME = ept1Name;
            ept.ENTERPRISE_PROJECT_TYPE_ORDER = 1;
            ept.ENTERPRISE_PROJECT_TYPE_UID = ept1Uid;
            ept.ENTERPRISE_PROJECT_WORKSPACE_TEMPLATE_NAME = String.Empty;

            workflowDataSet.EnterpriseProjectType.AddEnterpriseProjectTypeRow(ept);

            ept = workflowDataSet.EnterpriseProjectType.NewEnterpriseProjectTypeRow();
            ept.SetENTERPRISE_PROJECT_PLAN_TEMPLATE_UIDNull();
            ept.SetWORKFLOW_ASSOCIATION_UIDNull();
            ept.SetWORKFLOW_ASSOCIATION_NAMENull();

            ept.IS_DEFAULT_PROJECT_TYPE = true;
            ept.ENTERPRISE_PROJECT_TYPE_DESCRIPTION = ept2Description;
            ept.ENTERPRISE_PROJECT_TYPE_NAME = ept2Name;
            ept.ENTERPRISE_PROJECT_TYPE_ORDER = 2;
            ept.ENTERPRISE_PROJECT_TYPE_UID = ept2Uid;
            ept.ENTERPRISE_PROJECT_WORKSPACE_TEMPLATE_NAME = String.Empty;

            workflowDataSet.EnterpriseProjectType.AddEnterpriseProjectTypeRow(ept);

            _service.AssignEPTs(workflowDataSet);

            var result = _service.EnterpriseProjectTypes;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.EnterpriseProjectType.Count, 2);

            var ept1 = result.EnterpriseProjectType[0];
            var ept2 = result.EnterpriseProjectType[1];

            Assert.AreEqual(ept1.ENTERPRISE_PROJECT_TYPE_UID, ept1Uid);
            Assert.AreEqual(ept1.ENTERPRISE_PROJECT_TYPE_NAME, ept1Name);

            Assert.AreEqual(ept2.ENTERPRISE_PROJECT_TYPE_UID, ept2Uid);
            Assert.AreEqual(ept2.ENTERPRISE_PROJECT_TYPE_NAME, ept2Name);
        }

        [TestMethod]
        public void DataService_AssignPhasesTest()
        {
            var phase1Uid = new Guid("11111111-1111-1111-1111-111111111111");
            const string phase1Name = "Phase 1";

            var phase2Uid = new Guid("11111111-1111-1111-1111-111111111112");
            const string phase2Name = "Phase 2";

            var workflowDataSet = new WorkflowDataSet();

            var phase = workflowDataSet.WorkflowPhase.NewWorkflowPhaseRow();
            phase.SetPHASE_DESCRIPTIONNull();
            phase.PHASE_UID = phase1Uid;
            phase.PHASE_NAME = phase1Name;

            workflowDataSet.WorkflowPhase.AddWorkflowPhaseRow(phase);

            phase = workflowDataSet.WorkflowPhase.NewWorkflowPhaseRow();
            phase.SetPHASE_DESCRIPTIONNull();
            phase.PHASE_UID = phase2Uid;
            phase.PHASE_NAME = phase2Name;

            workflowDataSet.WorkflowPhase.AddWorkflowPhaseRow(phase);

            _service.AssignPhases(workflowDataSet);

            var result = _service.WorkflowPhases;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.WorkflowPhase.Count, 2);

            var phase1 = result.WorkflowPhase.FindByPHASE_UID(phase1Uid);
            var phase2 = result.WorkflowPhase.FindByPHASE_UID(phase2Uid);

            Assert.AreEqual(phase1.PHASE_UID, phase1Uid);
            Assert.AreEqual(phase1.PHASE_NAME, phase1Name);
            Assert.IsTrue(phase1.IsPHASE_DESCRIPTIONNull());

            Assert.AreEqual(phase2.PHASE_UID, phase2Uid);
            Assert.AreEqual(phase2.PHASE_NAME, phase2Name);
            Assert.IsTrue(phase2.IsPHASE_DESCRIPTIONNull());    
        }
        
        [TestMethod]
        public void DataService_AssignStagesTest()
        {
            var stage1Uid = new Guid("11111111-1111-1111-1111-111111111111");
            const string stage1Name = "Stage 1";
            const string stage1Description = "Stage 1 description";
            const string stage1SubmitDescription = "Stage 1 submit description";

            var stage2Uid = new Guid("11111111-1111-1111-1111-111111111112");
            const string stage2Name = "Stage 2";
            const string stage2Description = "Stage 2 description";
            const string stage2SubmitDescription = "Stage 2 submit description";

            var statusPdpUid = new Guid("22222222-2222-2222-2222-222222222222");
            var phaseUid = new Guid("33333333-3333-3333-3333-333333333333");
            const string phaseName = "Phase";

            var workflowDataSet = new WorkflowDataSet();

            var phase = workflowDataSet.WorkflowPhase.NewWorkflowPhaseRow();
            phase.SetPHASE_DESCRIPTIONNull();
            phase.PHASE_UID = phaseUid;
            phase.PHASE_NAME = phaseName;

            workflowDataSet.WorkflowPhase.AddWorkflowPhaseRow(phase);

            var stage = workflowDataSet.WorkflowStage.NewWorkflowStageRow();
            stage.STAGE_DESCRIPTION = stage1Description;
            stage.STAGE_NAME = stage1Name;
            stage.STAGE_SUBMIT_DESCRIPTION = stage1SubmitDescription;
            stage.STAGE_UID = stage1Uid;
            stage.STATUS_PDP_UID = statusPdpUid;
            stage.PHASE_UID = phaseUid;
            stage.PHASE_NAME = phaseName;

            workflowDataSet.WorkflowStage.AddWorkflowStageRow(stage);

            stage = workflowDataSet.WorkflowStage.NewWorkflowStageRow();
            stage.STAGE_DESCRIPTION = stage2Description;
            stage.STAGE_NAME = stage2Name;
            stage.STAGE_SUBMIT_DESCRIPTION = stage2SubmitDescription;
            stage.STAGE_UID = stage2Uid;
            stage.STATUS_PDP_UID = statusPdpUid;
            stage.PHASE_UID = phaseUid;
            stage.PHASE_NAME = phaseName;

            workflowDataSet.WorkflowStage.AddWorkflowStageRow(stage);

            _service.AssignStages(workflowDataSet);

            var result = _service.WorkflowStages;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.WorkflowStage.Count, 2);

            var stage1 = result.WorkflowStage.FindBySTAGE_UID(stage1Uid);
            var stage2 = result.WorkflowStage.FindBySTAGE_UID(stage2Uid);

            Assert.IsNotNull(stage1);
            Assert.AreEqual(stage1.STAGE_DESCRIPTION, stage1Description);
            Assert.AreEqual(stage1.STAGE_NAME, stage1Name);
            Assert.AreEqual(stage1.STAGE_SUBMIT_DESCRIPTION, stage1SubmitDescription);
            Assert.AreEqual(stage1.STAGE_UID, stage1Uid);
            Assert.AreEqual(stage1.STATUS_PDP_UID, statusPdpUid);
            Assert.AreEqual(stage1.PHASE_UID, phaseUid);
            Assert.AreEqual(stage1.PHASE_NAME, phaseName);

            Assert.IsNotNull(stage2);
            Assert.AreEqual(stage2.STAGE_DESCRIPTION, stage2Description);
            Assert.AreEqual(stage2.STAGE_NAME, stage2Name);
            Assert.AreEqual(stage2.STAGE_SUBMIT_DESCRIPTION, stage2SubmitDescription);
            Assert.AreEqual(stage2.STAGE_UID, stage2Uid);
            Assert.AreEqual(stage2.STATUS_PDP_UID, statusPdpUid);
            Assert.AreEqual(stage2.PHASE_UID, phaseUid);
            Assert.AreEqual(stage2.PHASE_NAME, phaseName);
        }

        [TestMethod]
        public void DataService_AssignPDPsTest()
        {
            var fakeSpList = new FakeSPList();
            for (var i = 0; i < 5; i++)
            {
                fakeSpList.Items.Add(new FakeSPListItem("PDP" + i));
            }

            _service.AssignPDPs(fakeSpList);

            var result = _service.ProjectDetailPages.Items;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), 5);
            Assert.AreEqual(result.First()["TitlePDP"], "PDP0");
        }

        [TestMethod]
        public void DataService_AddExportWarningItemsTest()
        {
            var exportWarningItems = new List<Dependency.DependencyInfo>()
             {
                 new Dependency.DependencyInfo()
                     {
                         Name = "Item1", 
                         Type = EntityType.Phase, 
                         Uid = new Guid("11111111-1111-1111-1111-111111111111")
                     },
                 new Dependency.DependencyInfo()
                     {
                         Name = "Item2", 
                         Type = EntityType.ProjectDetailPage, 
                         Uid = new Guid("11111111-1111-1111-1111-111111111112")
                     },
                 new Dependency.DependencyInfo()
                     {
                         Name = "Item3", 
                         Type = EntityType.LookupTable, 
                         Uid = new Guid("11111111-1111-1111-1111-111111111113")
                     },
             };

            _service.AddExportWarningItems(exportWarningItems);

            var result = _service.ExportWarningItems;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 3);

            var secondItem = result[1];

            Assert.IsNotNull(secondItem);
            Assert.AreEqual(secondItem.Name, exportWarningItems[1].Name);
            Assert.AreEqual(secondItem.Type, exportWarningItems[1].Type);
            Assert.AreEqual(secondItem.Uid, exportWarningItems[1].Uid);
        }

        [TestMethod]
        public void DataService_AddDependencyToStorageTest()
        {
            var dependency = new Dependency("Item1", new Guid("11111111-1111-1111-1111-111111111111"), EntityType.Ept);

            _service.AddDependencyToStorage(dependency);

            var result = _service.Dependencies;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result.First().Info.Name, dependency.Info.Name);
            Assert.AreEqual(result.First().Info.Type, dependency.Info.Type);
            Assert.AreEqual(result.First().Info.Uid, dependency.Info.Uid);
        }

        [TestMethod]
        public void DataService_MakeDependencyGraphTest()
        {
            var phaseDependency = new Dependency("Phase1", new Guid("11111111-1111-1111-1111-111111111111"), EntityType.Phase);
            var stageDependency = new Dependency("Stage1", new Guid("22222222-2222-2222-2222-222222222222"), EntityType.Stage);
            var pdpDependency = new Dependency("Pdp1", new Guid("33333333-3333-3333-3333-333333333333"), EntityType.ProjectDetailPage);

            phaseDependency.AddDependency(stageDependency);
            stageDependency.AddDependency(pdpDependency);

            _service.AddDependencyToStorage(phaseDependency);
            _service.MakeDependencyGraph();

            var result = _service.Dependencies;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 2);

            var first = result[0];
            var second = result[1];

            Assert.IsNotNull(first);
            Assert.AreEqual(first.Info.Uid, phaseDependency.Info.Uid);
            Assert.AreEqual(first.Info.Name, phaseDependency.Info.Name);
            Assert.AreEqual(first.Info.Type, phaseDependency.Info.Type);
            Assert.AreEqual(first.Dependencies.Count(), 1);
            Assert.AreEqual(first.Dependencies.First().Info.Uid, stageDependency.Info.Uid);
            Assert.AreEqual(first.Dependencies.First().Info.Name, stageDependency.Info.Name);
            Assert.AreEqual(first.Dependencies.First().Info.Type, stageDependency.Info.Type);

            Assert.IsNotNull(second);
            Assert.AreEqual(second.Info.Uid, stageDependency.Info.Uid);
            Assert.AreEqual(second.Info.Name, stageDependency.Info.Name);
            Assert.AreEqual(second.Info.Type, stageDependency.Info.Type);
            Assert.AreEqual(second.Dependencies.Count(), 1);
            Assert.AreEqual(second.Dependencies.First().Info.Uid, pdpDependency.Info.Uid);
            Assert.AreEqual(second.Dependencies.First().Info.Name, pdpDependency.Info.Name);
            Assert.AreEqual(second.Dependencies.First().Info.Type, pdpDependency.Info.Type);
        }

        [TestMethod]
        public void DataService_AssignCustomFieldsTest()
        {
            var customFieldDataSet = new CustomFieldDataSet();

            var cfRow = customFieldDataSet.CustomFields.NewCustomFieldsRow();
            cfRow.MD_PROP_UID = new Guid("22222222-2222-2222-2222-222222222222");
            cfRow.MD_PROP_NAME = "CF";
            cfRow.MD_PROP_DESCRIPTION = "D";
            cfRow.MD_PROP_ID = 0;
            cfRow.MD_ENT_TYPE_UID = new Guid(EntityCollection.Entities.ProjectEntity.UniqueId);
            cfRow.MD_PROP_TYPE_ENUM = (byte)CustomField.Type.TEXT;

            customFieldDataSet.CustomFields.AddCustomFieldsRow(cfRow);

            _service.AssignCustomFields(customFieldDataSet);

            var result = _service.CustomFields.CustomFields;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);

            var cf = result.FindByMD_PROP_UID(cfRow.MD_PROP_UID);
            
            Assert.IsNotNull(cf);
            Assert.AreEqual(cf.MD_PROP_UID, cfRow.MD_PROP_UID);
            Assert.AreEqual(cf.MD_PROP_NAME, cfRow.MD_PROP_NAME);
        }

        [TestMethod]
        public void DataService_AssignLookupTablesTest()
        {
            var lookupTableDataSet = new LookupTableDataSet();

            var ltRow = lookupTableDataSet.LookupTables.NewLookupTablesRow();
            ltRow.LT_NAME = "LT";
            ltRow.LT_UID = new Guid("11111111-1111-1111-1111-111111111111");

            lookupTableDataSet.LookupTables.AddLookupTablesRow(ltRow);

            _service.AssignLookupTables(lookupTableDataSet);

            var result = _service.LookupTables.LookupTables;
            
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);

            var lt = result.FindByLT_UID(ltRow.LT_UID);

            Assert.IsNotNull(lt);
            Assert.AreEqual(lt.LT_UID, ltRow.LT_UID);
            Assert.AreEqual(lt.LT_NAME, ltRow.LT_NAME);
        }
    }
}
