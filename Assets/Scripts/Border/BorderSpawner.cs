using PuzzleLevelEditor.Grid;
using PuzzleLevelEditor.GridItem;
using UnityEngine;

namespace PuzzleLevelEditor.BorderLogic
{
    public class BorderSpawner
    {
        public (BorderType, Quaternion) GetBorderTypeWithRotation<T>(Grid<T> grid, Vector2Int coords) where T : class, IGridItem
        {
            // Compute bitmask => each neighbor that is "hole/out-of-bounds" => set bit
            int code = 0;
            if (!grid.IsAvailable(new Vector2Int(coords.x,     coords.y + 1))) code |= 1;  // up
            if (!grid.IsAvailable(new Vector2Int(coords.x + 1, coords.y    ))) code |= 2;  // right
            if (!grid.IsAvailable(new Vector2Int(coords.x,     coords.y - 1))) code |= 4;  // down
            if (!grid.IsAvailable(new Vector2Int(coords.x - 1, coords.y    ))) code |= 8;  // left
        
            if (code == 0)
                return (BorderType.None, Quaternion.identity);
        
            var (prefab, rotation) = GetBorderPiece(coords.x, coords.y, code);
            return (prefab, Quaternion.Euler(0, rotation, 0));
        }
    
        private (BorderType, float) GetBorderPiece(int x, int y, int code)
        {
            // code in [1..15]
            switch (code)
            {
                // Single side => End piece
                case 1:   return (BorderType.End, 0f);   // up
                case 2:   return (BorderType.End, 90f);  // right
                case 4:   return (BorderType.End, 180f); // down
                case 8:   return (BorderType.End, 270f); // left

                // Straight lines => up/down=5(0101), left/right=10(1010)
                case 5:   return (BorderType.Straight, 90f);   // vertical
                case 10:  return (BorderType.Straight, 0f);  // horizontal

                // Corners => up+right=3, right+down=6, down+left=12, left+up=9
                // Actually the codes for 2 adjacent bits can be 3,6,9,12 
                case 3:   return (BorderType.Corner, 0f);   // up(1)+right(2)
                case 6:   return (BorderType.Corner, 90f);  // right(2)+down(4)
                case 12:  return (BorderType.Corner, 180f); // down(4)+left(8)
                case 9:   return (BorderType.Corner, 270f); // left(8)+up(1)

                // T => 7=0111, 11=1011, 13=1101,14=1110
                case 7:   return (BorderType.U, 0f);   // up,right,down
                case 11:  return (BorderType.U, 270f);  // up,right,left
                case 13:  return (BorderType.U, 180f); // up,down,left
                case 14:  return (BorderType.U, 90f); // right,down,left

                // Cross => 15 => all sides open
                case 15:  return (BorderType.Single, 0f);

                default:
                    return (BorderType.None, 0f);
            }
        }
    }
}