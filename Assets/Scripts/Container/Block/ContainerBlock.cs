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
        [SerializeField] private List<Container> _subContainers;
        [SerializeField] private List<BorderMesh> _Borders;

        public List<Transform> RightDirRays = new List<Transform>();
        public List<Transform> LeftDirRays = new List<Transform>();
        public List<Transform> FrontDirRays = new List<Transform>();
        public List<Transform> DownDirRays = new List<Transform>();
        public LayerMask LayerMask;
        public Transform LidsBorderParent => _lidsBorderParent.transform;
        public MovementConstraint MovementConstraint => _movementConstraint;
        public BlockColor Color => _blockColor;
        public BlockType BlockType => _blockType;
        public bool BlockPass;
        private List<Transform> rays;
        private List<Transform> BlockExitRays = new List<Transform>();
        private Dictionary<RaycastDirections, List<Transform>> BlockExitRayData = new Dictionary<RaycastDirections, List<Transform>>();
        private RaycastDirections raycastDirections;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.25f);
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

            rays.Clear();
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

        int tryagain = 0;
        public bool EmitRayCastFromAllSides_Old()
        {
            Debug.Log("Mouse Up__EmitRayCastFromAllSides");


            BlockExitRayData.Clear();
            BlockExitRays.Clear();
            bool PointFound = false; ;
            Dictionary<RaycastDirections, List<Transform>> raycastPairs = new Dictionary<RaycastDirections, List<Transform>>()
    {
        { RaycastDirections.Right, RightDirRays },
        { RaycastDirections.Left, LeftDirRays },
        { RaycastDirections.Front, FrontDirRays },
        { RaycastDirections.Down, DownDirRays }
    };

            Debug.Log("Mouse Up__EmitRayCastFromAllSides_A");

            float rayLength = 0.25f; // Increased for testing
            foreach (var pair in raycastPairs)
            {
                RaycastDirections direction = pair.Key;
                List<Transform> directionRays = pair.Value;
                Debug.Log("Mouse Up__EmitRayCastFromAllSides_B");

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

                    if (Physics.Raycast(raycast, out hit, rayLength, LayerMask))
                    {
                        if (hit.collider.TryGetComponent<ExtrationSide>(out ExtrationSide extrationSide))
                        {
                            if (extrationSide.BlockColor == _blockColor)
                            {
                                Debug.Log("ExtrationSide Found in: " + direction + " and saving now");
                                Debug.DrawRay(item.position, item.forward * rayLength, UnityEngine.Color.red, 2);

                                // Save the rays and direction in a dictionary
                                if (!BlockExitRayData.ContainsKey(direction))
                                {
                                    raycastDirections = direction;
                                    BlockExitRays = new List<Transform>(directionRays);
                                    BlockExitRayData[direction] = new List<Transform>(directionRays);
                                }
                            }
                        }
                        else
                        {
                            Debug.DrawRay(item.position, item.forward * rayLength, UnityEngine.Color.gray, 2);

                            PointFound = false;
                        }
                    }
                    else
                    {

                        if (tryagain < 3)
                        {
                            tryagain++;
                        }
                        Debug.Log("Mouse Up__EmitRayCastFromAllSides_Not hit");
                        return false;
                    }
                }
            }


            PointFound = HitForExtration();
            return PointFound;
        }
        public void StartEmitRayCastFromAllSides(Action<bool> callback)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(EmitRayCastFromAllSides(callback));
        }

        private IEnumerator EmitRayCastFromAllSides(Action<bool> callback)
        {
            Debug.Log("Mouse Up__EmitRayCastFromAllSides");

            BlockExitRayData.Clear();
            BlockExitRays.Clear();
            bool PointFound = false;
            int maxRetries = 3; // Maximum number of retries
            int retryCount = 0;

            Dictionary<RaycastDirections, List<Transform>> raycastPairs = new Dictionary<RaycastDirections, List<Transform>>()
    {
        { RaycastDirections.Right, RightDirRays },
        { RaycastDirections.Left, LeftDirRays },
        { RaycastDirections.Front, FrontDirRays },
        { RaycastDirections.Down, DownDirRays }
    };

            Debug.Log("Mouse Up__EmitRayCastFromAllSides_A");

            float rayLength = 0.25f; // Increased for testing

            while (retryCount <= maxRetries)
            {
                bool hitSomething = false;

                foreach (var pair in raycastPairs)
                {
                    RaycastDirections direction = pair.Key;
                    List<Transform> directionRays = pair.Value;

                    if (directionRays.Count == 0)
                    {
                        Debug.LogWarning("No objects in " + direction);
                        continue;
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

                        if (Physics.Raycast(raycast, out hit, rayLength, LayerMask))
                        {
                            if (hit.collider.TryGetComponent<ExtrationSide>(out ExtrationSide extrationSide))
                            {
                                if (extrationSide.BlockColor == _blockColor)
                                {
                                    Debug.Log("ExtrationSide Found in: " + direction + " and saving now");
                                    Debug.DrawRay(item.position, item.forward * rayLength, UnityEngine.Color.red, 2);

                                    if (!BlockExitRayData.ContainsKey(direction))
                                    {
                                        raycastDirections = direction;
                                        BlockExitRays = new List<Transform>(directionRays);
                                        BlockExitRayData[direction] = new List<Transform>(directionRays);
                                    }

                                    PointFound = true;
                                    hitSomething = true;
                                }
                            }
                            else
                            {
                                Debug.DrawRay(item.position, item.forward * rayLength, UnityEngine.Color.gray, 2);
                            }
                        }
                        else
                        {
                            Debug.Log("RayCast Not hitting ");
                        }
                    }
                }

                if (hitSomething) break; // Exit loop if we hit something

                retryCount++;
                Debug.Log($"Retrying raycast attempt {retryCount}/{maxRetries}...");
                yield return new WaitForSeconds(0.1f); // Delay before retrying
            }

            PointFound = HitForExtration();

            // Call the callback function with the result
            callback?.Invoke(PointFound);
        }




        ExtrationSide extrationSide;
        protected bool HitForExtration()
        {
            bool NotFound = false;
            if (BlockExitRays == null || BlockExitRays.Count == 0)
            {
                Debug.LogWarning("Hit__E: BlockExitRays is empty!");
                NotFound = false;
                return NotFound;
            }

            int TotalSideFound = 0;

            foreach (var item in BlockExitRays)
            {

                float RayLength = 0.25f;

                if (item == null)
                {
                    continue;
                }

                Ray raycast = new Ray(item.position, item.forward);
                RaycastHit hit;

                if (Physics.Raycast(raycast, out hit, RayLength, LayerMask))
                {
                    Debug.Log("War gae panday vich" + hit.collider.name);
                    Debug.DrawRay(item.position, item.forward * RayLength, UnityEngine.Color.black, 8);
                    if (hit.collider.TryGetComponent<ContainerBlock>(out ContainerBlock component1))
                    {
                        Debug.Log("War gae panday vich__B" + hit.collider.name);

                        NotFound = false;
                        break;
                    }
                    else
                    {
                        Debug.Log("War gae panday vich_A " + hit.collider.name);

                        RayLength += 0.6f;
                        if (hit.collider.TryGetComponent<ExtrationSide>(out ExtrationSide component))
                        {
                            extrationSide = component;
                            TotalSideFound++;
                        }
                    }
                }
                else
                {

                    Debug.DrawRay(item.position, item.forward * RayLength, UnityEngine.Color.red, 8);
                    Debug.Log("War gae panday vich__C" + item.name);
                    RayLength += 0.6f;

                    if (Physics.Raycast(raycast, out hit, RayLength, LayerMask))
                    {
                        Debug.Log("War gae panday vich__D" + hit.collider.name);

                        if (hit.collider.TryGetComponent<ContainerBlock>(out ContainerBlock component1))
                        {
                            Debug.Log("War gae panday vich__E" + hit.collider.name);

                            NotFound = false;
                            break;

                        }
                        else
                        {
                            Debug.Log("War gae panday vich__F" + hit.collider.name);


                            if (hit.collider.TryGetComponent<ExtrationSide>(out ExtrationSide component))
                            {
                                Debug.Log("War gae panday vich__G" + hit.collider.name);

                                extrationSide = component;
                                TotalSideFound++;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("War gae panday vich__ Raycast Missed: " + item.name);

                    }
                }
            }

            Debug.Log("Hit__E: Total ExtrationSides Found: " + TotalSideFound);

            if (TotalSideFound == BlockExitRays.Count && extrationSide.BlockColor == _blockColor)
            {
                Debug.Log("Move Object To Where it should");
                NotFound = true;

                MoveOutfromGrid();
            }
            else
            {
                NotFound = false;
                Debug.Log("Extrartion Point Not Matched");
            }

            return NotFound;


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
                    Dir = Vector3.back;
                    break;
                default:
                    break;
            }
            Debug.Log("MoveOutfromGrid__");
            StartCoroutine(ExitFromGrid(Dir, 3, 0.5f));
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
            BlockPass = true;

            LevelManager.Instance.LevelInfo.BlockPass?.Invoke();

            gameObject.SetActive(false);
            //transform.position = targetPosition; // Ensure it reaches exactly
        }

    }
}