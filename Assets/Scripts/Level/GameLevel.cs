using Sirenix.OdinInspector;
using UnityEngine;

namespace PuzzleLevelEditor.LevelVisualization
{
    public class GameLevel : MonoBehaviour
    {
        [SerializeField, ReadOnly] private TextAsset _relatedLevelAsset;

        public TextAsset LevelAsset => _relatedLevelAsset;

        public void SetLevelAsset(TextAsset asset)
        {
            _relatedLevelAsset = asset;
        }
    }
}