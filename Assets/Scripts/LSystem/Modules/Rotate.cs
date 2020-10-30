using UnityEngine;

namespace CKZH.ProcedualTree
{
    public class Rotate : Module
    {
        [SerializeField] private Vector3 minRot;
        [SerializeField] private Vector3 maxRot;

        [SerializeField] private bool binary;

        public override void Generate(Seed seed)
        {
            transform.SetParent(seed.currentTransform.parent);

            Vector3 to;

            if (!binary)
                to = new Vector3(Random.Range(minRot.x, maxRot.x),
                 Random.Range(minRot.y, maxRot.y),
                 Random.Range(minRot.z, maxRot.z));
            else
            {
                to = seed.WasMinRotate ? maxRot : minRot;
                seed.WasMinRotate = !seed.WasMinRotate;
                
            }
            to += (to * Mathf.Deg2Rad * seed.AngleIncrease);
            transform.position = seed.currentTransform.position;
            seed.currentTransform.rotation *= Quaternion.Euler(to) ;
            Destroy(this.gameObject);
        }
    }
}
