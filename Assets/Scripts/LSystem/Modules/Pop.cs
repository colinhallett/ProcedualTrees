namespace CKZH.ProcedualTree
{
    public class Pop : Module
    {
        public override void Generate(Seed seed)
        {
            seed.currentTransform = seed.previousTransforms.Pop();
            
            Destroy(this.gameObject);
        }
    }

}
