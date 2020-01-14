/* Author: Josh Ng
 * File Name: Building.cs
 * Creation Date: 2018-01
 * Modification Date: 2018-01-22
 * Description: Manages the building system for the player
 */
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TempISU12;

namespace MiningAwayISU
{
    class Building
    {
        // Constant variables to store tile type.
        const int GRASS = 0;
        const int IRON = 1;
        const int STONE = 2;
        const int WOOD = 3;

        /// <summary>
        /// This procedure will add a tile.
        /// </summary>
        /// <param name="tile">List of Collision tiles</param>
        /// <param name="inventory">Is an Inventory object</param>
        /// <param name="mouse">MouseState</param>
        public void AddBlock(List<CollisionTiles> tile, Inventory inventory, MouseState mouse)
        {
            // Check to see if player choses a tile.
            if (inventory.RemoveItem() >= 0)
            {
                // Switch through item type.
                switch (inventory.RemoveItem())
                {
                    // Add grass tile to the location of mouse.
                    case 0:
                        tile.Add(new CollisionTiles(GRASS, new Rectangle(mouse.X, mouse.Y, Tiles.TILE_SIZE, Tiles.TILE_SIZE)));
                        break;
                    // Add iron tile to the location of mouse.
                    case 1:
                        tile.Add(new CollisionTiles(inventory.RemoveItem(), new Rectangle(mouse.X, mouse.Y, Tiles.TILE_SIZE, Tiles.TILE_SIZE)));
                        break;
                    // Add stone tile to the location of mouse.
                    case 2:
                        tile.Add(new CollisionTiles(STONE, new Rectangle(mouse.X, mouse.Y, Tiles.TILE_SIZE, Tiles.TILE_SIZE)));
                        break;
                    // Add wood tile to the location of mouse.
                    case 3:
                        tile.Add(new CollisionTiles(WOOD, new Rectangle(mouse.X, mouse.Y, Tiles.TILE_SIZE, Tiles.TILE_SIZE)));
                        break;
                }
            }
        }
    }
}
