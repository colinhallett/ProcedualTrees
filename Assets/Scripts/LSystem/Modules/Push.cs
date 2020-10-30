namespace CKZH.ProcedualTree
{
    public class Push : Module
    {
        public override void Generate(Seed seed)
        {
            var copy = new SavedTransform(seed.currentTransform);
            seed.previousTransforms.Push(copy);
            transform.position = seed.currentTransform.position;
            transform.rotation = seed.currentTransform.rotation;
            transform.SetParent(seed.currentTransform.parent);
            seed.currentTransform.parent = transform;
            seed.currentTransform.depth++;
            if (setStatic)
                gameObject.isStatic = true;
        }
    }

}
