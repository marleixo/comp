using System.ComponentModel;

namespace DMExport.Library.Entities
{
    public enum EntityType
    {
        [Description("EPTs")]
        Ept,
        
        [Description("Phases")]
        Phase,
        
        [Description("Stages")]
        Stage,
        
        [Description("Project Detail Pages")]
        ProjectDetailPage,
        
        [Description("Custom Fields")]
        CustomField,
        
        [Description("Lookup Tables")]
        LookupTable
    }
}


