using BookEffectV3;
using UnityEditor;
using UnityEngine;

namespace eWolf.BookEffect
{
    [CustomEditor(typeof(BookMeshBuilder)), CanEditMultipleObjects]
    public class BookMeshBuilder_UI : Editor
    {
        private BookMeshBuilder _node;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUI.color = Color.green;
            if (GUILayout.Button("Build Book"))
            {
                _node.BuildMesh();
                EditorUtility.SetDirty(target);
            }

            GUI.color = Color.yellow;
            if (GUILayout.Button("Close"))
            {
                _node.CloseEditorBook();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Open"))
            {
                _node.OpenEditorBook();
                EditorUtility.SetDirty(target);
            }
        }

        private void OnEnable()
        {
            _node = (BookMeshBuilder)target;
        }
    }
}