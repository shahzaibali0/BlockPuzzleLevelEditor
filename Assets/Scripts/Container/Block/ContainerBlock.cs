using System.Collections.Generic;
using PuzzleLevelEditor.GridItem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PuzzleLevelEditor.Container.Block
{
    [SelectionBase]
    public class ContainerBlock : BaseGridItem
    {
        [Header("BorderParents")]
        [SerializeField] private GameObject _lidsBorderParent; 
        [SerializeField] private MovementConstraint _movementConstraint;

        [Header("Set By Level Editor")]
        [SerializeField, ReadOnly] private BlockColor _blockColor;
        [SerializeField, ReadOnly] private BlockType _blockType;
        [SerializeField, ReadOnly] private List<Container> _subContainers;

        public Transform LidsBorderParent => _lidsBorderParent.transform;
        public MovementConstraint MovementConstraint => _movementConstraint;
        public BlockColor Color => _blockColor;
        public BlockType BlockType => _blockType;

        public void SetBlockInformation(BlockRelatedInformation information)
        {
            _blockColor = information.Color;
            _movementConstraint = information.MovementConstraint;
            _blockType = information.Type;
        }

        public void AddSubContainer(Container c)
        {
            if (!_subContainers.Contains(c))
                _subContainers.Add(c);
        }
    }
}