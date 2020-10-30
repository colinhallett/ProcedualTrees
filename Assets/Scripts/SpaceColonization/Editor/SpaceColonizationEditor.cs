using UnityEngine;
using System.Collections;
using UnityEditor;

namespace CKZH.ProcedualTree.SpaceColonization
{
    [CustomEditor(typeof(SpaceColonization))]
    public class SpaceColonizationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SpaceColonization sC = (SpaceColonization)target;

            DrawDefaultInspector();
            if (GUILayout.Button("Generate"))
            {
                sC.Generate();
            }
        }
    }

}
