using PuzzleLevelEditor.Container;
using UnityEngine;

[CreateAssetMenu(menuName = "Create LevelVisualizationData", fileName = "LevelVisualizationData")]
public class LevelVisualizationData : ScriptableObject
{
    [SerializeField] private GameObject _groundItem;
    [SerializeField] private Container _containerPrefab;
    
    public GameObject GroundItem => _groundItem;
    public Container ContainerPrefab => _containerPrefab;
}