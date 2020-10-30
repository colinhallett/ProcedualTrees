using UnityEngine;

namespace CKZH.ProcedualTree
{
    public class Scale : Module
    {
        [SerializeField] private float scale;

        public override void Generate(Seed seed)
        {
            seed.currentTransform.currentScale *= scale;
            Destroy(this.gameObject);
        }
    }
}
