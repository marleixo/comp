using System;
using System.Collections.Generic;
using DMExport.Library;

namespace DMExport.Tests.Entities
{
    internal class FakeSPListItem : ISPListItem
    {
        internal Dictionary<string, object> _fields = new Dictionary<string, object>();

        public FakeSPListItem(string titlePDP)
        {
            _fields.Add("TitlePDP", titlePDP);
            UniqueId = Guid.NewGuid();
        }

        public object this[string name]
        {
            get { return _fields[name]; }
        }

        public Guid UniqueId
        {
            get;
            private set;
        }

        public int ID
        {
            get { return 1; }
        }
    }
}


