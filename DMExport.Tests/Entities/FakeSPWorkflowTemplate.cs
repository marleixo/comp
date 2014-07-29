using DMExport.Library;

namespace DMExport.Tests.Entities
{
    internal class FakeSPWorkflowTemplate : ISPWorkflowTemplate
    {
        public string Name
        {
            get { return "Template Name"; }
        }
    }
}