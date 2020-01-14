/* Author: Ali Sher
 * File Name: Companion.cs
 * Creation Date: 2018-01
 * Modification Date: 2018-01-22
 * Description: Companion characters that are part of the objective
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MiningAwayISU
{
    public class Companion
    {
        //variables to draw the companion
        Rectangle rect;

        //boolean to store whether or not the companion has been saved
        bool isSaved = false;

        //set the position of the companion
        public Companion(Vector2 position)
        {
            //initialize the rectangle
            rect = new Rectangle((int)position.X, (int)position.Y, 30, 30);
        }

        //Pre: none
        //Post: return rectangle
        //Desc: Getter for rectangle
        public Rectangle GetRect()
        {
            return rect;
        }

        //Pre: none
        //Post: return boolean
        //Desc: Getter for isSaved
        public bool GetIsSaved()
        {
            return isSaved;
        }

        //Pre: boolean
        //Post: none
        //Desc: Setter for isSaved
        public void SetIsSaved(bool isSaved)
        {
            this.isSaved = isSaved;
        }
    }
}
