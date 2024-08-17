using eWolf.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace eWolf.Common.Helper
{
    [CustomEditor(typeof(SceneHelpers)), CanEditMultipleObjects]
    public class SceneHelpers_UI : Editor
    {
        public void DefaultStateAll()
        {
            IEnumerable<ISetDefaultState> fixers = FindObjectsOfType<MonoBehaviour>().OfType<ISetDefaultState>();
            foreach (ISetDefaultState fix in fixers)
            {
                fix.SetDefaultState();
            }
        }

        public void FixAll()
        {
            IEnumerable<IFixAsset> fixers = FindObjectsOfType<MonoBehaviour>().OfType<IFixAsset>();
            foreach (IFixAsset fix in fixers)
            {
                fix.Fix();
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Randomize All"))
            {
                RandomizeAll();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Randomize Visual"))
            {
                RandomizeAllVisual();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Fix all"))
            {
                FixAll();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Set the default state"))
            {
                DefaultStateAll();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Set Bake Lighting "))
            {
                SetAllBakeLighting();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Generate Lighting"))
            {
                Lightmapping.BakeAsync();
            }
        }

        private void RandomizeAll()
        {
            IEnumerable<IRandomizer> randomizers = FindObjectsOfType<MonoBehaviour>().OfType<IRandomizer>();
            foreach (IRandomizer randomizer in randomizers)
            {
                if (randomizer.IsLocked)
                    continue;
                randomizer.Randomize();
            }
        }

        private void RandomizeAllVisual()
        {
            IEnumerable<IRandomizer> randomizers = FindObjectsOfType<MonoBehaviour>().OfType<IRandomizer>();
            foreach (IRandomizer randomizer in randomizers)
            {
                if (randomizer.IsLocked)
                    continue;
                randomizer.RandomizeVisual();
            }
        }

        private void SetAllBakeLighting()
        {
            IEnumerable<ISetLighting> randomizers = FindObjectsOfType<MonoBehaviour>().OfType<ISetLighting>();
            foreach (ISetLighting randomizer in randomizers)
            {
                randomizer.SetBakedLighting();
            }
        }
    }
}