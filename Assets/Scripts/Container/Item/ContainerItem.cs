using UnityEngine;

namespace PuzzleLevelEditor.Container.Item
{
    public class ContainerItem : MonoBehaviour
    {
        [SerializeField] private BlockColor _itemColor;
        [SerializeField] private Transform _tr;
    
        public BlockColor ItemColor => _itemColor;
    }
}