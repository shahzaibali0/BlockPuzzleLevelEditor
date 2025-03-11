using System;
using System.Collections.Generic;
using PuzzleLevelEditor.GridItem;
using UnityEngine;

namespace PuzzleLevelEditor.Data
{
    [Serializable]
    public class CellData : IGridItem
    {
        public int ShapeID;
        public BlockRelatedInformation BlockRelatedInformation;
        public PlaceableGridItem PlacedPrefab; // if you have a "BaseGridItem" script or similar
        public List<BlockColor> ItemStack = new ();
    
        public Vector2Int GridCoords { get; set; }

        public CellData()
        {
            ClearCellData();
        }

        public void ClearCellData()
        {
            ShapeID = 0;
            BlockRelatedInformation = new BlockRelatedInformation();
            PlacedPrefab = null;
            ItemStack.Clear();
        }
    }
}

[Serializable]
public class BlockRelatedInformation
{
    public BlockColor Color;
    public BlockType Type;
    public MovementConstraint MovementConstraint;

    public BlockRelatedInformation(BlockColor color, BlockType type, MovementConstraint movementConstraint)
    {
        Color = color;
        Type = type;
        MovementConstraint = movementConstraint;
    }

    public BlockRelatedInformation()
    {
        Color = BlockColor.None;
        Type = BlockType.Regular;
        MovementConstraint = MovementConstraint.None;
    }

    public string GetBlockInfoText()
    {
        string text = "";

        // If the block type is not Regular, add type info on a new line.
        if (Type != BlockType.Regular)
        {
            string typeStr = Type.ToString();
            int length = Mathf.Min(typeStr.Length, 3);
            text += "\n" + typeStr.Substring(0, length);
        }
    
        // If the movement constraint is not None, add movement info on a new line.
        if (MovementConstraint != MovementConstraint.None)
        {
            string moveStr = MovementConstraint.ToString();
            int length = Mathf.Min(moveStr.Length, 3);
            text += "\n" + moveStr.Substring(0, length);
        }
    
        return text;
    }

}