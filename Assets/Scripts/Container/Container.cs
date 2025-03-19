using System;
using PuzzleLevelEditor.Container.Block;
using PuzzleLevelEditor.Container.Item;
using PuzzleLevelEditor.GridItem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PuzzleLevelEditor.Container
{
    public class Container : BaseGridItem
    {

        [SerializeField, ReadOnly] private ContainerInfo _containerInfo;
        [SerializeField] private ContainerItem[] _currentItems = new ContainerItem[4];

        [Header("References")]
        [SerializeField] protected ContainerPart[] _parts;

        public ContainerInfo Info => _containerInfo;

        public void SetContainerInfo(ContainerInfo info)
        {
            _containerInfo = info;
        }

        public Vector3 GetPartPosition(int index, ContainerItem item)
        {
            index %= _parts.Length;

            _currentItems[index] = item;
            return _parts[index].Position;
        }


 
    }
}

[Serializable]
public class ContainerInfo
{
    public ContainerBlock ParentBlock;
    public Vector2Int Offset;

    public ContainerInfo(ContainerBlock parentBlock, Vector2Int offset)
    {
        ParentBlock = parentBlock;
        Offset = offset;
    }
}