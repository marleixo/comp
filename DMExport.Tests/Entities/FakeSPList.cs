using System;
using System.Collections.Generic;
using DMExport.Library;

namespace DMExport.Tests.Entities
{
    internal class FakeSPList : ISPList
    {
        internal List<ISPListItem> _items = new List<ISPListItem>();

        public FakeSPList()
        {
            ID = Guid.NewGuid();
        }

        public IList<ISPListItem> Items
        {
            get { return _items; }
        }

        public Guid ID
        {
            get;
            internal set;
        }
    }
}


