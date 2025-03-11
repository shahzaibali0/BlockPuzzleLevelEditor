using UnityEngine;

namespace PuzzleLevelEditor.BorderLogic
{
    public class BorderMesh : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private BorderMaterialSet _borderMaterialSet;

        public void SetBorderColor(BlockColor color)
        {
            var materialInfo = _borderMaterialSet.GetDataByEnum(color);
        
            _renderer.sharedMaterials = materialInfo.Materials;
        }
    }
}