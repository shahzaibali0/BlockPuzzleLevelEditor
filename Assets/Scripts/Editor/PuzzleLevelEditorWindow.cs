using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using PuzzleLevelEditor.Data;
using PuzzleLevelEditor.Data.Process;
using PuzzleLevelEditor.Grid;
using PuzzleLevelEditor.GridItem;
using PuzzleLevelEditor.LevelVisualization;

public class PuzzleLevelEditorWindow : EditorWindow
{
    private enum EditorMode
    {
        BlockPlacement,
        PrefabPlacement,
        ItemPlacement,
        Unavailability
    }

    private EditorMode _currentMode = EditorMode.BlockPlacement;

    private Vector2Int _gridSize = new Vector2Int(6, 6);
    private Vector2Int _lastGridSize = new Vector2Int(6, 6);

    private Grid<CellData> _cellGrid;

    // For shape ID logic
    private int _currentShapeID = 1;
    private BlockColor _currentBlockColor = BlockColor.None;
    private BlockType _currentBlockType = BlockType.Regular;
    private MovementConstraint _currentMovementConstraint = MovementConstraint.None;

    // For item placement
    private BlockColor _selectedItemColor = BlockColor.None;

    // Prefab placement
    private readonly List<PlaceableGridItem> _prefabList = new();
    private PlaceableGridItem _selectedPrefab;

    // dynamic cell size
    private float _buttonSize;

    // scroll UI
    private Vector2 _dropAreaScroll;

    [MenuItem("Tools/PuzzleLevelEditor")]
    private static void OpenWindow()
    {
        var w = GetWindow<PuzzleLevelEditorWindow>("PuzzleLevelEditor");
        w.Show();
    }

    private void OnEnable()
    {
        if(_cellGrid == null) ResetGrid();
        LoadPrefabsFromProject();
    }

    private void Awake()
    {
        ResetGrid();
    }

    private void OnGUI()
    {
        EditorGUIUtility.wideMode = true;

        DrawGridSettingsUI();
        DrawModeSelectionUI();

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Spawn Selected Pieces"))
            {
                SpawnSelectedPieces();
            }
            if (GUILayout.Button("Copy Grid From Scene"))
            {
                CopyGridFromScene();
            }
            if (GUILayout.Button("Mirror Grid"))
            {
                MirrorGrid();
            }
            if (GUILayout.Button("Reset Grid"))
            {
                ResetGrid();
            }
        }

        ComputeButtonSize();
        DrawGrid();
    }

    private void ComputeButtonSize()
    {
        float verticalSpace = position.height - 300f; // overhead
        float horizontalSpace = position.width - 20f;

        float cellW = horizontalSpace / _gridSize.x;
        float cellH = verticalSpace / _gridSize.y;

        _buttonSize = Mathf.Min(cellW, cellH, 80f);
    }

    private void DrawGridSettingsUI()
    {
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        int newW = EditorGUILayout.IntSlider("Grid Width", _gridSize.x, 1, 20);
        int newH = EditorGUILayout.IntSlider("Grid Height", _gridSize.y, 1, 20);
        if (EditorGUI.EndChangeCheck())
        {
            // user changed slider => update
            _gridSize = new Vector2Int(newW, newH);
            UpdateGridContent(); 
            _lastGridSize = _gridSize;
        }
    }
    
    private void DrawModeSelectionUI()
    {
        EditorGUILayout.LabelField("Editor Mode:", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(_currentMode == EditorMode.BlockPlacement, "Block Placement", "Button"))
            _currentMode = EditorMode.BlockPlacement;
        if (GUILayout.Toggle(_currentMode == EditorMode.ItemPlacement, "Item Placement", "Button"))
            _currentMode = EditorMode.ItemPlacement;
        if (GUILayout.Toggle(_currentMode == EditorMode.PrefabPlacement, "Prefab Placement", "Button"))
            _currentMode = EditorMode.PrefabPlacement;
        if (GUILayout.Toggle(_currentMode == EditorMode.Unavailability, "Unavailability Mode", "Button"))
            _currentMode = EditorMode.Unavailability;
        EditorGUILayout.EndHorizontal();

        if (_currentMode == EditorMode.BlockPlacement)
        {
            DrawBlockPlacementMode();
        }
        else if (_currentMode == EditorMode.ItemPlacement)
        {
            DrawItemPlacementMode();
        }
        else if (_currentMode == EditorMode.PrefabPlacement)
        {
            DrawPrefabDropArea();
        }
    }

    private void DrawItemPlacementMode()
    {
        GUILayout.Label("Item Placement Mode", EditorStyles.boldLabel);

        // Let the user pick which color item to place
        _selectedItemColor = (BlockColor)EditorGUILayout.EnumPopup("Item Color/Type:", _selectedItemColor);

        // 1) Dictionary to count how many container cells exist for each *container color*
        //    (this tells us how many items we need for that color: cellCount * 4).
        Dictionary<BlockColor, int> neededByContainerColor = new Dictionary<BlockColor,int>();

        // 2) Dictionary to count how many items are placed for each *item color*
        //    (we sum across *all* container cells, ignoring the container's color).
        Dictionary<BlockColor, int> placedByItemColor = new Dictionary<BlockColor,int>();

        // Traverse the entire grid:
        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                Vector2Int coords = new Vector2Int(x, y);

                // We only care about valid container cells (ShapeID>0), skip holes/unavailable
                if (!_cellGrid.TryGetItem(coords, out CellData cellData)) 
                    continue;
                if (cellData.ShapeID <= 0) 
                    continue;

                // A) It's a container cell => increment needed for that container color
                BlockColor containerColor = cellData.BlockRelatedInformation.Color;
                if (!neededByContainerColor.ContainsKey(containerColor))
                    neededByContainerColor[containerColor] = 0;
                neededByContainerColor[containerColor] += 1;  // 1 container cell

                // B) Check each item in the cell's ItemStack
                foreach (var itemColor in cellData.ItemStack)
                {
                    // increment placed count for *that item color*
                    if (!placedByItemColor.ContainsKey(itemColor))
                        placedByItemColor[itemColor] = 0;
                    placedByItemColor[itemColor] += 1;
                }
            }
        }

        // 3) Build a multi-line string showing each *container color's* missing/extra
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // Sort them by color or just iterate:
        foreach (var kvp in neededByContainerColor)
        {
            BlockColor containerColor = kvp.Key;
            if (containerColor == BlockColor.None) continue; // skip “None” color

            int cellCount = kvp.Value;        // how many container cells of this color
            int needed    = cellCount * 4;    // each cell can hold 4
            int placed    = 0;

            // If we didn't place any items of this color at all, it's 0
            if (placedByItemColor.TryGetValue(containerColor, out int placedCount))
                placed = placedCount;

            // difference => negative means missing, positive means extra
            int difference = placed - needed;

            if (difference == 0)
            {
                sb.AppendLine($"{containerColor}: {placed} placed — 0 missing (perfect)");
            }
            else if (difference < 0)
            {
                sb.AppendLine($"{containerColor}: {placed} placed — {-difference} missing");
            }
            else
            {
                sb.AppendLine($"{containerColor}: {placed} placed — {difference} extra");
            }
        }

        // If we had no container colors at all, show a fallback
        if (sb.Length == 0)
            sb.AppendLine("No colored containers found; placing items has no effect right now.");

        EditorGUILayout.HelpBox(sb.ToString(), MessageType.Info);
    }

    
    private void DrawBlockPlacementMode()
    {
        EditorGUILayout.LabelField("Shape ID / Block Color / BlockType", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        _currentShapeID = EditorGUILayout.IntField("Shape ID:", _currentShapeID);
        if (EditorGUI.EndChangeCheck())
            _currentBlockType = BlockType.Regular;
        _currentBlockColor = (BlockColor)EditorGUILayout.EnumPopup("Block Color:", _currentBlockColor);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        _currentBlockType = (BlockType)EditorGUILayout.EnumPopup("Block Type:", _currentBlockType);
        _currentMovementConstraint = (MovementConstraint)EditorGUILayout.EnumPopup("Movement Constraint:", _currentMovementConstraint);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPrefabDropArea()
    {
        EditorGUILayout.LabelField("PlaceablePrefabs", EditorStyles.boldLabel);
        if (_selectedPrefab)
        {
            EditorGUILayout.LabelField("Selected:", _selectedPrefab.name);
        }
        else
        {
            EditorGUILayout.LabelField("No prefab selected.");
        }

        Rect dropRect = GUILayoutUtility.GetRect(0,100, GUILayout.ExpandWidth(true));
        GUI.Box(dropRect, "Drag & Drop Prefabs Here or Click to Select");

        _dropAreaScroll = GUI.BeginScrollView(dropRect, _dropAreaScroll,
            new Rect(0,0, dropRect.width - 20, Mathf.Max(dropRect.height, 90)));

        float xPos=5f, yPos=5f;
        int counter=0;
        foreach(var p in _prefabList)
        {
            if (!p)
                continue;
            
            // FIRST: try big preview
            Texture2D preview = p.IconTexture;

            // fallback: if still null, we can just show text
            GUIContent content = preview ? new GUIContent(preview, p.name) : new GUIContent(p.name);

            if (GUI.Button(new Rect(xPos,yPos,80,80), content))
            {
                _selectedPrefab = p;
                _currentMode = EditorMode.PrefabPlacement;
            }
            counter++;
            if(counter>=6)
            {
                counter=0;
                xPos=5f;
                yPos+=85f;
            }
            else
            {
                xPos+=85f;
            }
        }

        GUI.EndScrollView();
        HandleDragAndDrop(dropRect);
    }


    private void HandleDragAndDrop(Rect rect)
    {
        Event e=Event.current;
        switch(e.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if(!rect.Contains(e.mousePosition)) 
                    return;
                DragAndDrop.visualMode=DragAndDropVisualMode.Copy;
                if(e.type==EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach(var obj in DragAndDrop.objectReferences)
                    {
                        if(obj is GameObject go && go.TryGetComponent<PlaceableGridItem>(out var baseItem))
                        {
                            if(!_prefabList.Contains(baseItem))
                                _prefabList.Add(baseItem);
                        }
                    }
                    e.Use();
                }
                break;
        }
    }

    private void DrawGrid()
    {
        for(int y=_gridSize.y-1; y>=0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for(int x=0; x<_gridSize.x;x++)
            {
                DrawGridCell(new Vector2Int(x,y));
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawGridCell(Vector2Int coords)
    {
        Rect cellRect = GUILayoutUtility.GetRect(_buttonSize, _buttonSize,
            GUILayout.Width(_buttonSize), GUILayout.Height(_buttonSize));

        float margin=2f;
        Rect innerRect = new Rect(
            cellRect.x+margin, 
            cellRect.y+margin, 
            cellRect.width - margin*2, 
            cellRect.height - margin*2
        );

        if(!_cellGrid.TryGetItem(coords, out var cell))
        {
            // hole => black box + "X"
            GUIStyle holeStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
            };
            holeStyle.normal.background = MakeColorTexture(Color.black*0.8f);
            GUI.Box(innerRect, "X", holeStyle);

            DrawOutline(innerRect);

            if(Event.current.type==EventType.MouseDown && innerRect.Contains(Event.current.mousePosition))
            {
                OnCellClicked(coords,null);
                Event.current.Use();
            }
            return;
        }
        
        if (cell.PlacedPrefab != null)
        {
            // try to get a preview or a mini thumbnail
            Texture2D prefabIcon = cell.PlacedPrefab.IconTexture;
            if (prefabIcon == null)
                prefabIcon = AssetPreview.GetMiniThumbnail(cell.PlacedPrefab.gameObject);

            if (prefabIcon != null)
            {
                GUI.DrawTexture(innerRect, prefabIcon);
            }
            else
            {
                // If no icon, optionally draw the name or some label
                GUIStyle smallStyle = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = Color.white },
                    fontSize = 10
                };
                Rect labelRect = new Rect(
                    innerRect.xMax - 60,
                    innerRect.y,
                    60,
                    20);
                GUI.Label(labelRect, cell.PlacedPrefab.name, smallStyle);
            }
            
            if(Event.current.type==EventType.MouseDown && innerRect.Contains(Event.current.mousePosition))
            {
                OnCellClicked(coords,cell);
                Event.current.Use();
            }
            return;
        }

        // background for container color
        Color bg = GetBlockColor(cell.BlockRelatedInformation.Color);
        GUI.DrawTexture(innerRect, MakeColorTexture(bg));
        
        // Up to 4 items => draw small circles
        UnityEditor.Handles.BeginGUI();
        for(int i=0; i<cell.ItemStack.Count && i<4; i++)
        {
            BlockColor c = cell.ItemStack[i];
            Rect subRect = GetSubRect(innerRect, i);
            DrawColorCircle(subRect, GetBlockColor(c));
        }
        UnityEditor.Handles.EndGUI();

        DrawOutline(innerRect);
        
        // If shape ID>0, draw ID
        if(cell.ShapeID>0)
        {
            GUIStyle lbl = new GUIStyle(GUI.skin.label)
            {
                alignment= TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor=Color.white },
            };
            GUI.Label(innerRect, cell.ShapeID + " " + cell.BlockRelatedInformation.GetBlockInfoText(), lbl);
        }

        if(Event.current.type==EventType.MouseDown && innerRect.Contains(Event.current.mousePosition))
        {
            OnCellClicked(coords, cell);
            Event.current.Use();
        }
    }

    private void DrawOutline(Rect r)
    {
        Color lineColor = Color.black;
        // top
        DrawRect(new Rect(r.x, r.y, r.width, 1), lineColor);
        // bottom
        DrawRect(new Rect(r.x, r.yMax-1, r.width, 1), lineColor);
        // left
        DrawRect(new Rect(r.x, r.y, 1, r.height), lineColor);
        // right
        DrawRect(new Rect(r.xMax-1, r.y, 1, r.height), lineColor);
    }

    private void DrawRect(Rect rr, Color c)
    {
        GUI.DrawTexture(rr, MakeColorTexture(c));
    }

    private Rect GetSubRect(Rect parent, int idx)
    {
        float hw = parent.width*0.5f;
        float hh = parent.height*0.5f;
        switch(idx)
        {
            case 0: return new Rect(parent.x, parent.y, hw, hh);
            case 1: return new Rect(parent.x+hw, parent.y, hw, hh);
            case 2: return new Rect(parent.x, parent.y+hh, hw, hh);
            case 3: return new Rect(parent.x+hw, parent.y+hh, hw, hh);
        }
        return parent;
    }

    private void DrawColorCircle(Rect rect, Color circleColor)
    {
        float radius = Mathf.Min(rect.width, rect.height)*0.4f;
        Vector2 center = new Vector2(rect.x + rect.width*0.5f, rect.y + rect.height*0.5f);

        UnityEditor.Handles.color = Color.black;
        UnityEditor.Handles.DrawSolidDisc(center, Vector3.forward, radius);

        float innerRadius = radius - 1f;
        if (innerRadius < 0f) innerRadius = radius;

        UnityEditor.Handles.color = circleColor;
        UnityEditor.Handles.DrawSolidDisc(center, Vector3.forward, innerRadius);
    }
    
    private void OnCellClicked(Vector2Int coords, CellData cell)
    {
        switch(_currentMode)
        {
            case EditorMode.BlockPlacement:
                if(cell != null)
                {
                    if (Event.current.button == 1)
                        cell.ClearCellData();
                    else
                    {
                        if(SetShapeIDWithAdjacency(coords, _currentShapeID))
                        {
                            cell.BlockRelatedInformation = 
                                new(
                                _currentBlockColor,
                                _currentBlockType,
                                _currentMovementConstraint);
                            cell.PlacedPrefab = null;
                            cell.ItemStack.Clear();
                            PropagateClickToOtherCells(_currentShapeID);
                        }
                    }
                }
                break;

            case EditorMode.Unavailability:
            {
                bool toggle = !_cellGrid.IsAvailable(coords);
                _cellGrid.SetAvailability(coords, toggle);
                if(toggle)
                {
                    _cellGrid.SetItem(coords, new CellData());
                }
                else
                {
                    _cellGrid.SetItem(coords, null);
                }
                break;
            }

            case EditorMode.PrefabPlacement:
                if(cell != null)
                {
                    if(Event.current.button == 1)
                    {
                        cell.PlacedPrefab = null;
                    }
                    else if(_selectedPrefab != null)
                    {
                        cell.ClearCellData();
                        cell.PlacedPrefab = _selectedPrefab;
                    }
                }
                break;

            case EditorMode.ItemPlacement:
                if(cell != null)
                {
                    if(cell.ShapeID == 0)
                    {
                        Debug.LogWarning("Cannot place items here—no container (ShapeID=0).");
                        return;
                    }

                    if(Event.current.button == 0)
                    {
                        if(cell.ItemStack.Count < 4 && _selectedItemColor != BlockColor.None)
                        {
                            cell.ItemStack.Add(_selectedItemColor);
                        }
                    }
                    else if(Event.current.button == 1)
                    {
                        if(cell.ItemStack.Count>0)
                            cell.ItemStack.RemoveAt(cell.ItemStack.Count-1);
                    }
                }
                break;
        }
    }

    private bool SetShapeIDWithAdjacency(Vector2Int coords, int newID)
    {
        var cell = _cellGrid.GetItem(coords);
        if(cell.ShapeID == newID) return true;

        if(newID==0)
        {
            cell.ClearCellData();
            return true;
        }
        if(!ShapeIDExists(newID))
        {
            cell.ShapeID=newID;
            return true;
        }
        if(!IsAdjacentToSameID(coords,newID))
        {
            Debug.LogWarning($"Cannot place shape {newID} at {coords}, not adjacent!");
            return false;
        }
        cell.ShapeID=newID;
        return true;
    }

    private bool ShapeIDExists(int shapeID)
    {
        for(int x=0;x<_gridSize.x;x++)
        {
            for(int y=0;y<_gridSize.y;y++)
            {
                if(_cellGrid.TryGetItem(new Vector2Int(x,y), out var c) && c.ShapeID==shapeID)
                    return true;
            }
        }
        return false;
    }

    private bool IsAdjacentToSameID(Vector2Int coords, int checkID)
    {
        Vector2Int[] neighbors = {
            new Vector2Int(coords.x, coords.y+1),
            new Vector2Int(coords.x, coords.y-1),
            new Vector2Int(coords.x-1, coords.y),
            new Vector2Int(coords.x+1, coords.y)
        };
        foreach(var n in neighbors)
        {
            if(_cellGrid.IsValidCoordinates(n))
            {
                if(_cellGrid.TryGetItem(n, out var c) && c.ShapeID==checkID)
                    return true;
            }
        }
        return false;
    }

    private void PropagateClickToOtherCells(int shapeID)
    {
        for (int i = 0; i < _cellGrid.Width; i++)
        {
            for (int j = 0; j < _cellGrid.Height; j++)
            {
                Vector2Int coords = new Vector2Int(i, j);

                if (!_cellGrid.TryGetItem(coords, out var cell)) continue;
                if (cell.ShapeID != shapeID) continue;

                cell.BlockRelatedInformation = new BlockRelatedInformation(
                    _currentBlockColor,
                    _currentBlockType,
                    _currentMovementConstraint);
            }
        }
    }

    private Color GetBlockColor(BlockColor c)
    {
        switch(c)
        {
            case BlockColor.Blue:   return new Color(0.3f,0.5f,1f);
            case BlockColor.Red:    return new Color(0.9f,0.3f,0.3f);
            case BlockColor.Green:  return new Color(0.3f,0.9f,0.3f);
            case BlockColor.Orange: return new Color(1f,0.5f,0f);
            case BlockColor.Yellow: return new Color(0.9f,0.9f,0.2f);
            default:
            case BlockColor.None:   return Color.gray;
        }
    }

    private Texture2D MakeColorTexture(Color c)
    {
        Texture2D t=new Texture2D(1,1);
        t.SetPixel(0,0,c);
        t.Apply();
        return t;
    }

    private void SpawnSelectedPieces()
    {
        LevelVisualizationManager mgr = FindAnyObjectByType<LevelVisualizationManager>();
        
        if(mgr!=null)
        {
            mgr.SpawnSelectedPieces(_cellGrid);
        }
        else
        {
            Debug.LogWarning("No LevelVisualizationManager found in scene!");
        }
    }

    private readonly LevelSerializer _levelSerializer = new();
    
    private void CopyGridFromScene()
    {
        GameLevel level = FindAnyObjectByType<GameLevel>();
        
        if (!level)
        {
            Debug.LogWarning("No Level found in scene to copy from.");
            return;
        }

        var levelData = _levelSerializer.ParseData(level.LevelAsset.text);
        
        _cellGrid = levelData.CellGrid;
        _gridSize = new Vector2Int(_cellGrid.Width, _cellGrid.Height);

        Debug.Log("Copied grid from scene into editor window.");
    }
    
    private void MirrorGrid()
    {
        int w = _gridSize.x;
        int h = _gridSize.y;

        // Build a new grid of same size
        var mirrored = new Grid<CellData>(w, h);

        // For availability copying
        var oldGrid = _cellGrid;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Vector2Int oldPos = new Vector2Int(x, y);

                // newPos is the "rotated/mirrored" position
                int newX = (w - 1 - x);
                int newY = (h - 1 - y);
                Vector2Int newPos = new Vector2Int(newX, newY);

                if (oldGrid.TryGetItem(oldPos, out var oldCell))
                {
                    // Copy the same cell data
                    // (You can do a shallow copy or a brand-new object, your choice).
                    CellData newCell = new CellData
                    {
                        ShapeID      = oldCell.ShapeID,
                        BlockRelatedInformation =  oldCell.BlockRelatedInformation,
                        PlacedPrefab = oldCell.PlacedPrefab,
                        ItemStack    = new List<BlockColor>(oldCell.ItemStack)
                    };

                    mirrored.SetItem(newPos, newCell);
                }

                // Also copy availability from oldGrid
                bool wasAvailable = oldGrid.IsAvailable(oldPos);
                mirrored.SetAvailability(newPos, wasAvailable);
            }
        }

        // Now swap out the old _cellGrid with the mirrored version
        _cellGrid = mirrored;
    }

    private void LoadPrefabsFromProject()
    {
        _prefabList.Clear();
        string[] guids=AssetDatabase.FindAssets("t:Prefab");
        foreach(var g in guids)
        {
            string path=AssetDatabase.GUIDToAssetPath(g);
            var go=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(go && go.TryGetComponent<PlaceableGridItem>(out var gridItem))
                _prefabList.Add(gridItem);
        }
    }

    private void UpdateGridContent()
    {
        var oldGrid = _cellGrid;
        if (oldGrid == null)
        {
            _cellGrid = new Grid<CellData>(_gridSize);
            return;
        }
        
        var newGrid = new Grid<CellData>(_gridSize.x, _gridSize.y);

        int copyWidth = Mathf.Min(_lastGridSize.x, _gridSize.x);
        int copyHeight = Mathf.Min(_lastGridSize.y, _gridSize.y);

        // 1) partial copy
        for (int x = 0; x < copyWidth; x++)
        {
            for (int y = 0; y < copyHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (oldGrid.TryGetItem(pos, out var oldCell)) 
                    newGrid.SetItem(pos, oldCell);
                
                // preserve old availability
                bool oldAvail = oldGrid.IsAvailable(pos);
                newGrid.SetAvailability(pos, oldAvail);
            }
        }

        // 2) fill new areas in ANY dimension:
        for(int x=0; x<_gridSize.x; x++)
        {
            for(int y=0; y<_gridSize.y; y++)
            {
                var coords = new Vector2Int(x,y);

                if (!newGrid.IsAvailable(coords))
                    continue;
                
                if(newGrid.GetItem(coords) == null)
                {
                    newGrid.SetAvailability(coords, true);
                    newGrid.SetItem(coords, new CellData());
                }
            }
        }

        _cellGrid = newGrid;
    }
    
    private void ResetGrid()
    {
        _lastGridSize = _gridSize;
        _cellGrid = new Grid<CellData>(_gridSize);
        for (int x=0; x<_gridSize.x; x++)
        {
            for(int y=0; y<_gridSize.y; y++)
            {
                var cd = new CellData();
                _cellGrid.SetItem(new Vector2Int(x,y), cd);
            }
        }
    }
}
