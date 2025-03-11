using UnityEngine;

namespace PuzzleLevelEditor.GridItem
{
    public abstract class PlaceableGridItem : BaseGridItem
    {
        [SerializeField] private Sprite _icon;
    
        public Texture2D IconTexture => _icon.texture;
    }
}