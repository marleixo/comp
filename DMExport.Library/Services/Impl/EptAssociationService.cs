using System;
using System.Collections.Generic;
using System.Linq;
using DMExport.Library.Entities;
using EPTAssociationTool.Library;
using EPTAssociationTool.Library.AssociationDataAccess;
using EPTAssociationTool.Library.Model;
using EPTAssociationTool.Library.Services;
using EPTAssociationTool.Library.Services.Impl;
using Microsoft.SharePoint;

namespace DMExport.Library.Services.Impl
{
    public class EptAssociationService : IEptAssociationService
    {
        #region Fields and Services

        private readonly string _serverUrl;
        
        private readonly ILogService _logService;
        private readonly IViewService _viewService;
        private readonly ISessionService _sessionService;
        private readonly ISharePointService _sharePointService;
        private readonly ICustomFieldsWebService _customFieldsWebService;
        private readonly EPTAssociationTool.Library.Services.IWorkflowService _workflowService;
        private readonly EPTAssociationTool.Library.Services.ICustomFieldsService _customFieldsService;

        #endregion

        #region Constructors

        public EptAssociationService(string serverUrl)
        {
            _serverUrl = serverUrl;
            _logService = new LogService();
            _sessionService = new SessionService();

            InitializeSessionService();

            _sharePointService = new SharePointService(_sessionService);
            _customFieldsWebService = new CustomFieldsWebService(_sessionService);
            _customFieldsService = new EPTAssociationTool.Library.Services.Impl.CustomFieldsService(_logService,
                                                                                                    _customFieldsWebService);

            _workflowService = new EPTAssociationTool.Library.Services.Impl.WorkflowService(
                _logService, _sessionService, _sharePointService, _customFieldsService);

            _viewService = new ViewService(_logService, _sessionService, _customFieldsService);
        }

        #endregion

        /// <summary>
        /// Initializes EPTAssociationTool Session service
        /// </summary>
        private void InitializeSessionService()
        {
            var uri = new Uri(_serverUrl);
            var serverName = uri.Host;

            var pwa = String.Empty;
            if (uri.Segments.Count() > 1)
            {
                pwa = uri.Segments[1];
            }

            _sessionService.ServerName = serverName;
            _sessionService.Pwa = pwa;
        }

        /// <summary>
        /// Do some action under EPTASsociationTool context
        /// </summary>
        /// <param name="action">Action instance</param>
        private void PerformInContext(Action<IAssociationDataContextAdapter, ISessionService, AssociationDataService> action)
        {
            using (var contextAdapter = new AssociationDataContextAdapter(_sessionService.PwaBaseUrl))
            {
                using (var assDataService = new AssociationDataService(contextAdapter))
                {
                    action(contextAdapter, _sessionService, assDataService);
                }
            }
        }

        /// <summary>
        /// Determines if EPTAssociationTool is installed under current PWA site.
        /// </summary>
        public bool IsEptAssociationToolInstalled
        {
            get
            {
                return false; //Callegario
                using (var spSite = new SPSite(_serverUrl))
                {
                    using (var spWeb = spSite.OpenWeb())
                    {
                        return EPTAssociationToolFeature.IsInstalled(spWeb);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if EPT is Workflow Controlled under EPTAssociationTool
        /// </summary>
        /// <param name="eptUid">EPT UID</param>
        /// <returns>True if workflow-controlled.</returns>
        public bool IsEptWorkflowControlled(Guid eptUid)
        {
            bool isEptWorkflowControlled = false;

            PerformInContext(
                (ctxAdapter, sessionService, assDataService) =>
                {
                    using (var eptDataService = new EPTDataService(_workflowService, assDataService, ctxAdapter))
                    {
                        isEptWorkflowControlled = eptDataService.IsEPTWorkflowControlled(eptUid);
                    }
                }
            );
            return isEptWorkflowControlled;
        }

        /// <summary>
        /// Gets PDPs Custom Fields UIDs.
        /// </summary>
        /// <param name="pdpUid">PDP UID</param>
        /// <returns>Collection of Custom Fields UIDs</returns>
        public IEnumerable<Guid> GetPdpCustomFieldsUids(Guid pdpUid)
        {
            return null; //Callegario
            IEnumerable<PSWebPartInfo> pdpWebParts = _sharePointService.GetPDPWebParts(pdpUid, true); 

            return pdpWebParts
                .Where(webPart => webPart.CustomFieldUids != null)
                .SelectMany(webPart => webPart.CustomFieldUids);
        }

        /// <summary>
        /// Gets Dependencies for a workflow-controlled EPT from EPTAssociationTool.
        /// </summary>
        /// <param name="eptUid">EPT UID</param>
        /// <returns>Collection of Dependencies.</returns>
        public IEnumerable<Dependency> GetWorkflowControlledEptDependencies(Guid eptUid)
        {
            BaseInfo baseInfo = null;

            PerformInContext(
                (ctxAdapter, sessionService, assDataService) =>
                    {
                        EPTAssociationTool.Library.PSI psi = new EPTAssociationTool.Library.PSI(_workflowService,
                                                                                                assDataService,
                                                                                                _sharePointService,
                                                                                                _viewService);
                        baseInfo = psi.GetEPTInfo(eptUid, true, true);
                    });

            var dependencies = new List<Dependency>();
            if (baseInfo == null)
            {
                return dependencies;
            }

            if (baseInfo.Children == null)
            {
                return dependencies;
            }
            
            baseInfo.Children
                .Where(phase => phase.Children != null)
                .ToList()
                .ForEach(phase =>
                 {
                     Dependency phaseDependency = new Dependency(phase.Name, phase.Uid, EntityType.Phase);
                     dependencies.Add(phaseDependency);

                     // Process phase
                     phase.Children
                         .Where(stage => stage.Children != null)
                         .ToList()
                         .ForEach(stage =>
                          {
                              Dependency stageDependency = new Dependency(stage.Name, stage.Uid, EntityType.Stage);
                              dependencies.Add(stageDependency);

                              // Process Stage
                              stage.Children
                                 .Where(pdp => pdp.Children != null)
                                 .ToList()
                                 .ForEach(pdp =>
                                  {
                                      Dependency pdpDependency = new Dependency(pdp.Name, pdp.Uid, EntityType.ProjectDetailPage);
                                      dependencies.Add(pdpDependency);

                                      // Process PDP
                                      pdp.Children
                                          .Where(webPart => webPart.Children != null)
                                          .ToList()
                                          .ForEach(webPart =>
                                           {
                                               // Skip WebParts
                                               // Process
                                               webPart.Children
                                                  .ToList()
                                                  .ForEach(cf =>
                                                  {
                                                      Dependency customFieldDependency = new Dependency(cf.Name, cf.Uid, EntityType.CustomField);
                                                      dependencies.Add(customFieldDependency);
                                                  });
                                           });
                                  });
                          });
                 });

            return dependencies;
        }
    }
}


