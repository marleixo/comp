using System;
using System.Collections.Generic;
using DMExport.Library.Entities;

namespace DMExport.Library.Services
{
    public interface IEptAssociationService
    {
        bool IsEptAssociationToolInstalled { get; }
        bool IsEptWorkflowControlled(Guid eptUid);
        IEnumerable<Guid> GetPdpCustomFieldsUids(Guid pdpUid);
        IEnumerable<Dependency> GetWorkflowControlledEptDependencies(Guid eptUid);
    }
}