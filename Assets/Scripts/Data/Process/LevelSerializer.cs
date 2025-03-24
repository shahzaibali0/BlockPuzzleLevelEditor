using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace PuzzleLevelEditor.Data.Process
{
    public class LevelSerializer : IParser<LevelData>
    {
        public static TextAsset SaveLevel(LevelData levelData, string relativeDirectory)
        {
            string json = JsonUtility.ToJson(levelData, false);
            File.WriteAllText($"{Application.dataPath}{relativeDirectory}.json", json);
#if UNITY_EDITOR        
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/{relativeDirectory}.json");

#endif

            return null;
        }

        public LevelData ParseData(string file)
        {
            return JsonUtility.FromJson<LevelData>(file);
        }
    }
}
