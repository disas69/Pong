using Framework.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Configuration.Editor
{
    [CustomEditor(typeof(BallSettingsStorage))]
    public class BallSettingsStorageEditor : CustomEditorBase<BallSettingsStorage>
    {
        protected override void DrawInspector()
        {
            base.DrawInspector();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Ball Settings Storage", HeaderStyle);
                if (GUILayout.Button("Add Ball Settings"))
                {
                    RecordObject("Ball Settings Storage Change");
                    Target.BallSettings.Add(new BallSettings());
                }

                var ballSettings = serializedObject.FindProperty("BallSettings");
                var count = ballSettings.arraySize;
                for (int i = 0; i < count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        var element = ballSettings.GetArrayElementAtIndex(i);
                        var ballName = element.FindPropertyRelative("Name");
                        var size = element.FindPropertyRelative("Size");
                        var startSpeed = element.FindPropertyRelative("StartSpeed");
                        var bouncingSpeed = element.FindPropertyRelative("BouncingSpeed");
                        var view = element.FindPropertyRelative("View");

                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.PropertyField(ballName);
                            EditorGUILayout.PropertyField(size);
                            EditorGUILayout.PropertyField(startSpeed);
                            EditorGUILayout.PropertyField(bouncingSpeed);
                            EditorGUILayout.PropertyField(view);
                        }
                        EditorGUILayout.EndVertical();

                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            RecordObject("Ball Settings Storage Change");
                            Target.BallSettings.RemoveAt(i);
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