using eWolf.BookEffectV2;
using UnityEditor;
using UnityEngine;

namespace eWolf.BookEffect
{
    [CustomEditor(typeof(Book))]
    public class Book_UI : Editor
    {
        private Book _node;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Label("NOTE: Starting Page numbers has to be even number.");

            if (_node.Details.GetStartingPage % 2 == 1)
            {
                _node.Details.GetStartingPage += 1;
            }
        }

        private void OnEnable()
        {
            _node = (Book)target;
        }
    }
}