//Author: Joshua Ng
//File Name: MiningAwayISU
//Project Name: MiningAwayISU
//Creation Date: Janurary, 2018
//Modified Date: Janurary, 2018
//Description: This class models the destruction of each tile.
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
    public class HitTile
    {
        // Constant Attributes
        public static int lastCrack = -1;
        private const int UNUSED = 0;
        private const int TILE = 1;
        private const int STONE = 2;
        private const int MAX_HITS = 20;
        private const int TIMETOLIVE = 60;

        // Attributes
        public static Random rand;
        public CollisionTiles[] data;
        private int[] order;
        private int location;
        TerrainBuilder tB;

        // Constructor.
        public HitTile()
        {
            data = new CollisionTiles[21];
            // Initialize each tile to have a total of 21 hits.
            order = new int[21];

            // Add collision tiles to the data array and an int value respecivley.
            for (int i = 0; i <= 20; ++i)
            {
                this.data[i] = new CollisionTiles();
                this.order[i] = i;
            }
            // Set location to be 0.
            this.location = 0;
        }

        /// <summary>
        /// This procedure will clear the tile.
        /// </summary>
        /// <param name="tileType">Is an int</param>
        public void Clear(int tileType)
        {
            // Check to see if tile is elidgible to clear
            if (tileType < 0 || tileType > 20)
            {
                return;
            }
            // Clear tile from array.
            this.data[tileType].Clear();
        }

        /// <summary>
        /// This function will add damage to current block being attacked   
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="damageAmount">An int to store the amount of damage per hit</param>
        /// <param name="updateAmount">A bool to see if there can be more damage done</param>
        /// <returns>The damage done to tile</returns>
        public int AddDamage(int tileType, int damageAmount, bool updateAmount = true)
        {
            // Check to see if tile can be damaged
            if (tileType < 0 || tileType > 20 || tileType == this.location && damageAmount == 0)
            {
                return 0;
            }
            CollisionTiles hitTileObject = this.data[tileType];

            // Add more damage if there can be more damage done.
            if (!updateAmount)
            {
                return hitTileObject.damage + damageAmount;
            }

            // Add damage and initiate the tile's time to remain live.
            hitTileObject.damage += damageAmount;
            hitTileObject.timeToLive = 60;

            // Check to see if the tile type is at the location.
            if (tileType == this.location)
            {
                // Set location to last index of order.
                location = order[20];
                data[location].Clear();

                // Allow player to keep hitting tile.
                for (int i = 20; i > 0; --i)
                {
                    // Remove durability from tile.
                    order[i] = order[i - 1];
                }
                // Set location to the first index of order.
                this.order[0] = this.location;
            }
            else
            {
                // Initialize index to be 0.
                int index = 0;
                while (index <= 20 && this.order[index] != tileType)
                {
                    // Incrament index by 1 each loop.
                    ++index;

                    // Loop when until index is less than 1.
                    for (; index > 1; --index)
                    {
                        int num = this.order[index - 1];
                        order[index - 1] = order[index];
                        order[index] = num;
                    }
                    // Set tileType to the order.
                    this.order[1] = tileType;
                }
            }
            return hitTileObject.damage;
        }

        /// <summary>
        /// This function will return the location of the tile that was hit.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="hitType"></param>
        /// <returns>location</returns>
        public int HitObject(int x, int y, int hitType)
        {
            for (int index1 = 0; index1 <= 20; ++index1)
            {
                int index2 = this.order[index1];

                // Store the index where tile was hit
                CollisionTiles hitTileObject = this.data[index2];

                // Check to see if the hit type can hit the block.
                if (hitTileObject.type == hitType)
                {
                    // Check to see if the location matches.
                    if (hitTileObject.X == x && hitTileObject.Y == y)
                    {
                        return index2;
                    }
                }
                // Check if a tool is equipped
                else if (index1 != 0 && hitTileObject.type == 0)
                {
                    break;
                }
            }
            // Store the index where tile was hit
            CollisionTiles hitTileObject1 = data[location];

            // Store X and Y location.
            hitTileObject1.X = x;
            hitTileObject1.Y = y;

            // Store the type of hit done.
            hitTileObject1.type = hitType;

            return this.location;
        }

    }
}
