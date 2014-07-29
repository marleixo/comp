using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DMExport.Library.Entities;

namespace DMExport.Library.Helpers
{
    public static class TreeHelper
    {
        /// <summary>
        /// Searches for TreeNode by name (entity UID)
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="name">TreeNode name (entity UID)</param>
        /// <returns>TreeNode if found</returns>
        public static TreeNode GetTreeNodeByName(this TreeView treeView, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return (from TreeNode node in treeView.Nodes
                    select SelectChildTreeNodeByName(node, name))
                .FirstOrDefault(result => result != null);
        }

        /// <summary>
        /// Searches for TreeNode by Text (entity name) in the specified node group
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="text">TreeNode text (entity name)</param>
        /// <param name="parentNodeType">Node Group to search into</param>
        /// <returns>TreeNode if found</returns>
        public static TreeNode GetTreeNodeByText(this TreeView treeView, string text, EntityType parentNodeType)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            return GetAllNodesByGroup(treeView, parentNodeType, false)
                .Where(node => node.Text.ToLower() == text.ToLower())
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects parent tree node by Group
        /// </summary>
        /// <param name="treeView">TreeNode</param>
        /// <param name="parentNodeType">parentNodeType</param>
        /// <returns>TreeNode</returns>
        public static TreeNode GetParentNodeByGroup(this TreeView treeView, EntityType parentNodeType)
        {
            return treeView.Nodes[(int)parentNodeType];
        }

        /// <summary>
        /// Gets selected nodes by Group (EPT, Phases, etc.)
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="entityType">Group</param>
        /// <returns>Collection of TreeNodes</returns>
        private static List<TreeNode> GetSelectedNodesByGroup(this TreeView treeView, EntityType entityType)
        {
            return GetAllNodesByGroup(treeView, entityType, true);
        }

        /// <summary>
        /// Gets nodes by Group (EPT, Phases, etc.)
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="entityType">Group</param>
        /// <param name="onlyChecked">Selected only checked nodes</param>
        /// <returns>Collection of TreeNodes</returns>
        private static List<TreeNode> GetAllNodesByGroup(this TreeView treeView, EntityType entityType, bool onlyChecked)
        {
            var nodes = treeView.Nodes[(int)entityType].Nodes
                .Cast<TreeNode>();

            if (onlyChecked)
            {
                nodes = nodes.Where(node => node.Checked);
            }

            return nodes.ToList();
        }

        /// <summary>
        /// Make TreeNodes checked according to TreeSelection passed.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="treeSelection">TreeSelection</param>
        public static void DoSelectTreeNodes(this TreeView treeView, TreeSelection treeSelection)
        {
            // Uncheck all
            treeView.Nodes
                .Cast<TreeNode>()
                .ToList()
                .ForEach(node => node.Nodes.Cast<TreeNode>().ToList().ForEach(n => n.Checked = false));

            // Apply
            treeSelection.SelectedEpts.ForEach(node => node.Checked = true);
            treeSelection.SelectedPhases.ForEach(node => node.Checked = true);
            treeSelection.SelectedStages.ForEach(node => node.Checked = true);
            treeSelection.SelectedCustomFields.ForEach(node => node.Checked = true);
            treeSelection.SelectedLookupTables.ForEach(node => node.Checked = true);
            treeSelection.SelectedPdps.ForEach(node => node.Checked = true);
        }

        /// <summary>
        /// Searches Child Tree Node by given Name
        /// </summary>
        /// <param name="treeNode">Parent TreeNode</param>
        /// <param name="name">Child Node name</param>
        /// <returns>Child TreeNode if found</returns>
        private static TreeNode SelectChildTreeNodeByName(TreeNode treeNode, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return treeNode.Nodes
                .Cast<TreeNode>()
                .FirstOrDefault(node => node.Name.ToLower() == name.ToLower());
        }

        #region Tree Selection

        /// <summary>
        /// Gets all selected nodes and fill TreeSelection object.
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <returns>TreeSelection object</returns>
        public static TreeSelection GetSelectedNodes(this TreeView treeView)
        {
            return new TreeSelection
                       {
                           SelectedEpts = treeView.GetSelectedNodesByGroup(EntityType.Ept),
                           SelectedPhases = treeView.GetSelectedNodesByGroup(EntityType.Phase),
                           SelectedStages = treeView.GetSelectedNodesByGroup(EntityType.Stage),
                           SelectedCustomFields = treeView.GetSelectedNodesByGroup(EntityType.CustomField),
                           SelectedLookupTables = treeView.GetSelectedNodesByGroup(EntityType.LookupTable),
                           SelectedPdps = treeView.GetSelectedNodesByGroup(EntityType.ProjectDetailPage)
                       };
        }

        public class TreeSelection
        {
            public List<TreeNode> SelectedEpts { get; set; }
            public List<TreeNode> SelectedPhases { get; set; }
            public List<TreeNode> SelectedStages { get; set; }
            public List<TreeNode> SelectedCustomFields { get; set; }
            public List<TreeNode> SelectedLookupTables { get; set; }
            public List<TreeNode> SelectedPdps { get; set; }

            public TreeSelection()
            {
                SelectedEpts = new List<TreeNode>();
                SelectedPhases = new List<TreeNode>();
                SelectedStages = new List<TreeNode>();
                SelectedCustomFields = new List<TreeNode>();
                SelectedLookupTables = new List<TreeNode>();
                SelectedPdps = new List<TreeNode>();
            }
        }

        #endregion
    }
}


