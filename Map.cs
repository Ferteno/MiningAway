//Author: Joshua Ng
//File Name: MiningAwayISU
//Project Name: MiningAwayISU
//Creation Date: Janurary, 2018
//Modified Date: Janurary, 2018
//Description: This class models the map.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TempISU12;

namespace MiningAwayISU
{
    class Map
    {
        // Attributes.
        private int width;
        private int height;
        private Inventory inventory;
        private Collision collision;
        Building build = new Building();
        HitTile hitTile;

        // A list of collision tiles.
        public List<CollisionTiles> collisionTilesList = new List<CollisionTiles>();

        // Constructor
        public Map(ContentManager content, SpriteFont smallFont, SpriteFont mediumFont, Collision collision)
        {
            this.collision = collision;
            inventory = new Inventory(content, smallFont, mediumFont, this.collision);
        }

        // Accessors
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public Inventory GetInventory()
        {
            return inventory;
        }

        /// <summary>
        /// This function generates the map.
        /// </summary>
        /// <param name="map">A 2D array of int</param>
        /// <param name="size">size of each tile</param>
        /// <returns>List<CollisionTiles></returns>
        public List<CollisionTiles> Generate(int[,] map, int size)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int number = map[y, x];

                    // Keep adding tiles to the list of collision tiles until end of 2D array.
                    if (number > 0)
                    {
                        collisionTilesList.Add(new CollisionTiles(number, new Rectangle(x * size, y * size, size, size)));
                    }
                    width = (x + 1) * size;
                }
            }
            return collisionTilesList;
        }

        // Draw the map.
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (CollisionTiles tile in collisionTilesList)
            {
                tile.Draw(spriteBatch);
            }
        }

        // Update.
        public void Update(SpriteBatch spriteBatch, MouseState mouse)
        {
            build.AddBlock(collisionTilesList, inventory,mouse);
        }

        public void Temp(GameTime gameTime, MouseState mouse)
        {
            build.AddBlock(collisionTilesList, inventory, mouse);
        }
    }
}
