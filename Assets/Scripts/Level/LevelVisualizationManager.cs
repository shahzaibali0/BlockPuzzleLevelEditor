using System.Collections.Generic;
using System.Linq;
using PuzzleLevelEditor.BorderLogic;
using PuzzleLevelEditor.Container.Block;
using PuzzleLevelEditor.Container.Item;
using PuzzleLevelEditor.Data;
using PuzzleLevelEditor.Data.Process;
using PuzzleLevelEditor.Gameplay;
using PuzzleLevelEditor.Grid;
using PuzzleLevelEditor.GridItem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace PuzzleLevelEditor.LevelVisualization
{
    public class LevelVisualizationManager : MonoBehaviour
    {
        [SerializeField] private LevelVisualizationData _levelVisualizationData;
        [SerializeField] private ContainerBlockSet _containerBlockSet;
        [SerializeField] private BorderPrefabSet _gridBorderPrefabSet;
        [SerializeField] private BorderPrefabSet _containerBorderPrefabSet;
        [SerializeField] private BorderPrefabSet _lidsBorderPrefabSet;
        [SerializeField] private BorderPrefabSet _lockedLidsBorderPrefabSet;
        [SerializeField] private ContainerItemSet _containerItemSet;

        private readonly float _cellSize = 0.5f;

        private readonly BorderSpawner _borderSpawner = new();
        private readonly LevelSerializer _levelSerializer = new();

        private static string s_savePath = "Assets/Prefabs/Levels/";
        private static string s_relativePath = "/Prefabs/Levels/";

        [Button]
        public void VisualizeLevel(TextAsset levelAsset)
        {
            //Try to find if the asset is already in the project

#if UNITY_EDITOR
            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { s_savePath });

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var l = AssetDatabase.LoadAssetAtPath<GameLevel>(path);

                if (!l || l.LevelAsset != levelAsset) continue;

                PrefabUtility.InstantiatePrefab(l);
                return;
            }

#endif
            //If not we spawn and save it

            var level = _levelSerializer.ParseData(levelAsset.text);
            var grid = level.CellGrid;

            if (grid is { Width: > 0, Height: > 0 })
                SpawnSelectedPieces(grid);
            else
                Debug.LogWarning("Cannot parse grid");
        }

        public void SpawnSelectedPieces(Grid<CellData> cellGrid)
        {
#if UNITY_EDITOR
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Levels"))
                s_savePath = EditorUtility.OpenFolderPanel("Select Save Path", "", "");

            if (string.IsNullOrWhiteSpace(s_savePath))
                return;

            int width = cellGrid.Width;
            int height = cellGrid.Height;

            // Create puzzle manager
            GameObject levelObject = new GameObject(SceneManager.GetActiveScene().name);
            PuzzleGridManager gridManager = levelObject.AddComponent<PuzzleGridManager>();
            GameLevel level = levelObject.AddComponent<GameLevel>();
            LevelInfo levelInfo = levelObject.AddComponent<LevelInfo>();
            levelInfo.WidthBlocks = width;
            levelInfo.HeightBlocks = height;
            Grid<Container.Container> containerGrid = new Grid<Container.Container>(width, height);

            // 1) Instantiate ground for each playable cell
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int coords = new Vector2Int(x, y);
                    if (!cellGrid.TryGetItem(coords, out var cellData))
                    {
                        containerGrid.SetAvailability(coords, false);
                        continue;
                        // hole => skip
                    }

                    Vector3 worldPosition = GetWorldPosition(coords, width, height, _cellSize);

                    if (_levelVisualizationData.GroundItem != null)
                    {
                        // Find or Create Parent inside the prefab
                        GameObject parentGO = levelObject.transform.Find("GroundParent")?.gameObject;

                        if (parentGO == null)
                        {
                            parentGO = new GameObject("GroundParent");
                            parentGO.transform.SetParent(levelObject.transform); // Set as a child of the prefab
                        }


                        // Instantiate the Ground Object as a Child of Parent
                        var groundGO = PrefabUtility.InstantiatePrefab(_levelVisualizationData.GroundItem, parentGO.transform) as GameObject;

                        if (groundGO != null)
                        {
                            groundGO.transform.position = worldPosition;
                            groundGO.name = $"Ground_{x}_{y}";
                        }
                    }

                    var borderInfo = _borderSpawner.GetBorderTypeWithRotation(cellGrid, coords);
                    var borderPrefab = _gridBorderPrefabSet.GetDataByEnum(borderInfo.Item1);

                    // Find or Create Parent inside the prefab
                    GameObject BorderParent = levelObject.transform.Find("BorderParent")?.gameObject;

                    if (BorderParent == null)
                    {
                        BorderParent = new GameObject("BorderParent");
                        BorderParent.transform.SetParent(levelObject.transform); // Set as a child of the prefab
                    }

                    if (borderPrefab != null)
                    {
                        var border = PrefabUtility.InstantiatePrefab(borderPrefab, BorderParent.transform)
                            as BorderMesh;

                        border.transform.position = worldPosition;
                        border.transform.localRotation = borderInfo.Item2;
                        border.name = $"Border{x}_{y}";
                    }

                    if (cellData.PlacedPrefab != null)
                    {
                        var placeableGridItem = PrefabUtility.InstantiatePrefab(cellData.PlacedPrefab,
                            gridManager.transform) as PlaceableGridItem;

                        placeableGridItem.transform.position = worldPosition;
                        placeableGridItem.GridCoords = coords;
                        containerGrid.SetAvailability(coords, false);
                    }
                }
            }

            // 2) Collect shape IDs -> list of coords
            Dictionary<int, List<Vector2Int>> shapeCoordinates = new Dictionary<int, List<Vector2Int>>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int coords = new Vector2Int(x, y);
                    if (cellGrid.TryGetItem(coords, out CellData cell))
                    {
                        if (cell.ShapeID > 0)
                        {
                            if (!shapeCoordinates.ContainsKey(cell.ShapeID))
                                shapeCoordinates[cell.ShapeID] = new List<Vector2Int>();
                            shapeCoordinates[cell.ShapeID].Add(coords);
                        }
                    }
                }
            }

            // 3) For each shape, spawn 1 block + multiple sub-containers
            foreach (var kvp in shapeCoordinates)
            {
                int shapeID = kvp.Key;
                List<Vector2Int> coordsList = kvp.Value;

                if (_levelVisualizationData.ContainerPrefab == null)
                {
                    Debug.LogWarning("ContainerBlock or Container Prefab missing. Skipping container spawn.");
                    continue;
                }

                // 3a) compute anchor + offsets
                Vector2Int anchor = GetMinXY(coordsList);
                List<Vector2Int> offsets = coordsList.Select(c => c - anchor).ToList();

                var currentCell = cellGrid.GetItem(anchor + offsets[0]);
                var blockInformation = currentCell.BlockRelatedInformation;

                // 3b) instantiate ContainerBlock
                ContainerBlock blockInstance =
                    PrefabUtility.InstantiatePrefab(_containerBlockSet.GetDataByEnum(blockInformation.Type),
                            gridManager.transform)
                        as ContainerBlock;

                blockInstance.name = $"ContainerBlock_ID_{shapeID}";

                // set anchor
                blockInstance.GridCoords = anchor;
                blockInstance.Transform.position = GetWorldPosition(anchor, width, height, _cellSize);

                blockInstance.SetBlockInformation(blockInformation);

                Grid<PlainGridItem> blockGrid = new Grid<PlainGridItem>(GetGridSize(offsets));

                for (int i = 0; i < blockGrid.Width; i++)
                    for (int j = 0; j < blockGrid.Height; j++)
                        blockGrid.SetAvailability(new Vector2Int(i, j), false);

                foreach (var off in offsets)
                    blockGrid.SetAvailability(off, true);

                // 3c) create sub-containers for each offset
                for (int i = 0; i < offsets.Count; i++)
                {
                    Vector2Int offset = offsets[i];
                    Container.Container containerChild = PrefabUtility.InstantiatePrefab(_levelVisualizationData.ContainerPrefab,
                            blockInstance.transform)
                        as Container.Container;
                    containerChild.name = $"Container_{offset.x}_{offset.y}";

                    // position relative to the block
                    var localPos = new Vector3(offset.x * _cellSize, 0f, offset.y * _cellSize);
                    containerChild.transform.localPosition = localPos;

                    // register in the block
                    containerChild.SetContainerInfo(new ContainerInfo(blockInstance, offset: offsets[i]));
                    blockInstance.AddSubContainer(containerChild);

                    // Mark the container occupant in containerGrid => 
                    // so we have an initial arrangement
                    Vector2Int cellCoords = anchor + offset;
                    containerGrid.SetItem(cellCoords, containerChild);

                    currentCell = cellGrid.GetItem(cellCoords);

                    var localBorderInfo = _borderSpawner.GetBorderTypeWithRotation(blockGrid, offset);
                    var borderPrefab = _containerBorderPrefabSet.GetDataByEnum(localBorderInfo.Item1);

                    if (borderPrefab != null)
                    {
                        BorderMesh containerBorder = PrefabUtility.InstantiatePrefab(borderPrefab, blockInstance.transform)
                            as BorderMesh;

                        containerBorder.transform.localPosition = localPos;
                        containerBorder.transform.localRotation = localBorderInfo.Item2;
                        containerBorder.SetBorderColor(blockInformation.Color);
                        blockInstance.AddBorders(containerBorder);

                    }

                    var lidsBorderPrefab = _lidsBorderPrefabSet.GetDataByEnum(localBorderInfo.Item1);

                    if (lidsBorderPrefab != null)
                    {
                        BorderMesh lidsBorder = PrefabUtility.InstantiatePrefab(lidsBorderPrefab, blockInstance.LidsBorderParent)
                            as BorderMesh;

                        lidsBorder.transform.localPosition = localPos;
                        lidsBorder.transform.localRotation = localBorderInfo.Item2;
                        lidsBorder.SetBorderColor(blockInformation.Color);

                    }

                    if (blockInformation.Type == BlockType.Locked && blockInstance.TryGetComponent(out LockedContainer lockedContainer))
                    {
                        var lockedLidsBorderPrefab = _lockedLidsBorderPrefabSet.GetDataByEnum(localBorderInfo.Item1);

                        if (lockedLidsBorderPrefab != null)
                        {
                            BorderMesh lockedLidsBorder = PrefabUtility.InstantiatePrefab(lockedLidsBorderPrefab,
                                    lockedContainer.LockedPartParentTransform)
                                as BorderMesh;

                            lockedLidsBorder.transform.localPosition = localPos;
                            lockedLidsBorder.transform.localRotation = localBorderInfo.Item2;
                            lockedLidsBorder.SetBorderColor(blockInformation.Color);
                        }
                    }

                    for (int j = 0; j < currentCell.ItemStack.Count; j++)
                    {
                        var containerItem = PrefabUtility.InstantiatePrefab(
                            _containerItemSet.GetDataByEnum(currentCell.ItemStack[j]),
                            containerChild.transform) as ContainerItem;

                        containerItem.transform.position = containerChild.GetPartPosition(j, containerItem);
                    }
                }
            }

            gridManager.SetGridInformation(containerGrid, _cellSize);

            var levelName = $"{gridManager.name}_{GUID.Generate()}";
            var textAsset = LevelSerializer.SaveLevel(new LevelData(cellGrid),
                System.IO.Path.Combine(s_relativePath, levelName));

            level.SetLevelAsset(textAsset);

            // Save as prefab
            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(
                levelObject,
                System.IO.Path.Combine(s_savePath, $"{levelName}.prefab")
            );

            PrefabUtility.InstantiatePrefab(savedPrefab);

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            DestroyImmediate(levelObject);
#endif             
        }

        private Vector2Int GetGridSize(List<Vector2Int> offsets)
        {
            int minX = offsets.Min(o => o.x);
            int maxX = offsets.Max(o => o.x);
            int minY = offsets.Min(o => o.y);
            int maxY = offsets.Max(o => o.y);

            int w = (maxX - minX) + 1;
            int h = (maxY - minY) + 1;

            return new Vector2Int(w, h);
        }

        private Vector3 GetWorldPosition(Vector2Int coords, int width, int height, float cellSize)
        {
            float worldX = -(width - 1) * cellSize + coords.x * cellSize;
            float worldZ = coords.y * cellSize;

            worldX += (width - 1) * cellSize / 2f;
            worldZ -= (height - 1) * cellSize / 2f;

            return new Vector3(worldX, 0f, worldZ);
        }

        private Vector2Int GetMinXY(List<Vector2Int> coordsList)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            foreach (var c in coordsList)
            {
                if (c.x < minX) minX = c.x;
                if (c.y < minY) minY = c.y;
            }
            return new Vector2Int(minX, minY);
        }
    }
}