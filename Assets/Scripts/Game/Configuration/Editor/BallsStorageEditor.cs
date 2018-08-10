using Framework.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Configuration.Editor
{
    [CustomEditor(typeof(BallsStorage))]
    public class BallsStorageEditor : CustomEditorBase<BallsStorage>
    {
        protected override void DrawInspector()
        {
            base.DrawInspector();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Balls Storage", HeaderStyle);
                if (GUILayout.Button("Add Ball"))
                {
                    RecordObject("Balls Storage Change");
                    Target.Balls.Add(null);
                }

                var balls = serializedObject.FindProperty("Balls");
                var count = balls.arraySize;
                for (int i = 0; i < count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        var element = balls.GetArrayElementAtIndex(i);
                        var elementName = element.objectReferenceValue != null ? element.objectReferenceValue.name : "None";

                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.PropertyField(element, new GUIContent(elementName));
                        }
                        EditorGUILayout.EndVertical();

                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            RecordObject("Balls Storage Change");
                            Target.Balls.RemoveAt(i);
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