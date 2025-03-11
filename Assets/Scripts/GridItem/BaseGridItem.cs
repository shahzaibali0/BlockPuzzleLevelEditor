using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PuzzleLevelEditor.GridItem
{
    [SelectionBase]
    public abstract class BaseGridItem : MonoBehaviour, IBaseGridItem
    {
        [field: SerializeField, ReadOnly] 
        public Vector2Int GridCoords { get; set; }
        public Transform Transform => transform;

        public event Action<BaseGridItem> onDestroy;

        protected void RaiseDestroyEvent(BaseGridItem item)
        {
            onDestroy?.Invoke(item);
        }
    }
}