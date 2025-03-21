using System;
using System.Collections;
using System.Collections.Generic;
using PuzzleLevelEditor.BorderLogic;
using PuzzleLevelEditor.GridItem;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
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
        private List<Transform> rays;
        private List<Transform> BlockExitRays;
        private Dictionary<RaycastDirections, List<Transform>> BlockExitRayData = new Dictionary<RaycastDirections, List<Transform>>();
        private RaycastDirections raycastDirections;

        private void Start()
        {
            GetRayData();
        }
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
            return CanProceed;
        }
        public void EmitRayCastFromAllSides()
        {
            Dictionary<RaycastDirections, List<Transform>> raycastPairs = new Dictionary<RaycastDirections, List<Transform>>()
    {
        { RaycastDirections.Right, RightDirRays },
        { RaycastDirections.Left, LeftDirRays },
        { RaycastDirections.Front, FrontDirRays },
        { RaycastDirections.Down, DownDirRays }
    };
            float rayLength = 0.5f; // Increased for testing
            foreach (var pair in raycastPairs)
            {
                RaycastDirections direction = pair.Key;
                List<Transform> directionRays = pair.Value;

                if (directionRays.Count == 0)
                {
                    Debug.LogWarning("No objects in " + direction);
                    continue; // Skip empty lists
                }
                foreach (var item in directionRays)
                {
                    if (item == null)
                    {
                        Debug.LogError("Null transform in " + direction);
                        continue;
                    }
                    RaycastHit hit;
                    Ray raycast = new Ray(item.position, item.forward);

                    if (Physics.Raycast(raycast, out hit, rayLength))
                    {
                        Debug.Log("total pairs__B " + direction);

                        if (hit.collider.GetComponent<ExtrationSide>())
                        {
                            Debug.Log("ExtrationSide Found in: " + direction + " and saving now");

                            // Save the rays and direction in a dictionary
                            if (!BlockExitRayData.ContainsKey(direction))
                            {
                                raycastDirections = direction;
                                BlockExitRays = new List<Transform>(directionRays);
                                BlockExitRayData[direction] = new List<Transform>(directionRays);
                            }
                        }
                    }
                }
            }
            HitForExtration();
        }
        ExtrationSide extrationSide;
        protected void HitForExtration()
        {
            if (BlockExitRays == null || BlockExitRays.Count == 0)
            {
                Debug.LogWarning("Hit__E: BlockExitRays is empty!");
                return;
            }

            int TotalSideFound = 0;
            //Debug.Log("Hit__E: Total Rays = " + BlockExitRays.Count);
            float RayLength = 0.25f; // Reset per ray

            foreach (var item in BlockExitRays)
            {
                if (item == null)
                {
                    //  Debug.LogError("Null transform found in BlockExitRays");
                    continue;
                }

                //Debug.Log("Hit__E: Checking Ray: " + item.name + " Ray Length " + RayLength);

                Ray raycast = new Ray(item.position, item.forward);
                RaycastHit hit;

                if (Physics.Raycast(raycast, out hit, RayLength))
                {
                    // Debug.Log("Hit__E: Raycast Hit: " + hit.collider.name);
                    Debug.DrawRay(item.position, item.forward * RayLength, UnityEngine.Color.black, 8);
                    RayLength += 0.6f;
                    if (hit.collider.TryGetComponent<ExtrationSide>(out ExtrationSide component))
                    {
                        // Debug.Log("Hit__E:  - ExtrationSide Found");
                        extrationSide = component;
                        TotalSideFound++;
                    }
                }
                else
                {
                    //  Debug.DrawRay(item.position, item.forward * RayLength, UnityEngine.Color.red, 8);
                    // Debug.Log("Hit__E: Raycast Missed: " + item.name);
                }
            }

            Debug.Log("Hit__E: Total ExtrationSides Found: " + TotalSideFound);

            if (TotalSideFound == BlockExitRays.Count && extrationSide.BlockColor == _blockColor)
            {
                Debug.Log("Move Object To Where it should");
                MoveOutfromGrid();
            }
            else
            {
                Debug.LogError("Extrartion Point Not Matched");
            }
        }

        private void MoveOutfromGrid()
        {

            Debug.Log("MoveOutfromGrid");
            Vector3 Dir = Vector3.zero;

            switch (raycastDirections)
            {
                case RaycastDirections.Right:
                    Dir = Vector3.right;
                    break;
                case RaycastDirections.Left:
                    Dir = Vector3.left;
                    break;
                case RaycastDirections.Front:
                    Dir = Vector3.forward;
                    break;
                case RaycastDirections.Down:
                    Dir = Vector3.down;
                    break;
                default:
                    break;
            }
            Debug.Log("MoveOutfromGrid__");
            StartCoroutine(ExitFromGrid(Dir, 5, 0.85f));
        }
        private IEnumerator ExitFromGrid(Vector3 direction, float moveDistance, float duration)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + (direction.normalized * moveDistance);

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration; // Normalize time (0 to 1)
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition; // Ensure it reaches exactly
        }

    }
}