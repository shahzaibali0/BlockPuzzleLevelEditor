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

        private void Start()
        {
            cellRaycast = transform.GetComponent<CellRaycast>();

        }


        public Transform RayData(RaycastDirections raycastDirections)
        {
            return cellRaycast.GetRayData(raycastDirections);
        }
    }
}