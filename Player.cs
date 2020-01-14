/*
* Author: Phillip
* File Name: Player.cs
* Project Name: Mining Away
* Creation Date: Dec, 2017
* Modified Date: Jan, 2018
* Description: Store all information for player function
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
    class Player
    {
        //Store player details
        private string name;
        private string gender;

        //Store player textures
        private Texture2D textureM;
        private Texture2D textureF;

        //Store possible collisions and all tiles
        List<CollisionTiles> map = new List<CollisionTiles>();
        List<Rectangle> possibleCollisions = new List<Rectangle>();

        //Store map height and width
        private int mapWidth;
        private int mapHeight;

        //Store position
        private Vector2 position = new Vector2(300, 3000);
        private Rectangle rectangle;

        //Store movement information
        private Vector2 velocity;
        private bool hasJumped = false;
        
        //Store Animation details
        private Animation walkingAni;
        public bool animationChosen = false;
        private bool flip = false;

        public Vector2 Position
        {
            get { return position; }
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public Rectangle GetRectangle()
        {
            return rectangle;
        }

        public string GetGender()
        {
            return gender;
        }

        public void SetGender(string gender)
        {
            this.gender = gender;
        }

        public Player(int mapWidth, int mapHeight, List<CollisionTiles> map)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.name = "";
            this.gender = "";
            this.map = map;
            }
        
        public void Load(ContentManager Content)
        {
            //Load Textures
            textureF = Content.Load<Texture2D>("Images/Sprites/PlayerS/Female_Hero");
            textureM = Content.Load<Texture2D>("Images/Sprites/PlayerS/Male_Hero");

            //Load Rectangle
            rectangle = new Rectangle((int)position.X, (int)position.Y, 20, 40);
        }

        public void Update(GameTime gameTime)
        {
            //Reset possible collisions
            possibleCollisions.Clear();
            foreach (CollisionTiles box in map)
            {
                //Check if tile is in radius
                if (rectangle.X - 100 < box.Rectangle.X && rectangle.X + 100 > box.Rectangle.X && rectangle.Y - 100 < box.Rectangle.Y && rectangle.Y + 100 > box.Rectangle.Y && box.type != 5)
                {
                    //Add tile to possible collisions
                    possibleCollisions.Add(box.Rectangle);
                }
            }

            //Animate player
            if (walkingAni != null)
            {
                walkingAni.Update(gameTime);
                walkingAni.destRec = rectangle;
            }

            //Update player position (move player)
            rectangle.X = rectangle.X + (int)velocity.X;
            rectangle.Y = rectangle.Y + (int)velocity.Y;
            Input(gameTime);
        }
        //Pre: Key is clicked
        //Post: Velocity is Changed
        //Desc: Check what key is clicked and update velocity
        private void Input(GameTime gameTime)
        {
            //Set Animation based on gender
            if (!animationChosen)
            {
                if (gender == "female")
                {
                    walkingAni = new Animation(textureF, 1, 12, 12, 0, 0, -1, 10, rectangle, true, 33, 56);
                    animationChosen = true;
                    walkingAni.destRec = rectangle;
                }
                else if (gender == "male")
                {
                    walkingAni = new Animation(textureM, 1, 13, 13, 0, 0, -1, 10, rectangle, true, 33, 56);
                    animationChosen = true;
                    walkingAni.destRec = rectangle;
                }
            }

            //Check if player is not against a wall and D is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.D) && CheckWallCollision(rectangle, possibleCollisions) != 1)
            {
                //Move player
                rectangle.X += 1;
                flip = true;
            }
            //Check if player is not against a wall and A is pressed
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && CheckWallCollision(rectangle, possibleCollisions) != 2)
            {
                //Move player
                rectangle.X += -1;
                flip = false;
            }
            else
            {
                //Stop player
                velocity.X = 0f;
            }

            //Check if player can jump, and presses space
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && hasJumped == false)
            {
                //Shoot player up
                rectangle.Y -= 10;
                //Set Velocity
                velocity.Y = -5f;
                //Stop player from jumping in air
                hasJumped = true;
            }

            //Check if player is falling
            if (hasJumped)
            {
                //Move player down
                float i = 1;
                velocity.Y += 0.15f * i;
            }

            //Check if player is on a block
            if (CheckFloorCollision(rectangle, possibleCollisions) == 1)
            {
                //Stop player
                velocity.Y = 0;
                //Allow player to jump
                hasJumped = false;
            }
            else
            {
                //Stop player from jumping in air
                hasJumped = true;
            }

            //Check if player is colliding with wall
            if (CheckWallCollision(rectangle, possibleCollisions) > 0)
            {
                //Stop player
                velocity.X = 0;
            }

            //Check if player is colliding with ceiling
            if (CheckTopCollision(rectangle, possibleCollisions) == 1)
            {
                //Stop player
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
            if (animationChosen && walkingAni != null)
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
