//Author: Joshua Ng
//File Name: MiningAwayISU
//Project Name: MiningAwayISU
//Creation Date: Janurary, 2018
//Modified Date: Janurary, 2018
//Description: A class to model the collision tiles.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningAwayISU
{
    public class CollisionTiles : Tiles
    {
        // Attributes.
        public int X;
        public int Y;
        public int damage;
        public int timeToLive;
        public int crackStyle;
        public int type;

        // Overloaded constructor
        public CollisionTiles(int i, Rectangle newRectangle)
        {
            // Render the tiles due to irs respective name.
            texture = Content.Load<Texture2D>("Tile" + i);
            this.Rectangle = newRectangle;
            this.type = i;
        }

        // Default constructor.
        public CollisionTiles()
        {
            this.Clear();
        }

        /// <summary>
        /// This procedure clears the tile.
        /// </summary>
        public void Clear()
        {
            this.X = 0;
            this.Y = 0;
            this.damage = 0;
            this.type = 0;
            this.timeToLive = 0;
            
            // Check to see if rand is null.
            if (HitTile.rand == null)
            {
                HitTile.rand = new Random((int)DateTime.Now.Ticks);
            }
            this.crackStyle = HitTile.rand.Next(4);
        }
    }
}