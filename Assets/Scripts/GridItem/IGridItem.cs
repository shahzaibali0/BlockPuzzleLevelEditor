using UnityEngine;

namespace PuzzleLevelEditor.GridItem
{
    public interface IGridItem
    {
        public Vector2Int GridCoords { get; set; }
    }
}