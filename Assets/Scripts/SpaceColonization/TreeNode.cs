using System.Collections.Generic;
using UnityEngine;

namespace CKZH.ProcedualTree.SpaceColonization
{
    public class TreeNode
    {
        public Vector3 start;
        public Vector3 end;
        public Vector3 direction;
        public TreeNode parent;
        public float radius = 0;

        public bool hasConnection;

        public List<TreeNode> children = new List<TreeNode>();
        public List<AttractionPoint> attractors = new List<AttractionPoint>();
        public int verticesId;

        public TreeNode(Vector3 start, Vector3 end, Vector3 direction, TreeNode parent = null)

        {
            this.start = start;
            this.end = end;
            this.direction = direction;
            this.parent = parent;

            UpdateChildren(parent);
        }

        void UpdateChildren(TreeNode parent)
        {
            var before = parent;
            var child = this;

            while (before != null)
            {
                before.children.Add(child);
                child = before;
                before = before.parent;
            }
        }
    }
}
