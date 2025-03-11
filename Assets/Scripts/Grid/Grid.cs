using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleLevelEditor.GridItem;
using UnityEngine;

namespace PuzzleLevelEditor.Grid
{
    [Serializable]
    public class Grid<T> where T : class, IGridItem
    {
        [SerializeField] private int _width;
        [SerializeField] private int _height;

        [SerializeField] private T[] _grid;
        [SerializeField] private List<Vector2Int> _unavailabilitySet = new();

        public int Width => _width;
        public int Height => _height;
    
        public Grid(int width, int height)
        {
            _width = width;
            _height = height;

            _grid = new T[width * height];
        }

        public Grid(Vector2Int gridSize)
        {
            _width = gridSize.x;
            _height = gridSize.y;

            _grid = new T[_width * _height];
        }

        public bool IsEmpty(Vector2Int coords) => _grid[GetIndex(coords)] == null;
        public bool IsAvailable(Vector2Int coords) => 
            IsValidCoordinates(coords) && !_unavailabilitySet.Contains(coords);

        public void SetAvailability(Vector2Int coordinates, bool isAvailable)
        {
            if (!IsValidCoordinates(coordinates)) return;

            if (isAvailable)
            {
                if (_unavailabilitySet.Contains(coordinates))
                    _unavailabilitySet.Remove(coordinates);
            }
            else
            {
                if (!_unavailabilitySet.Contains(coordinates))
                {
                    _unavailabilitySet.Add(coordinates);
                    SetItem(coordinates, null);
                }
            }
        }
    
        public void SetItem(Vector2Int coordinates, T item) 
        {
            if (!IsValidCoordinates(coordinates)) return;
        
            _grid[GetIndex(coordinates)] = item;

            if (item != null)
                item.GridCoords = coordinates;
        }

        public void RemoveItemFromGrid(Vector2Int coordinates)
        {
            _grid[GetIndex(coordinates)] = null;
        }

        public bool TryGetItem(Vector2Int coords, out T item)
        {
            item = null;

            if (!IsValidCoordinates(coords))
                return false;

            if (!IsAvailable(coords))
                return false;
        
            if (IsEmpty(coords))
                return false;
        
            item = GetItem(coords);
            return true;
        }

        public T GetItem(Vector2Int coords) => _grid[GetIndex(coords)];
    
        public bool TryGetCoordinate(T item, out Vector2Int coordinates)
        {
            coordinates = default;
        
            if (!_grid.Contains(item)) return false;

            coordinates = GetCoordinate(item);
            return true;
        }
    
        public Vector2Int GetCoordinate(T item)
        {
            for (int x = 0; x < _width; x++)    
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector2Int coords = new Vector2Int(x, y);
                
                    if (_grid[GetIndex(coords)].Equals(item))
                        return coords;
                }   
            }

            throw new ArgumentNullException();
        }

    
        public IEnumerable<T> GetRow(int columnIndex)
        {
            if (columnIndex >= _height || columnIndex < 0)
                throw new IndexOutOfRangeException();

            return Enumerable.Range(0, _grid.GetLength(0))
                .Select(x => _grid[GetIndex(new Vector2Int(x, columnIndex))]);
        }
    
        public IEnumerable<T> GetColumn(int rowIndex)
        {
            if (rowIndex >= _width || rowIndex < 0)
                throw new IndexOutOfRangeException();

            return Enumerable.Range(0, _grid.GetLength(1))
                .Select(y => _grid[GetIndex(new Vector2Int(rowIndex, y))]);
        }
    
        public bool IsValidCoordinates(Vector2Int coordinates)
        {
            return coordinates.x >= 0 && coordinates.x < _width && coordinates.y >= 0 && coordinates.y < _height;
        }
    
        private int GetIndex(Vector2Int coordinates)
        {
            return coordinates.y * _width + coordinates.x;
        }
    }
}