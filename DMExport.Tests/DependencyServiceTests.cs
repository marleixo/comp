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
using Microsoft.Office.Project.Server.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DMExport.Tests
{
    [TestClass]
    public class DependencyServiceTests
    {
        private readonly IDependencyService _dependencyService;
        private readonly IDataService _dataService;
        private readonly IPSIService _psiService;
        
        public DependencyServiceTests()
        {
            _psiService = new PSIService("http://localhost/pwa");
            _dataService = new DataService();
            _dependencyService = new DependencyService(_dataService, _psiService);
        }
        
        [TestMethod]
        public void DependencyService_GetCustomFieldDependenciesTest()
        {
            var lookupTableDataSet = new LookupTableDataSet();

            var ltRow = lookupTableDataSet.LookupTables.NewLookupTablesRow();
            ltRow.LT_NAME = "LT";
            ltRow.LT_UID = new Guid("11111111-1111-1111-1111-111111111111");

            lookupTableDataSet.LookupTables.AddLookupTablesRow(ltRow);

            _dataService.AssignLookupTables(lookupTableDataSet);

            var customFieldDataSet = new CustomFieldDataSet();

            var cfRow = customFieldDataSet.CustomFields.NewCustomFieldsRow();
            cfRow.MD_PROP_UID = new Guid("22222222-2222-2222-2222-222222222222");
            cfRow.MD_PROP_NAME = "CF";
            cfRow.MD_LOOKUP_TABLE_UID = ltRow.LT_UID;
            cfRow.MD_PROP_DESCRIPTION = "D";
            cfRow.MD_PROP_ID = 0;
            cfRow.MD_ENT_TYPE_UID = new Guid(EntityCollection.Entities.ProjectEntity.UniqueId);
            cfRow.MD_PROP_TYPE_ENUM = (byte)CustomField.Type.TEXT;

            customFieldDataSet.CustomFields.AddCustomFieldsRow(cfRow);

            _dataService.AssignCustomFields(customFieldDataSet);

            var dependency = _dependencyService.GetCustomFieldDependencies(cfRow.MD_PROP_UID);

            Assert.IsNotNull(dependency);
            Assert.AreEqual(dependency.Info.Uid, cfRow.MD_PROP_UID);
            Assert.AreEqual(dependency.Info.Name, cfRow.MD_PROP_NAME);
            Assert.AreEqual(dependency.Info.Type, EntityType.CustomField);
            
            Assert.AreEqual(dependency.Dependencies.Count(), 1);
            Assert.AreEqual(dependency.Dependencies.First().Info.Uid, ltRow.LT_UID);
            Assert.AreEqual(dependency.Dependencies.First().Info.Name, ltRow.LT_NAME);
            Assert.AreEqual(dependency.Dependencies.First().Info.Type, EntityType.LookupTable);
        }

        [TestMethod]
        public void DependencyService_GetStageDependenciesTest()
        {
            // Add phase
            var phase1Uid = new Guid("33333333-3333-3333-3333-333333333333");
            const string phase1Name = "Phase 1";
            var statusPdpUid = new Guid("22222222-2222-2222-2222-222222222222");

            var workflowDataSet = new WorkflowDataSet();

            var phase = workflowDataSet.WorkflowPhase.NewWorkflowPhaseRow();
            phase.SetPHASE_DESCRIPTIONNull();
            phase.PHASE_UID = phase1Uid;
            phase.PHASE_NAME = phase1Name;

            workflowDataSet.WorkflowPhase.AddWorkflowPhaseRow(phase);

            // Add Stage
            var stageUid = new Guid("11111111-1111-1111-1111-111111111111");
            const string stageName = "Stage 1";
            const string stageDescription = "Stage 1 description";
            const string stageSubmitDescription = "Stage 1 submit description";

            var stage = workflowDataSet.WorkflowStage.NewWorkflowStageRow();
            stage.STAGE_DESCRIPTION = stageDescription;
            stage.STAGE_NAME = stageName;
            stage.STAGE_SUBMIT_DESCRIPTION = stageSubmitDescription;
            stage.STAGE_UID = stageUid;
            stage.STATUS_PDP_UID = statusPdpUid;
            stage.PHASE_UID = phase1Uid;
            stage.PHASE_NAME = phase1Name;

            workflowDataSet.WorkflowStage.AddWorkflowStageRow(stage);

            // Add CustomField
            var cfUid = new Guid("44444444-4444-4444-4444-444444444444");
            const string cfName = "Cf";
            
            var cfRow = workflowDataSet.WorkflowStageCustomFields.NewWorkflowStageCustomFieldsRow();
            cfRow.MD_PROP_UID = cfUid;
            cfRow.MD_PROP_NAME = cfName;
            cfRow.STAGE_UID = stageUid;

            workflowDataSet.WorkflowStageCustomFields.AddWorkflowStageCustomFieldsRow(cfRow);

            _dataService.AssignStages(workflowDataSet);

            var dependency = _dependencyService.GetStageDependencies(stageUid);

            Assert.IsNotNull(dependency);
            Assert.AreEqual(dependency.Dependencies.Count(), 2);

            Assert.AreEqual(dependency.Info.Name, stage.STAGE_NAME);
            Assert.AreEqual(dependency.Info.Uid, stage.STAGE_UID);
            Assert.AreEqual(dependency.Info.Type, EntityType.Stage);

            // Find phase
            var depPhase = dependency.Dependencies.Where(item => item.Info.Type == EntityType.Phase).FirstOrDefault();

            Assert.IsNotNull(depPhase);
            Assert.AreEqual(depPhase.Info.Name, phase.PHASE_NAME);
            Assert.AreEqual(depPhase.Info.Uid, phase.PHASE_UID);
            Assert.AreEqual(depPhase.Info.Type, EntityType.Phase);

            // Find customField
            var depCf = dependency.Dependencies.Where(item => item.Info.Type == EntityType.CustomField).FirstOrDefault();

            Assert.IsNotNull(depCf);
            Assert.AreEqual(depCf.Info.Name, cfRow.MD_PROP_NAME);
            Assert.AreEqual(depCf.Info.Uid, cfRow.MD_PROP_UID);
            Assert.AreEqual(depCf.Info.Type, EntityType.CustomField);
        }
    }
}
