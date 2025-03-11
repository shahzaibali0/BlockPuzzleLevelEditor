using PuzzleLevelEditor.Grid;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PuzzleLevelEditor.Gameplay
{
    public class PuzzleGridManager : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private Grid<Container.Container> _containerGrid;
    
        [SerializeField, ReadOnly]
        private float _cellSize = 1f;
    
        public void SetGridInformation(Grid<Container.Container> containerGrid, float cellSize)
        {
            _containerGrid = containerGrid;
            _cellSize = cellSize;
        }
    }
}