# BlockPuzzleLevelEditor

**BlockPuzzleLevelEditor** is a custom Unity Editor tool designed to facilitate puzzle level creation within a grid layout. It combines several modes—block placement, item placement, prefab placement, and unavailability—to define a puzzle’s shape, interactive pieces, and special blockers. A **procedural generation algorithm** is used to automatically instantiate 3D puzzle geometry based on this layout—ensuring borders, container blocks, and lids are properly generated and saved as a prefab for final use in your game.

![Showcase](https://github.com/user-attachments/assets/45458fb3-892f-4144-8af7-055744ad7b11)

### See the editor in action:
* App Store: https://apps.apple.com/tr/app/block-slide-sort/id6742429510
* Google Play Store: https://play.google.com/store/apps/details?id=com.BugraCetinel.BlockSlideJam

# Special Features
1. **Procedural Generation Algorithm**  
   - Automatically spawns puzzle borders, container blocks, and lids using an adjacency‐based bitmasking technique.  
   - Eliminates the need for manual 3D modeling, ensuring each puzzle is constructed quickly and consistently.

![ProceduralContainers](https://github.com/user-attachments/assets/9a2ce668-63f5-47b0-95f6-718b4d007f9d)
![GridBordersExample](https://github.com/user-attachments/assets/c5147977-f8c5-457d-8979-66acfe67509c)

2. **Multiple Placement Modes**  
   - **Block Placement**, **Item Placement**, **Prefab Placement**, and **Unavailability** modes allow varied control over puzzle data.  
   - Provides straightforward **left‐click to add** and **right‐click to remove** functionality.

3. **Scene Copying and Mirroring**  
   - **Copy Grid From Scene**: Loads an existing layout from any active `GameLevel`.  
   - **Mirror Grid**: Flips the entire grid horizontally and vertically, offering quick variations.

4. **Modular Data Sets for Visual Variation**
   - The `LevelVisualizationManager` references data sets that control which borders, containers, lids, and items get instantiated.
   - By assigning different `ContainerBlockSet`, `BorderPrefabSet`, or `ContainerItemSet` assets, it becomes possible to generate the same puzzle logic with entirely different visuals.
   - This approach allows designers and artists to iterate on puzzle appearances with minimal code changes, simply swapping out references in the Inspector to load alternative 3D models or materials.

**Here you can see the same grid with two different item set:**  

![ItemSetsExample](https://github.com/user-attachments/assets/e9b6ebbe-2b6a-456b-82e5-850b5153cdd4)

# How To
1. **Open the Editor**  
   - In Unity, select **Tools → PuzzleLevelEditor** to access the custom editor window.

2. **Configure Grid Settings**  
   - **Grid Width** and **Grid Height** are set via the provided sliders. The grid is then resized in the editor window to match these dimensions.

3. **Select a Placement Mode**  
   - **Block Placement**, **Item Placement**, **Prefab Placement**, or **Unavailability** can be chosen at the top of the editor. The approach taken when interacting with the grid depends on the selected mode.

4. **Interact with the Grid**  
   - **Left‐click** applies the current settings (e.g., shape ID, item, prefab).  
   - **Right‐click** removes or clears the data (e.g., reset shape ID, remove an item, or clear a prefab).

5. **Use Additional Options**  
   - **Mirror Grid**: Creates a mirrored puzzle arrangement.  
   - **Copy Grid From Scene**: Imports an existing puzzle layout from a `GameLevel` object.  
   - **Reset Grid**: Clears all cells, restoring the editor to a blank state.

6. **Spawn Selected Pieces**  
   - Once the layout is complete, clicking **Spawn Selected Pieces** instructs the **LevelVisualizationManager** to generate the puzzle in 3D.  
   - A final level prefab is then automatically saved in the project.

https://github.com/user-attachments/assets/f4ef34e9-d1af-466e-8798-b9d581139623

# Placement Modes

### a) Block Placement
- **Purpose**: Defines puzzle blocks through shape IDs, color assignments, block types (e.g., regular or locked), and optional movement constraints.  
- **Usage**:
  - **Block Placement** mode is selected in the toolbar.  
  - Shape ID, block color, block type, and movement constraint are configured in the panel.  
  - **Left‐click** to set these properties; **right‐click** clears the cell data.

![BlockPlacement](https://github.com/user-attachments/assets/3da79f75-a961-4617-a52a-c0759c6978b5)

### b) Item Placement
- **Purpose**: Inserts items (up to four per cell) in container cells that have a nonzero `ShapeID`.  
- **Usage**:
  - **Item Placement** mode is selected.  
  - An item color is chosen from the dropdown.  
  - **Left‐click** adds the item color; **right‐click** removes the last placed item in that cell.  
  - A summary in the panel shows which colored items are missing or exceed the required amount.

![ItemPlacementMode](https://github.com/user-attachments/assets/834b7f27-7ba2-4985-82d1-93554960e73d)

### c) Prefab Placement
- **Purpose**: Places special prefab objects (e.g., blockers, locks) onto cells.  
- **Usage**:
  - **Prefab Placement** mode is selected.  
  - Prefabs of type `PlaceableGridItem` can be dragged into the “PlaceablePrefabs” drop area or selected from an existing list.  
  - **Left‐click** applies the chosen prefab to a cell, and **right‐click** removes any existing prefab.

![PrefabPlacementMode](https://github.com/user-attachments/assets/3d3a7f96-7485-44de-b7dc-b39ff17a21e6)

### d) Unavailability Mode
- **Purpose**: Marks cells as holes or unavailable, removing them from the playable region of the grid.  
- **Usage**:
  - **Unavailability** mode is selected.  
  - **Left‐click** toggles the cell’s status.  
  - Unavailable cells appear as an “X” and are not considered in the final puzzle generation.

# Licensing Information
This project utilizes third-party assets, which require separate licenses.





