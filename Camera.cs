//Author: Joshua Ng
//File Name: MiningAwayISU
//Project Name: MiningAwayISU
//Creation Date: Janurary, 2018
//Modified Date: Janurary, 2018
//Description: This class models the camera.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningAwayISU
{
    public class Camera
    {
        // Attributes
        private Matrix transform;
        private Vector2 centre;
        private Viewport viewport;

        // Accessors
        public Matrix Transform
        {
            get { return transform; }
        }

        // Constructor
        public Camera(Viewport newViewport)
        {
            viewport = newViewport;
        }

        /// <summary>
        /// This procedure will update and centre the camera.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        public void Update(Vector2 position, int xOffset, int yOffset)
        {
            // Check to see if player is within the screen
            if (position.X < viewport.Width / 2)
            {
                centre.X = viewport.Width / 2;
            }
            // Move the camera once player is about to leave the screen.
            else if (position.X > xOffset - (viewport.Width / 2))
            {
                centre.X = xOffset - (viewport.Width / 2);
            }
            // Centre the camera respective to the player.
            else
            {
                centre.X = position.X;
            }

            // Check to see if player is within the screen
            if (position.Y < viewport.Height / 2)
            {
                centre.Y = viewport.Height / 2;
            }
            // Move the camera once player is about to leave the screen.
            else if (position.Y > yOffset - (viewport.Height / 2))
            {
                centre.Y = yOffset - (viewport.Height / 2);
            }
            // Centre the camera respective to the player.
            else
            {
                centre.Y = position.Y;
            }
            transform = Matrix.CreateTranslation(new Vector3(-centre.X + (viewport.Width / 2), -centre.Y + (viewport.Height / 2), 0));
        }
    }
}
