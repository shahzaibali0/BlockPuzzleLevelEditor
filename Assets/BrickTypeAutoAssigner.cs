using UnityEngine;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

public class BrickTypeAutoAssigner : MonoBehaviour
{
    [System.Serializable]
    public class MaterialBrickTypePair
    {
        public Material material;
        public BrickType brickType;
    }

    [Tooltip("Drag materials here and assign their corresponding BrickTypes")]
    public List<MaterialBrickTypePair> materialTypeMappings = new List<MaterialBrickTypePair>();

#if ODIN_INSPECTOR
    [Button("Auto Assign Brick Types", ButtonSizes.Large)]
#endif
    public void AssignBrickTypesBasedOnMaterial()
    {
        if (materialTypeMappings.Count == 0)
        {
            Debug.LogWarning("No material mappings defined!");
            return;
        }

        int assignedCount = 0;
        BuildingSinglePiece[] allPieces = GameObject.FindObjectsOfType<BuildingSinglePiece>(true);

        foreach (BuildingSinglePiece piece in allPieces)
        {
            Renderer renderer = piece.GetComponent<Renderer>();
            if (renderer == null) continue;

            bool typeAssigned = false;
            foreach (Material mat in renderer.sharedMaterials)
            {
                if (mat == null) continue;

                foreach (var mapping in materialTypeMappings)
                {
                    if (mapping.material == mat)
                    {
                        piece.BrickType = mapping.brickType;
                        assignedCount++;
                        typeAssigned = true;
                        break;
                    }
                }
                if (typeAssigned) break;
            }

            if (!typeAssigned)
            {
                Debug.LogWarning($"No BrickType mapping found for materials on {piece.name}");
            }
        }

        Debug.Log($"Assigned BrickTypes to {assignedCount} objects");
    }

#if ODIN_INSPECTOR
    [Button("Find All Materials In Scene", ButtonSizes.Medium)]
#endif
    public void FindAllMaterialsUsed()
    {
        materialTypeMappings.Clear();
        HashSet<Material> uniqueMaterials = new HashSet<Material>();

        BuildingSinglePiece[] allPieces = GameObject.FindObjectsOfType<BuildingSinglePiece>(true);
        foreach (BuildingSinglePiece piece in allPieces)
        {
            Renderer renderer = piece.GetComponent<Renderer>();
            if (renderer == null) continue;

            foreach (Material mat in renderer.sharedMaterials)
            {
                if (mat != null)
                {
                    uniqueMaterials.Add(mat);
                }
            }
        }

        // Create empty mappings
        //foreach (Material mat in uniqueMaterials)
        //{
        //    materialTypeMappings.Add(new MaterialBrickTypePair()
        //    {
        //        material = mat,
        //        brickType = BrickType.None // You'll need to set these manually
        //    });
        //}

        Debug.Log($"Found {uniqueMaterials.Count} unique materials. Please assign BrickTypes to them.");
    }
}