using UnityEngine;

namespace CKZH.ProcedualTree
{
    public class SavedTransform
    {
        public SavedTransform(Vector3 pos, Quaternion rot, Transform par, float scale, int depth, float currentBranchWidth)
        {
            position = pos;
            rotation = rot;
            parent = par;
            currentScale = scale;
            this.depth = depth;
            this.currentBranchWidth = currentBranchWidth;
        }
        public SavedTransform(SavedTransform copy)
        {
            position = copy.position;
            rotation = copy.rotation;
            parent = copy.parent;
            currentScale = copy.currentScale;
            depth = copy.depth;
            currentBranchWidth = copy.currentBranchWidth;
        }

        public Transform parent;
        public Vector3 position;
        public Quaternion rotation;
        public float currentScale;
        public int depth;
        public float currentBranchWidth;
    }
}
