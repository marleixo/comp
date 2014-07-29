using DMExport.Library;

namespace DMExport.Tests.Entities
{
    internal class FakeSPWorkflowAssociation : ISPWorkflowAssociation
    {
        readonly FakeSPWorkflowTemplate _template = new FakeSPWorkflowTemplate();

        public ISPWorkflowTemplate BaseTemplate
        {
            get { return _template; }
        }
    }
}


