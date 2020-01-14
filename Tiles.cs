//Author: Joshua Ng
//File Name: MiningAwayISU
//Project Name: MiningAwayISU
//Creation Date: Janurary, 2018
//Modified Date: Janurary, 2018
//Description: This class models the tile system.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningAwayISU
{
    public class Tiles
    {
        // Constant attributes for the tile class.
        public const int TILE_SIZE = 20;
        public const int AIR_BLOCK = 0;
        public const int GRASS_BLOCK = 1;
        public const int DIRT_BLOCK = 2;
        public const int WOOD_BLOCK = 3;
        public const int STONE_BLOCK = 4;
        public const int LOG_BLOCK = 5;
        public const int IRON_BLOCK = 6;

        // Attributes
        protected Texture2D texture;
        private Rectangle rectangle;
        private static ContentManager content;

        // Getter and setter for rectangle.
        public Rectangle Rectangle
        {
            get { return rectangle; }
            protected set { rectangle = value; }
        }

        // Getter and setter for content.
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        // Draw the tiles
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }
}
