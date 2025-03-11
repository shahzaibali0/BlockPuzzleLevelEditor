using UnityEngine;

namespace PuzzleLevelEditor.Container.Block
{
    public class LockedContainer : MonoBehaviour
    {
        [SerializeField] private ContainerBlock _containerBlock;
        [SerializeField] private GameObject _lockedPartParent;
    
        public Transform LockedPartParentTransform => _lockedPartParent.transform;
    }
}