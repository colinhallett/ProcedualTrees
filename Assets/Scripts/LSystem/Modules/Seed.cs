using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CKZH.ProcedualTree
{
    public class Seed : Module
    {
        [SerializeField] private bool growOnPlay;
        [SerializeField] private bool instant;
        [SerializeField] private float drawTime = 0.1f;
        [SerializeField] private float growTime = 20f;
        [SerializeField] private float startScale;
        [SerializeField] private float angleIterationAffect;
        [SerializeField] private bool inheritScale;
        [SerializeField] private string axiom;
        [SerializeField] private int iterations;
        [SerializeField] private RulesDict rules;
        [SerializeField] private ImplementationDict implementation;
        

        private string sentence;

        public SavedTransform currentTransform;
        public Stack<SavedTransform> previousTransforms;

        private List<Module> modules = new List<Module>();

        public bool WasMinRotate { get; set; }

        public float AngleIncrease => currentTransform.depth * angleIterationAffect;

        private void Awake()
        {
            previousTransforms = new Stack<SavedTransform>();
        }

        private void Start()
        {
            if (growOnPlay)
                Grow();
        }
        [ContextMenu("Grow")]
        private void Grow()
        {
            if (instant)
                Generate(this);
            else
                StartCoroutine(Gen(this));
        }

        private IEnumerator Gen(Seed seed)
        {
            Generate(seed);

            float current = 0f;

            while (current < growTime)
            {
                for (int i = 0; i < modules.Count; i++)
                {
                    if (modules[i] != null)
                    {
                        var easing = Easing.Exponential.InOut(current / growTime);
                        modules[i].transform.localScale = Vector3.one * easing;
                    }
                        
                }
                
                current += Time.deltaTime;
                yield return null;
            }
        }

        public override void Generate(Seed seed)
        {
            if (seed == this)
                currentTransform = new SavedTransform(seed.transform.position, seed.transform.rotation, seed.transform, startScale, 0, startScale);
            else
                currentTransform = new SavedTransform(
                    seed.currentTransform.position,
                    seed.currentTransform.rotation,
                    seed.currentTransform.parent,
                    inheritScale ? seed.currentTransform.currentScale : startScale,
                    seed.currentTransform.depth,
                    inheritScale ? seed.currentTransform.currentBranchWidth : startScale);

            transform.position = currentTransform.position;
            transform.rotation = currentTransform.rotation;

            if (currentTransform.parent != transform)
                transform.SetParent(currentTransform.parent);

            currentTransform.parent = transform;

            sentence = LSystem.GenerateSentence(axiom, iterations, rules);

            for (int i = 0; i < sentence.Length; i++)
            {
                if (implementation.ContainsKey(sentence[i]))
                {
                    var mod = Instantiate(implementation[sentence[i]]);
                    if (mod is Seed seed1 && !seed1.instant)
                    {
                        mod.StartCoroutine(seed1.Gen(this));
                    } else
                    {
                        mod.Generate(this);
                    }

                    
                    modules.Add(mod);
                }
                    
            }
            if (setStatic)
                gameObject.isStatic = true;
        }
    }
}
