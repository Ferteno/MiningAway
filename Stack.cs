/* Author: Ubiadullah
 * File Name: Stack.cs
 * Creation Date: 2018-01
 * Modification Date: 2018-01-22
 * Description: Data structure
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningAwayISU
{
    class Stack<T>
    {
        ///TO USE GENERIC STACK: SPECIFY TYPE WITHING <>
        ///EX: private STACK<Vector2> random
        //private List<Vector2> locations;
        private List<T> type;
        private int count;


        public Stack()
        {
            type = new List<T>();
            count = 0;
        }

        //Pre: None
        //Post: Returns the size of the List
        //Desc: Returns the size of the stack
        public int Size()
        {
            return count;
        }

        //Pre: None
        //Post: Whether the stack is empty
        //Desc: Returns whether the stack is empty
        public bool IsEmpty()
        {
            return (count == 0);
        }

        //Pre: The new location to add
        //Post: None
        //Desc: Adds a new thing to the stack
        public void Push(T newType)
        {
            type.Add(newType);
            count++;
        }

        //Pre: None
        //Post: The thing on top of the stack
        //Desc: Shows the first thing and remove it
        public T Pop()
        {
            T first = default(T);

            //If there is at least one type
            if (!IsEmpty())
            {
                first = type[count - 1];
                type.RemoveAt(count - 1);
                count--;
            }

            return first;
        }

        //Pre: None
        //Post: The thing on top of the stack
        //Desc: Show what is on the top of the stack
        public T Top()
        {
            return type[count - 1];
        }

        //Pre: None
        //Post: The last thing of the stack
        //Desc: Show the thing that is the last of the stack
        public T Last()
        {
            return type[0];
        }
    }
}
