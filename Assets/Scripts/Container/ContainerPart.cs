using UnityEngine;

namespace PuzzleLevelEditor.Container
{
    public class ContainerPart : MonoBehaviour
    {
        [SerializeField] private Transform _tr;

        public Vector3 Position => _tr.position;
    }
}