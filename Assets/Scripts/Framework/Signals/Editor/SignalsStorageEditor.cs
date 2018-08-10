using Framework.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Signals.Editor
{
    [CustomEditor(typeof(SignalsStorage))]
    public class SignalsStorageEditor : CustomEditorBase<SignalsStorage>
    {
        protected override void DrawInspector()
        {
            base.DrawInspector();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Signals Storage", HeaderStyle);
                if (GUILayout.Button("Add Signal"))
                {
                    RecordObject("Signals Storage Change");
                    Target.Signals.Add(string.Empty);
                }

                var signals = serializedObject.FindProperty("Signals");
                var count = signals.arraySize;
                for (int i = 0; i < count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        var element = signals.GetArrayElementAtIndex(i);

                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.PropertyField(element, new GUIContent(string.Format("Signal {0}", i + 1)));
                        }
                        EditorGUILayout.EndVertical();

                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            RecordObject("Signals Storage Change");
                            Target.Signals.RemoveAt(i);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}