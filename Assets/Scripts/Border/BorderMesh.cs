using System.Linq;
using UnityEngine;

namespace PuzzleLevelEditor.BorderLogic
{
    public class BorderMesh : MonoBehaviour
    {


        [SerializeField] private Renderer _renderer;
        [SerializeField] private BorderMaterialSet _borderMaterialSet;
        CellRaycast cellRaycast;
        public void SetBorderColor(BlockColor color)
        {
            var materialInfo = _borderMaterialSet.GetDataByEnum(color);

            _renderer.sharedMaterials = materialInfo.Materials;
        }

        public void SetBorderMaterial(Material material)
        {
            Material[] materials = _renderer.sharedMaterials; // Get a copy
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }
            _renderer.sharedMaterials = materials; // Assign back
        }



        private void Start()
        {
            cellRaycast = transform.GetComponent<CellRaycast>();

        }


        public Transform RayData(RaycastDirections raycastDirections)
        {
            cellRaycast = transform.GetComponent<CellRaycast>();

            return cellRaycast.GetRayData(raycastDirections);
        }
    }
}