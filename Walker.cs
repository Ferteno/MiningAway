/*
* Author: Phillip
* File Name: Walker.cs
* Project Name: Mining Away
* Creation Date: Dec, 2017
* Modified Date: Jan, 2018
* Description: Store all information for walker function
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
    class Walker
    {
        //Store texture
        private Texture2D texture;

        //Store tiles that enemy can collide with
        List<CollisionTiles> map = new List<CollisionTiles>();
        List<Rectangle> possibleCollisions = new List<Rectangle>();

        //Store map width and height
        private int mapWidth;
        private int mapHeight;

        //Store postion of enemy
        private Vector2 position = new Vector2(300, 200);
        private Rectangle rectangle;

        //Store movement information
        private Vector2 velocity;
        private bool hasJumped = false;

        //Store animation information
        private Animation walkingAni;
        private bool flip = false;
        private Collision collision = new Collision();
        private Circle viewRadius;
        private Player player;

        public Vector2 Position
        {
            get { return position; }
        }
        public Rectangle GetRectangle()
        {
            return rectangle;
        }

        public Walker(int mapWidth, int mapHeight, List<CollisionTiles> map, Player player)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.map = map;
            this.player = player;
        }

        public void Load(ContentManager Content)
        {
            //Load texture
            texture = Content.Load<Texture2D>("Images/Sprites/PlayerS/Female_Hero");

            //Load rectangle
            rectangle = new Rectangle((int)position.X, (int)position.Y, 20, 40);

            //Load Animation
            walkingAni = new Animation(texture, 4, 1, 4, 1, 0, -1, 10, rectangle, true, 32, 41);
        }

        public void Update(GameTime gameTime)
        {
            //Reset possible collisions
            possibleCollisions.Clear();
            foreach (CollisionTiles box in map)
            {
                //Add tiles in 100 pixel radius
                if (rectangle.X - 100 < box.Rectangle.X && rectangle.X + 100 > box.Rectangle.X && rectangle.Y - 100 < box.Rectangle.Y && rectangle.Y + 100 > box.Rectangle.Y && box.type != 5)
                {
                    possibleCollisions.Add(box.Rectangle);
                }
            }

            //Animate
            if (walkingAni != null)
            {
                walkingAni.Update(gameTime);
            }
            
            //Update Rectangle (move enemy)
            rectangle.X = rectangle.X + (int)velocity.X;
            rectangle.Y = rectangle.Y + (int)velocity.Y;
           
            Input(gameTime);
        }

        //Pre: Enemy exists
        //Post: Enemy moves toward player
        //Desc: Check if the player is within the view radius and move enemy towards player
        private void Input(GameTime gameTime)
        {
            //Reset animation rectangle
            walkingAni.destRec = rectangle;

            ///Reset radius
            viewRadius = new Circle(100, rectangle.X, rectangle.Y);

            //Check if player is in radius
            if (player != null && collision.CheckCollision(player.GetRectangle(), viewRadius) && velocity.X < 5 && -5 < velocity.X && velocity.Y < 5 && -5 < velocity.Y)
            {
                //Move enemy base on position
                if (player.GetRectangle().X > rectangle.X && CheckWallCollision(rectangle, possibleCollisions) != 1)
                {
                    rectangle.X += 1;
                    flip = true;
                }
                else if (player.GetRectangle().X < rectangle.X && CheckWallCollision(rectangle, possibleCollisions) != 2)
                {
                    rectangle.X += -1;
                    flip = false;
                }
                    //Jump if enemy is stuck
                else if (hasJumped == false)
                {
                    rectangle.Y -= 10;
                    velocity.Y = -3f;
                    hasJumped = true;
                }
            }
            else
            {
                velocity.X = 0f;
            } 

            //Move enemy down if falling
            if (hasJumped)
            {
                float i = 1;
                velocity.Y += 0.15f * i;
            }

            //Check if enemy collides with floor
            if (CheckFloorCollision(rectangle, possibleCollisions) == 1)
            {
                velocity.Y = 0;
                hasJumped = false;
            }
            else
            {
                hasJumped = true;
            }

            //Check if enemey collides with walls
            if (CheckWallCollision(rectangle, possibleCollisions) > 0)
            {
                velocity.X = 0;
            }

            //Check if enemy collides with ceiling
            if (CheckTopCollision(rectangle, possibleCollisions) == 1)
            {
                velocity.Y = 0;
            }
        }

        //Pre: 2 Rectangles are inserted
        //Post: int is outputed to deterimine which wall is collided with
        //Desc: Checks collisions between rectangles and output an int
        public int CheckWallCollision(Rectangle box1, List<Rectangle> boxList)
        {
            foreach (Rectangle tile in boxList)
            {
                if (tile != null)
                {
                    //Check collision between player and rightside of tile
                    if (box1.IsTouchRight(tile))
                    {
                        return 2;
                    }
                    //Check collision between player and leftside of tile
                    if (box1.IsTouchLeft(tile))
                    {
                        return 1;
                    }
                }
            }
            //None of the above so return 0
            return 0;
        }

        //Pre: 2 Rectangles are inserted
        //Post: int is outputed
        //Desc: Checks collisions between rectangles and output an int
        public int CheckFloorCollision(Rectangle box1, List<Rectangle> boxList)
        {
            foreach (Rectangle tile in boxList)
            {
                if (tile != null)
                {
                    //Check if player is colliding with floor
                    if (box1.IsTouchTop(tile))
                    {
                        //Set player box bottom to floor so player doesnt get stucl
                        rectangle.Y = tile.Y - rectangle.Height - 1;
                        return 1;
                    }
                }
            }
            //return 0 if no collision is detected
            return 0;
        }

        //Pre: 2 Rectangles are inserted
        //Post: int is outputed
        //Desc: Checks collisions between rectangles and output an int
        public int CheckTopCollision(Rectangle box1, List<Rectangle> boxList)
        {
            foreach (Rectangle tile in boxList)
            {
                if (tile != null)
                {
                    //Check if player is colliding with bottom of tile
                    if (box1.IsTouchBottom(tile))
                    {
                        return 1;
                    }
                }
            }
            //Return 0 if false
            return 0;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            //Check if animation is chosen
            if (walkingAni != null)
            {
                //Flip animation based on charater movement
                if (!flip)
                {
                    walkingAni.Draw(spriteBatch, Color.White, SpriteEffects.None);
                }
                else
                {
                    walkingAni.Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
                }
            }
        }

        //Pre: NA
        //Post: Update map
        //Desc: Clear map and reset map
        public void UpdateMap(List<CollisionTiles> newMap)
        {
            map.Clear();
            foreach (CollisionTiles tile in newMap)
            {
                this.map.Add(tile);
            }
        }
    }
}
