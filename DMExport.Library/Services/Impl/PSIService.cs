using System;
using DMExport.Library.WorkflowService;
using DMExport.Library.CustomFieldsService;
using DMExport.Library.LookupTableService;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.Office.Project.Server.Library;

namespace DMExport.Library.Services.Impl
{
    public class PSIService : IPSIService
    {
        private readonly WorkflowService.Workflow _workflowWS;
        private readonly CustomFields _customFieldsWS;
        private readonly LookupTable _lookupTableWS;

        public string ServerUrl
        {
            get;
            private set;
        }

        public PSIService(string serverUrl)
        {
            ServerUrl = serverUrl;

            // Creating webservices
            _workflowWS = new WorkflowService.Workflow
                              {
                                  Credentials =
                                      System.Net.CredentialCache.DefaultCredentials,
                                  Url = string.Format("{0}/{1}", ServerUrl,
                                                      "_vti_bin/psi/workflow.asmx")
                              };

            _customFieldsWS = new CustomFields
                                  {
                                      Credentials =
                                          System.Net.CredentialCache.DefaultCredentials,
                                      Url = string.Format("{0}/{1}", ServerUrl,
                                                          "_vti_bin/psi/customfields.asmx")
                                  };

            _lookupTableWS = new LookupTable
                                 {
                                     Credentials =
                                         System.Net.CredentialCache.DefaultCredentials,
                                     Url = string.Format("{0}/{1}", ServerUrl,
                                                         "_vti_bin/psi/lookuptable.asmx")
                                 };
        }

        public WorkflowDataSet ReadEnterpriseProjectTypeList()
        {
            return _workflowWS.ReadEnterpriseProjectTypeList();
        }

        public WorkflowDataSet ReadWorkflowPhaseList()
        {
            return _workflowWS.ReadWorkflowPhaseList();
        }

        public WorkflowDataSet ReadWorkflowStageList()
        {
            return _workflowWS.ReadWorkflowStageList();
        }

        public CustomFieldDataSet ReadCustomFields()
        {
            return _customFieldsWS.ReadCustomFields(null, false);
        }

        public LookupTableDataSet ReadLookupTables()
        {
            return _lookupTableWS.ReadLookupTables(null, false,
                                                   CultureInfo.CurrentUICulture.LCID);
        }

        public WorkflowDataSet ReadEnterpriseProjectType(Guid eptUid)
        {
            return _workflowWS.ReadEnterpriseProjectType(eptUid);
        }

        public WorkflowDataSet ReadWorkflowPhase(Guid phaseUid)
        {
            return _workflowWS.ReadWorkflowPhase(phaseUid);
        }

        public WorkflowDataSet ReadWorkflowStage(Guid stageUid)
        {
            return _workflowWS.ReadWorkflowStage(stageUid);
        }

        public CustomFieldDataSet ReadCustomFieldsByMdPropUids2(Guid[] cfUids)
        {
            return _customFieldsWS.ReadCustomFieldsByMdPropUids2(cfUids, false);
        }

        public LookupTableDataSet ReadLookupTablesByUids(Guid[] ltUids)
        {
            return _lookupTableWS.ReadLookupTablesByUids(ltUids, false,
                                                         CultureInfo.CurrentUICulture.LCID);
        }

        /// <summary>
        /// Get the list containing the pdps
        /// </summary>
        public ISPList GetPDPList()
        {
            SPList pdpList = null;
            using (SPSite site = new SPSite(ServerUrl))
            {
                SPFolder pdpFolder =
                    site.RootWeb
                        .Folders[
                        ProjectDetailPages.ProjectDetailPagesRootUrl];

                foreach (SPList list in site.RootWeb.Lists)
                {
                    if (list.TemplateFeatureId.Equals(
                            ProjectDetailPages
                                .PROJECT_DETAIL_PAGES_FEATURE_UID) &&

                        (string.Compare(list.RootFolder.Url,
                                        ProjectDetailPages.ProjectDetailPagesRootUrl,
                                        StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        pdpList = list;
                        break;
                    }
                }
            }
            return new SPListAdapter(pdpList);
        }
    }
}


