using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DMExport.Library.Services;
using DMExport.Library.Services.Impl;
using DMExport.Tests.Entities;
using DMExport.Tests.Services;
using DMExport.Library;
using DMExport.Library.WorkflowService;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.LookupTableService;
using Microsoft.SharePoint.Deployment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DMExport.Tests
{
    [TestClass]
    public class TreeBuilderTests
    {
        private readonly IPSIService _psi;
        private readonly IDependencyService _dependencyService;
        private readonly IDataService _dataService;
        private readonly TreeBuilder _treeBuilder;

        public TreeBuilderTests()
        {
            _psi = new FakePSIService();
            _dataService = new DataService();
            _dependencyService = new DependencyService(_dataService, _psi);

            var list = new FakeSPList();
            for (int i = 0; i < 5; i++)
            {
                list._items.Add(new FakeSPListItem("PDP " + i));
            }

            _dataService.AssignPDPs(list);
            _treeBuilder = new TreeBuilder(_psi, _dependencyService, _dataService);
        }
        
        [TestMethod]
        public void TreeBuilder_CreateParentNodeAndAddPdpsTest()
        {
            FakeSPList list = new FakeSPList();
            for (int i = 0; i < 5; i++)
            {
                list._items.Add(new FakeSPListItem("PDP " + i));
            }

            using (TreeView treeView = new TreeView())
            {
                _treeBuilder.CreateParentNodeAndAddPdps(treeView, list);

                Assert.AreEqual(1, treeView.Nodes.Count);
                Assert.AreEqual(list._items.Count, treeView.Nodes[0].Nodes.Count);
            }
        }

        [TestMethod]
        public void TreeBuilder_CreateParentNodeAndAddEptsTest()
        {
            WorkflowDataSet eptDataSet = _psi.ReadEnterpriseProjectTypeList();
            
            using (TreeView treeView = new TreeView())
            {
                _treeBuilder.CreateParentNodeAndAddEpts(treeView, eptDataSet);

                Assert.AreEqual(1, treeView.Nodes.Count);
                Assert.AreEqual(eptDataSet.EnterpriseProjectType.Rows.Count,  treeView.Nodes[0].Nodes.Count);
            }
        }

        [TestMethod]
        public void TreeBuilder_CreateParentNodeAndAddPhasesTest()
        {
            WorkflowDataSet phasesDataSet = _psi.ReadWorkflowPhaseList();

            using (TreeView treeView = new TreeView())
            {
                _treeBuilder.CreateParentNodeAndAddPhases(treeView, phasesDataSet);

                Assert.AreEqual(1, treeView.Nodes.Count);
                Assert.AreEqual(phasesDataSet.WorkflowPhase.Rows.Count, treeView.Nodes[0].Nodes.Count);
            }
        }

        [TestMethod]
        public void TreeBuilder_CreateParentNodeAndAddStagesTest()
        {
            WorkflowDataSet stagesDataSet = _psi.ReadWorkflowPhaseList();

            using (TreeView treeView = new TreeView())
            {
                _treeBuilder.CreateParentNodeAndAddStages(treeView, stagesDataSet);

                Assert.AreEqual(1, treeView.Nodes.Count);
                Assert.AreEqual(stagesDataSet.WorkflowStage.Rows.Count, treeView.Nodes[0].Nodes.Count);
            }
        }

        [TestMethod]
        public void TreeBuilder_CreateParentNodeAndAddCustomFieldsTest()
        {
            CustomFieldDataSet cfsDataSet = _psi.ReadCustomFields();

            using (TreeView treeView = new TreeView())
            {
                _treeBuilder.CreateParentNodeAndAddCustomFields(treeView, cfsDataSet);

                Assert.AreEqual(1, treeView.Nodes.Count);
                Assert.AreEqual(cfsDataSet.CustomFields.Rows.Count, treeView.Nodes[0].Nodes.Count);
            }
        }

        [TestMethod]
        public void TreeBuilder_CreateParentNodeAndAddLookupTablesTest()
        {
            LookupTableDataSet ltsDataSet = _psi.ReadLookupTables();

            using (TreeView treeView = new TreeView())
            {
                _treeBuilder.CreateParentNodeAndAddLookupTables(treeView, ltsDataSet);

                Assert.AreEqual(1, treeView.Nodes.Count);
                Assert.AreEqual(ltsDataSet.LookupTables.Rows.Count, treeView.Nodes[0].Nodes.Count);
            }
        }

        [TestMethod]
        public void TreeBuilder_GetPdpsToExportTest()
        {
            const int nodesCount = 10;
            TreeNode pdpRootNode = GetHalfTreeNodesSelected(nodesCount);

            List<SPExportObject> pdpsToExport = _treeBuilder.GetPdpsToExport(pdpRootNode, Guid.NewGuid());
            Assert.AreEqual(nodesCount / 2, pdpsToExport.Count);
        }

        [TestMethod]
        public void TreeBuilder_GetEptsToExportTest()
        {
            const int nodesCount = 10;
            TreeNode eptRootNode = GetHalfTreeNodesSelected(nodesCount);

            WorkflowDataSet eptDataSet = _treeBuilder.GetEptsToExport(eptRootNode);//Callegario, new FakeSPWorkflowAssociationCollection());
            Assert.AreEqual(nodesCount / 2, eptDataSet.EnterpriseProjectType.Rows.Count);
        }

        [TestMethod]
        public void TreeBuilder_GetPhasesToExportTest()
        {
            const int nodesCount = 10;
            TreeNode phasesRootNode = GetHalfTreeNodesSelected(nodesCount);
            
            WorkflowDataSet phasesDataSet = _treeBuilder.GetPhasesToExport(phasesRootNode);
            Assert.AreEqual(nodesCount / 2, phasesDataSet.WorkflowPhase.Rows.Count);
        }

        [TestMethod]
        public void TreeBuilder_GetStagesToExportTest()
        {
            const int nodesCount = 10;
            TreeNode stagesRootNode = GetHalfTreeNodesSelected(nodesCount);

            WorkflowDataSet stagesDataSet = _treeBuilder.GetStagesToExport(stagesRootNode);
            Assert.AreEqual(nodesCount / 2, stagesDataSet.WorkflowStage.Rows.Count);
        }

        [TestMethod]
        public void TreeBuilder_GetCustomFieldsToExportTest()
        {
            const int nodesCount = 10;
            TreeNode cfsRootNode = GetHalfTreeNodesSelected(nodesCount);

            CustomFieldDataSet cfsDataSet = _treeBuilder.GetCustomFieldsToExport(cfsRootNode);
            Assert.AreEqual(nodesCount / 2, cfsDataSet.CustomFields.Rows.Count);
        }

        [TestMethod]
        public void TreeBuilder_GetLookupTablesToExportTest()
        {
            const int nodesCount = 10;
            TreeNode ltsRootNode = GetHalfTreeNodesSelected(nodesCount);

            LookupTableDataSet ltsDataSet = _treeBuilder.GetLookupTablesToExport(ltsRootNode);
            Assert.AreEqual(nodesCount / 2, ltsDataSet.LookupTables.Rows.Count);
        }

        private static TreeNode GetHalfTreeNodesSelected(int nodesCount)
        {
            TreeNode rootNode = new TreeNode();

            for (int i = 1; i <= nodesCount; i++)
            {
                TreeNode node = new TreeNode
                {
                    Text = "Node " + i,
                    Name = Guid.NewGuid().ToString()
                };
                node.Checked = (i % 2 == 0);
                rootNode.Nodes.Add(node);
            }
            return rootNode;
        }
    }
}


