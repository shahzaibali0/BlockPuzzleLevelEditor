using UnityEngine;

namespace PuzzleLevelEditor.Container
{
    public class ContainerMesh : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        public void SetMeshMaterial(Material mat)
        {
            _renderer.sharedMaterial = mat;
        }

    }
}