using Framework.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Gameplay.Editor
{
    [CustomEditor(typeof(GameController))]
    public class GameControllerEditor : CustomEditorBase<GameController>
    {
        protected override void DrawInspector()
        {
            base.DrawInspector();
            DrawDefaultInspector();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Game Modes", HeaderStyle);
                if (GUILayout.Button("Add Mode"))
                {
                    RecordObject("Game Controller Change");
                    Target.GameModeSets.Add(new GameModeSet());
                }

                var gameModeSets = serializedObject.FindProperty("GameModeSets");
                var count = gameModeSets.arraySize;
                for (int i = 0; i < count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        var element = gameModeSets.GetArrayElementAtIndex(i);
                        var type = element.FindPropertyRelative("Type");
                        var gameMode = element.FindPropertyRelative("GameMode");

                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.PropertyField(type);
                            EditorGUILayout.PropertyField(gameMode);
                        }
                        EditorGUILayout.EndVertical();

                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            RecordObject("Game Controller Change");
                            Target.GameModeSets.RemoveAt(i);
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