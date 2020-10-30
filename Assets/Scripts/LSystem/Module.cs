using UnityEngine;

namespace CKZH.ProcedualTree
{
    public abstract class Module : MonoBehaviour
    {
        [SerializeField] protected bool setStatic;

        public abstract void Generate(Seed seed);
    }
}
