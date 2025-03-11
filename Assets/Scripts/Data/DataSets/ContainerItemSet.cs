using PuzzleLevelEditor.Container.Item;
using UnityEngine;

namespace PuzzleLevelEditor.Data
{
    [CreateAssetMenu(menuName = "ContainerItemSet", fileName = "ContainerItemSet")]
    public class ContainerItemSet : BaseDataSet<BlockColor, ContainerItem>
    {
        
    }
}