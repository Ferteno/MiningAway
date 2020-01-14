/*
* Author: Phillip
* File Name: Flyer.cs
* Project Name: Mining Away
* Creation Date: Dec, 2017
* Modified Date: Jan, 2018
* Description: Store all information for flyer function
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiningAwayISU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TempISU12;

namespace MiningAwayISU
{
    class Flyer
    {
        //Store Texture
        private Texture2D texture;

        //Store All Tiles in map, and possible collisions
        private List<CollisionTiles> map = new List<CollisionTiles>();
        private List<Rectangle> possibleCollisions = new List<Rectangle>();

        //Store Map width and height
        private int mapWidth;
        private int mapHeight;

        //Store position
        private Vector2 position = new Vector2(300, 100);
        private Rectangle rectangle;

        //Store Velocity
        private Vector2 velocity = new Vector2(0,0);
        private double angle;

        //Store information needed for player tracking
        private Circle viewRadius;
        private Player player;
        private Collision collision = new Collision();

        public Vector2 Position
        {
            get { return position; }
        }

        public Flyer(int mapWidth, int mapHeight, Player player)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.player = player;
            viewRadius = new Circle(50, rectangle.Center.X, rectangle.Center.Y);
        }
        
        public void Load(ContentManager Content)
        {
            //Load Texture
            texture = Content.Load<Texture2D>("Images/Sprites/flyer");

            //Load Rectangle
            rectangle = new Rectangle((int)position.X, (int)position.Y, 6, 12);
        }

        public void Update(GameTime gameTime)
        {
            //Update rectangle based on velocity (Move flyer)
            rectangle.X = rectangle.X + (int)velocity.X;
            rectangle.Y = rectangle.Y + (int)velocity.Y;

            //Update radius
            viewRadius = new Circle(50, rectangle.Center.X, rectangle.Center.Y);


            Input(gameTime);
        }

        //Pre: Enemy exists
        //Post: Enemy moves toward player
        //Desc: Check if the player is within the view radius and move enemy towards player
        private void Input(GameTime gameTime)
        {
            //Check to make sure player exists
            if (player != null)
            {
                //Check if player is in radius
                if (collision.CheckCollision(player.GetRectangle(), viewRadius) && velocity.X < 5 && -5 < velocity.X && velocity.Y < 5 && -5 < velocity.Y)
                {
                    //Set Velocity
                    velocity.X = GetRotation().X * 5;
                    velocity.Y = GetRotation().Y * 5;
                }
            }
        }

        //Pre: 2 Rectangles are inputed
        //Post: Rotation is Outputed
        //Desc: Calculate the rotation between 2 rectangles
        public Vector2 GetRotation()
        {
            //Calculate angle
            angle = -Math.Atan2(player.GetRectangle().Y - rectangle.Y, player.GetRectangle().X - rectangle.X);

            //Turn angle into X and Y lengths
            float x = (float)Math.Cos(angle);
            float y = -(float)Math.Sin(angle);
            return new Vector2(x, y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
           //Draw Enemy
           spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }
}