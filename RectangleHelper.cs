//Author: Joshua Ng, Phillip Kalmanson
//File Name: MiningAwayISU
//Project Name: MiningAwayISU
//Creation Date: Janurary, 2018
//Modified Date: Janurary, 2018
//Description: A helper class for the rectangle.
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningAwayISU
{
    static class RectangleHelper
    {
        /// <summary>
        /// Check to see if player touches the top.
        /// </summary>
        /// <param name="r1">Is a rectangle</param>
        /// <param name="r2">Is a rectangle</param>
        /// <returns>bool</returns>
        public static bool IsTouchTop(this Rectangle r1, Rectangle r2)
        {
            return (r1.Bottom >= r2.Top - 1 &&
                r1.Bottom <= r2.Top + (r2.Height / 2) &&
                r1.Right > r2.Left &&
                r1.Left < r2.Right);
        }

        /// <summary>
        /// Check to see if player touches the bottom.
        /// </summary>
        /// <param name="r1">Is a rectangle</param>
        /// <param name="r2">Is a rectangle</param>
        /// <returns>bool</returns>
        public static bool IsTouchBottom(this Rectangle r1, Rectangle r2)
        {
            return (r1.Top <= r2.Bottom + (r2.Height / 5) &&
                    r1.Top >= r2.Bottom - 1 &&
                    r1.Right > r2.Left &&
                    r1.Left < r2.Right);
        }

        /// <summary>
        /// Check to see if player touches the left of rectangle.
        /// </summary>
        //// <param name="r1">Is a rectangle</param>
        /// <param name="r2">Is a rectangle</param>
        /// <returns>bool</returns>
        public static bool IsTouchLeft(this Rectangle r1, Rectangle r2)
        {
            return (r1.Right > r2.Left &&
                r1.Left < r2.Left &&
                r1.Top <= r2.Bottom &&
                r1.Bottom >= r2.Top);
        }

        /// <summary>
        /// Check to see if player touches the right of the rectangle.
        /// </summary>
        /// <param name="r1">Is a rectangle</param>
        /// <param name="r2">Is a rectangle</param>
        /// <returns>bool</returns>
        public static bool IsTouchRight(this Rectangle r1, Rectangle r2)
        {
            return (r1.Left < r2.Right &&
                r1.Right > r2.Right &&
                r1.Top <= r2.Bottom &&
                r1.Bottom >= r2.Top);
        }
    }
}
