using System;
using DMExport.Library;

namespace DMExport.Tests.Entities
{
    internal class FakeSPWorkflowAssociationCollection : ISPWorkflowAssociationCollection
    {
        public ISPWorkflowAssociation this[Guid uid]
        {
            get { return new FakeSPWorkflowAssociation(); }
        }
    }
}


