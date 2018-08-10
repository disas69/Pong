using Game.Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class GameDataEditor
    {
        [MenuItem("Game/Delete GameData")]
        public static void ClearData()
        {
            FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/" + GameData.FileName);
        }
    }
}