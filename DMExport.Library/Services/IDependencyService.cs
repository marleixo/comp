using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DMExport.Library.Entities;

namespace DMExport.Library.Services
{
    public interface IDependencyService
    {
        /// <summary>
        /// Gets dependency for a stage
        /// </summary>
        /// <param name="stageUid">Workflow Stage UID</param>
        /// <returns>Dependency instance</returns>
        Dependency GetStageDependencies(Guid stageUid);

        /// <summary>
        /// Gets dependency for a custom field
        /// </summary>
        /// <param name="cfUid">Custom Field UID</param>
        /// <returns>Dependency instance</returns>
        Dependency GetCustomFieldDependencies(Guid cfUid);

        /// <summary>
        /// Gets dependency for a PDP
        /// </summary>
        /// <param name="pdpUid">PDP UID</param>
        /// <returns>Dependency instance</returns>
        Dependency GetPdpDependencies(Guid pdpUid);

        /// <summary>
        /// Gets dependency for a EPT
        /// </summary>
        /// <param name="eptUid">EPT UID</param>
        /// <returns>Dependency instance</returns>
        Dependency GetEptDependencies(Guid eptUid);

        /// <summary>
        /// Fills a list of warnings during export.
        /// </summary>
        /// <param name="treeView">Treeview object</param>
        void CheckDependenciesOnExport(TreeView treeView);
    }
}
