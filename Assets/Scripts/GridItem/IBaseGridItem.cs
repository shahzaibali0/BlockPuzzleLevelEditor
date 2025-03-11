using UnityEngine;

namespace PuzzleLevelEditor.GridItem
{
    public interface IBaseGridItem : IGridItem
    {
        public Transform Transform { get; }
    }
}