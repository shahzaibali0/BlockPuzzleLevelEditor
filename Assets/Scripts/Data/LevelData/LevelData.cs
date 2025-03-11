using System;
using PuzzleLevelEditor.Data.Process;
using PuzzleLevelEditor.Grid;

namespace PuzzleLevelEditor.Data
{
    [Serializable]
    public class LevelData : IParsable
    {
        public Grid<CellData> CellGrid;

        public LevelData(Grid<CellData> cellGrid)
        {
            CellGrid = cellGrid;
        }
    }
}