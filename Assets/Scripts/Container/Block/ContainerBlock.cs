using System;
using System.Collections.Generic;
using PuzzleLevelEditor.BorderLogic;
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
        [SerializeField, ReadOnly] private List<BorderMesh> _Borders;

        public List<Transform> RightDirRays = new List<Transform>();
        public List<Transform> LeftDirRays = new List<Transform>();
        public List<Transform> FrontDirRays = new List<Transform>();
        public List<Transform> DownDirRays = new List<Transform>();

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
        public void AddBorders(BorderMesh c)
        {
            if (!_Borders.Contains(c))
                _Borders.Add(c);
        }

        private void Start()
        {
            GetRayData();
        }

        [Button(ButtonSizes.Medium)]
        public void GetRayData()
        {
            RightDirRays.Clear();
            LeftDirRays.Clear();
            FrontDirRays.Clear();
            DownDirRays.Clear();
            for (int i = 0; i < _Borders.Count; i++)
            {
                // Check and add only if not null
                if (_Borders[i].RayData(RaycastDirections.Right) is Transform rightObj && rightObj != null)
                    RightDirRays.Add(rightObj);

                if (_Borders[i].RayData(RaycastDirections.Left) is Transform leftObj && leftObj != null)
                    LeftDirRays.Add(leftObj);

                if (_Borders[i].RayData(RaycastDirections.Front) is Transform frontObj && frontObj != null)
                    FrontDirRays.Add(frontObj);

                if (_Borders[i].RayData(RaycastDirections.Down) is Transform downObj && downObj != null)
                    DownDirRays.Add(downObj);
            }

        }
        public int rayCount = 0;

        public List<Transform> rays;
        public bool CanProceedInDirection(RaycastDirections directions)
        {


            rays = new List<Transform>();


            switch (directions)
            {
                case RaycastDirections.Right:
                    rays = RightDirRays;
                    break;
                case RaycastDirections.Left:
                    rays = LeftDirRays;

                    break;
                case RaycastDirections.Front:
                    rays = FrontDirRays;

                    break;
                case RaycastDirections.Down:
                    rays = DownDirRays;

                    break;
                default:
                    break;
            }

            bool CanProceed = true;

            foreach (var item in rays)
            {
                RaycastHit raycastHit;
                Ray ray = new Ray(item.position, item.forward);

                if (Physics.Raycast(ray, out raycastHit, 0.25f))
                {
                    Debug.DrawRay(item.position, item.forward, UnityEngine.Color.red);

                    CanProceed = false;
                    break;
                }
                else
                {
                    Debug.DrawRay(item.position, item.forward, UnityEngine.Color.green);

                }

            }
            rayCount = 0;
            return CanProceed;
        }

    }
}