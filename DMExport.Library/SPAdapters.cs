using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint;

namespace DMExport.Library
{
    public interface ISPList
    {
        IList<ISPListItem> Items { get; }
        Guid ID { get; }
    }

    public interface ISPListItem
    {
        object this[string name] { get; }
        Guid UniqueId { get; }
    }

    public class SPListAdapter : ISPList
    {
        SPList _list;

        public SPListAdapter(SPList list)
        {
            _list = list;
        }

        public IList<ISPListItem> Items
        {
            get
            {
                return _list.Items
                    .OfType<SPListItem>()
                    .Select(i => new SPListItemAdapter(i))
                    .Cast<ISPListItem>()
                    .ToList();
            }
        }

        public Guid ID
        {
            get
            {
                return _list.ID;
            }
        }
    }

    public class SPListItemAdapter : ISPListItem
    {
        SPListItem _item;

        public SPListItemAdapter(SPListItem item)
        {
            _item = item;
        }

        public object this[string name]
        {
            get { return _item[name]; }
        }

        public Guid UniqueId
        {
            get { return _item.UniqueId; }
        }
    }

    public interface ISPWorkflowAssociationCollection
    {
        ISPWorkflowAssociation this[Guid uid] { get; }
    }

    public interface ISPWorkflowAssociation
    {
        ISPWorkflowTemplate BaseTemplate { get; }
    }

    public interface ISPWorkflowTemplate
    {
        string Name { get; }
    }

    public class SPWorkflowAssociationCollectionAdapert : ISPWorkflowAssociationCollection
    {
        SPWorkflowAssociationCollection _collection;

        public SPWorkflowAssociationCollectionAdapert(
            SPWorkflowAssociationCollection collection)
        {
            _collection = collection;
        }

        public ISPWorkflowAssociation this[Guid uid]
        {
            get
            {
                return new SPWorkflowAssociationAdapert(
                    _collection[uid]);
            }
        }
    }

    public class SPWorkflowAssociationAdapert : ISPWorkflowAssociation
    {
        SPWorkflowAssociation _association;

        public SPWorkflowAssociationAdapert(
            SPWorkflowAssociation association)
        {
            _association = association;
        }

        public ISPWorkflowTemplate BaseTemplate
        {
            get
            {
                return new SPWorkflowTemplateAdapert(
                    _association.BaseTemplate);
            }
        }
    }

    public class SPWorkflowTemplateAdapert : ISPWorkflowTemplate
    {
        SPWorkflowTemplate _template;

        public SPWorkflowTemplateAdapert(SPWorkflowTemplate template)
        {
            _template = template;
        }

        public string Name
        {
            get { return _template.Name; }
        }
    }
}
