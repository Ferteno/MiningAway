//Author: Ubaidullah Baryar
//File Name: Inventory.cs
//Project Name: MiningAwayISU
//Creation Date: Jan. 2018
//Modified Date: Jan. 2018
//Description: Inventory class that handels all fuctionality of the inventory, from drawing to searching to 
//crafting to managing the player's inventory. It also holds all inventory related variables such as the player items
//list and the default basic items table
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiningAwayISU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TempISU12
{
    class Inventory
    {
        //Holds images for inventory boxes and entering name box
        private Texture2D invtBoxImg;
        private Texture2D invtStorage;
        private Texture2D enterNameImg;

        //Hold block item images
        private Texture2D grassBlockImage;
        private Texture2D stoneBlockImage;
        private Texture2D woodBlockImage;
        private Texture2D ironBlockImage;

        //Hold miscellaneous item images
        private Texture2D stickImage;

        //Hold sword item images
        private Texture2D woodenSwordImage;
        private Texture2D stoneSwordImage;
        private Texture2D ironSwordImage;

        //Hold pickaxe item images
        private Texture2D woodenPickaxeImage;
        private Texture2D stonePickaxeImage;
        private Texture2D ironPickaxeImage;

        //Hold axe item images
        private Texture2D woodenAxeImage;
        private Texture2D stoneAxeImage;
        private Texture2D ironAxeImage;

        //Holds item type icons
        private Texture2D blocksIcon;
        private Texture2D toolsIcon;
        private Texture2D miscellaneousIcon;

        //Holds the placement of the tile, stats for the player items, searched items, and search texts
        public Vector2 StatsPosPlayer { get; private set; }
        public Vector2 StatsPosSearched { get; private set; }
        public Vector2 ItemNameSearchPlacement { get; private set; }
        private Vector2 titelPos;
      
        //A 2D array of rectangles that hold the position of inventory slots and a full box that covers
        //the region of space where the boxes will be drawn.
        private Rectangle[,] invtStorageBoxes;
        private Rectangle invtStorageFBox;

        //Number of inventory slots for ingame screen NOT inventory and the location of the first slot
        private int numInventoryBoxes;
        private Rectangle barLocation;

        //The string value of what the player is searching and the texts/boxes location
        public string SearchItemName { get; set; }
        public Rectangle SearchItemRec { get; private set; }

        //Rectangles that keep track of the area that each type of item is drawn in the inventory
        private Rectangle blocksSearchRec;
        private Rectangle toolsSearchRec;
        private Rectangle miscellaneousSearchRec;

        //Keeps track of the location and size of item type icons
        private Rectangle blocksIconRec;
        private Rectangle toolsIconRec;
        private Rectangle miscellaneousIconRec;

        //List to hold the ingredients to make an item represented through  vector2s
        private List<Vector2> indexOfIngredients;

        //Holds 2 different sizes of fonts
        private SpriteFont smallFont;
        private SpriteFont mediumFont;

        //Lists to hold the generic items in the game and the items that are held 
        //by the player represented as a vector 2
        public List<Item> ItemsTable { get; private set; }
        public List<Vector2> PlayerItems { get; set; }

        //The equipped item at any given moment
        private int equippedItemNumber;

        //Number of each type of item in the search section
        private int numBlocks;
        private int numTool;
        private int numMiscellaneous;

        //Above variables get replaced/modified so these keep the initial value before the change
        private int accNumBlocks;
        private int accNumTool;
        private int accNumMiscellaneous;

        //Variables to hold the index of the item that was hovered over and clicked in the player inventory
        public int HoveredInventItem { get; private set; }
        private int ClickedInventItem;

        //Variables to hold the index of the item that was hovered over and clicked in the searched items
        public int HoveredSearchItem { get; private set; }
        private int ClickedSearchedItem;

        //Location to move a moving item and a bool to track if it is moving
        private int SpotToMoveItem;
        private bool MovingItem;

        //Keeps track of if an item was picked up
        public bool PickedUpItem;

        //Reference to the collision class which allows one to easily check different types of collision
        private Collision collision;

        //Constructor
        public Inventory(ContentManager content, SpriteFont smallFont, SpriteFont mediumFont, Collision collision)
        {
            //Save the values that were inputed into the construction
            this.smallFont = smallFont;
            this.mediumFont = mediumFont;
            this.collision = collision;
            
            //Initialize all lists and arrays
            ItemsTable = new List<Item>();
            PlayerItems = new List<Vector2>();
            indexOfIngredients = new List<Vector2>();
            invtStorageBoxes = new Rectangle[5, 5];

            //Set the number of slots for items in the game screen and the location of the first one
            numInventoryBoxes = 10;
            barLocation = new Rectangle(60, 10, 50, 50);

            //Set up the locations of the searched text location and the box for it
            SearchItemName = "";     
            SearchItemRec = new Rectangle(280, 10, 300, 70);
            ItemNameSearchPlacement = new Vector2(308, 40);

            //Set the recs for icons
            blocksIconRec = new Rectangle(320, 90, 40, 40);
            toolsIconRec = new Rectangle(410, 90, 40, 40);
            miscellaneousIconRec = new Rectangle(490, 90, 40, 40);

            //Set the recs for the areas in which searched items can be drawn for each type
            blocksSearchRec = new Rectangle(320, 90, 40, 410);
            toolsSearchRec = new Rectangle(410, 90, 40, 410);
            miscellaneousSearchRec = new Rectangle(490, 90, 40, 410);
                                    
            //Set up the positions for the stats and title text
            StatsPosPlayer = new Vector2(10, 280);
            StatsPosSearched = new Vector2(585, 10);
            titelPos = new Vector2(10, 0);

            //Temp variables to set up locations for storange boxes
            int x = 0;
            int y = 30;

            //Loop through the number of rows for the inventory storage
            for (int i = 0; i < invtStorageBoxes.GetLength(0); ++i)
            {
                //Loop through the number of columns for the inventory storage
                for (int j = 0; j < invtStorageBoxes.GetLength(1); ++j)
                {
                    //Set up the location of that box using bath
                    invtStorageBoxes[i, j] = new Rectangle(10 + x, 10 + y, 40, 40);
                    y += 50;
                }

                //Update scale factors for next set of boxes
                y = 30;
                x += 50;
            }

            //Set up a box that surrounds all the inventory slots so collision can be checked with it first to limit 
            //number of checks
            invtStorageFBox = new Rectangle(invtStorageBoxes[0, 0].X,
                                            invtStorageBoxes[0, 0].Y,
                                            invtStorageBoxes[4, 4].Right - invtStorageBoxes[0, 0].X,
                                            invtStorageBoxes[4, 4].Bottom - invtStorageBoxes[0, 0].Y);

      
            //Load all images
            LoadInventoryImages(content);

            //Set up the generic items in the game
            SetupItemTable();

            //Add default player items
            PlayerItems.Add(new Vector2(0, 1));
            PlayerItems.Add(new Vector2(1, 1));
            PlayerItems.Add(new Vector2(2, 1));
            PlayerItems.Add(new Vector2(3, 1));
            PlayerItems.Add(new Vector2(4, 1));
            PlayerItems.Add(new Vector2(5, 1));
        }

        //Pre: N/A
        //Post: Load in all images
        //Desc: Load in all images
        public void LoadInventoryImages(ContentManager content)
        {
            //Load block images
            grassBlockImage = content.Load<Texture2D>("Images/Items/Blocks/grassBlock");
            stoneBlockImage = content.Load<Texture2D>("Images/Items/Blocks/stoneBlock");
            woodBlockImage = content.Load<Texture2D>("Images/Items/Blocks/woodBlock");
            ironBlockImage = content.Load<Texture2D>("Images/Items/Blocks/IronBlock");

            //Load Miscelaneous images
            stickImage = content.Load<Texture2D>("Images/Items/Miscellaneous/Stick");

            //Load Swords images 
            woodenSwordImage = content.Load<Texture2D>("Images/Items/Tools/WoodenSword");
            stoneSwordImage = content.Load<Texture2D>("Images/Items/Tools/StoneSword");
            ironSwordImage = content.Load<Texture2D>("Images/Items/Tools/IronSword");

            //Load Pickaxes images 
            woodenPickaxeImage = content.Load<Texture2D>("Images/Items/Tools/WoodenPickaxe");
            stonePickaxeImage = content.Load<Texture2D>("Images/Items/Tools/StonePickaxe");
            ironPickaxeImage = content.Load<Texture2D>("Images/Items/Tools/IronPickaxe");

            //Load axes images 
            woodenAxeImage = content.Load<Texture2D>("Images/Items/Tools/WoodenAxe");
            stoneAxeImage = content.Load<Texture2D>("Images/Items/Tools/StoneAxe");
            ironAxeImage = content.Load<Texture2D>("Images/Items/Tools/IronAxe");

            //Load item type icon images 
            blocksIcon = content.Load<Texture2D>("Images/Icons/blocksIcon");
            toolsIcon = content.Load<Texture2D>("Images/Icons/toolsIcon");
            miscellaneousIcon = content.Load<Texture2D>("Images/Icons/randomItemsIcon");

            //Load Rectangle images such as the inventory boxes and inputting name rectangle
            enterNameImg = content.Load<Texture2D>("Images/Boxes/nameChoosingBox");
            invtBoxImg = content.Load<Texture2D>("Images/Boxes/inventoryBox");
            invtStorage = content.Load<Texture2D>("Images/Boxes/inventoryStorage");
        }

        //Pre: N/A
        //Post: Set up items table
        //Desc: Set up items table
        public void SetupItemTable()
        {
            //Set up the inital values for block types.
            //They go in order of grass, iron, stone and wood
            ItemsTable.Add(new Block((byte)ItemType.Block,                  //Type of item is is
                                      true,                                 //Is the item stackable
                                      false,                                //Is the item craftable
                                      null,                                 //The item recipe
                                      "Grass Block",                        //The name of the item
                                      grassBlockImage,                      //Image of the item
                                      new Rectangle(0, 0, 20, 20),          //Location of the item
                                      1));                                  //Time in seconds it takes to break the block

            ItemsTable.Add(new Block((byte)ItemType.Block,          
                                      true,                         
                                      false,                        
                                      null,                         
                                      "Iron Block",                 
                                      ironBlockImage,               
                                      new Rectangle(0, 0, 20, 20),  
                                      2));                          

            ItemsTable.Add(new Block((byte)ItemType.Block, 
                                     true, 
                                     false, 
                                     null, 
                                     "Stone Block", 
                                     stoneBlockImage, 
                                     new Rectangle(0, 0, 20, 20), 
                                     2));

            ItemsTable.Add(new Block((byte)ItemType.Block,
                                     true,
                                     false,
                                     null,
                                     "Wood Block",
                                     woodBlockImage,
                                     new Rectangle(0, 0, 20, 20),
                                     2));

            //Set up initial value for miscellaneous items
            //They go in order: Stick
            ItemsTable.Add(new Miscellaneous((byte)ItemType.Miscellaneous,                             //Type of item is is
                                              true,                                                    //Is the item stackable
                                              true,                                                    //Is the item craftable
                                              new List<String>() { "WOOD BLOCK", "WOOD BLOCK" },       //The item recipe
                                              "Stick",                                                 //The name of the item
                                              "Used in making tools.",                                 //The use of the item
                                              stickImage,                                              //Image of the item
                                              new Rectangle(0, 0, 20, 20),                             //Location of the item
                                              2));                                                     //Time in seconds it takes to break the block

            //Set up initial value for sword items
            //They go in order: wood, stone and iron sword
            ItemsTable.Add(new Sword((byte)ItemType.Tool,                                                     //Type of item is is
                                     false,                                                                   //Is the item stackable
                                     true,                                                                    //Is the item craftable
                                     new List<String>() { "STICK", "STICK", "WOOD BLOCK", "WOOD BLOCK" },     //The item recipe
                                     "Wooden Sword",                                                          //The name of the item
                                     100,                                                                     //The durability of the item
                                     60,                                                                      //The damage of the item
                                     30,                                                                      //The speed of attack of the item
                                     woodenSwordImage,                                                        //The image of the item
                                     new Rectangle(0, 0, 20, 20)));                                           //The location of the item
            
            ItemsTable.Add(new Sword((byte)ItemType.Tool, 
                                     false, 
                                     true,
                                     new List<String>() { "STICK", "STICK", "STONE BLOCK", "STONE BLOCK" }, 
                                     "Stone Sword", 
                                     200, 
                                     45, 
                                     40, 
                                     stoneSwordImage, 
                                     new Rectangle(0, 0, 20, 20)));

            ItemsTable.Add(new Sword((byte)ItemType.Tool, 
                                     false, 
                                     true,
                                     new List<String>() { "STICK", "STICK", "IRON BLOCK", "IRON BLOCK" }, 
                                     "Iron Sword", 
                                     400, 
                                     30, 
                                     50, 
                                     ironSwordImage, 
                                     new Rectangle(0, 0, 20, 20)));

            //Set up initial value for pickaxe items
            //They go in order: wood, stone and iron pickaxe
            ItemsTable.Add(new Pickaxe((byte)ItemType.Tool,                                                                  //Type of item is is
                                        false,                                                                               //Is the item stackable
                                        true,                                                                                //Is the item craftable
                                        new List<String>() { "STICK", "STICK", "WOOD BLOCK", "WOOD BLOCK", "WOOD BLOCK" },   //The item recipe
                                        "Wooden Pickaxe",                                                                    //The name of the item
                                        100,                                                                                 //The durability of the item
                                        60,                                                                                  //The speed of the item to break a block in number of frames
                                        woodenPickaxeImage,                                                                  //The image of the item
                                        new Rectangle(0, 0, 20, 20)));                                                       //The location of the item

            ItemsTable.Add(new Pickaxe((byte)ItemType.Tool, 
                                        false, 
                                        true,
                                        new List<String>() { "STICK", "STICK", "STONE BLOCK", "STONE BLOCK", "STONE BLOCK" }, 
                                        "Stone Pickaxe", 
                                        200, 
                                        55, 
                                        stonePickaxeImage, 
                                        new Rectangle(0, 0, 20, 20)));

            ItemsTable.Add(new Pickaxe((byte)ItemType.Tool, 
                                       false, 
                                       true,
                                       new List<String>() { "STICK", "STICK", "IRON BLOCK", "IRON BLOCK", "IRON BLOCK" }, 
                                       "Iron Pickaxe", 
                                       400, 
                                       50, 
                                       ironPickaxeImage, 
                                       new Rectangle(0, 0, 20, 20)));

            //Set up initial value for axe items
            //They go in order: wood, stone and iron axe
            ItemsTable.Add(new Axe((byte)ItemType.Tool,                                                                     //Type of item is is
                                   false,                                                                                   //Is the item stackable
                                   true,                                                                                    //Is the item craftable
                                   new List<String>() { "STICK", "STICK", "WOOD BLOCK", "WOOD BLOCK", "WOOD BLOCK" },       //The item recipe
                                   "Wooden Axe",                                                                            //The name of the item
                                   100,                                                                                     //The durability of the item
                                   60,                                                                                      //The speed of the item to break a block in number of frames
                                   woodenAxeImage,                                                                          //The image of the item
                                   new Rectangle(0, 0, 20, 20)));                                                           //The location of the item

            ItemsTable.Add(new Axe((byte)ItemType.Tool, 
                                   false, 
                                   true,
                                   new List<String>() { "STICK", "STICK", "STONE BLOCK", "STONE BLOCK", "STONE BLOCK" }, 
                                   "Stone Axe",
                                   400, 
                                   55, 
                                   stoneAxeImage, 
                                   new Rectangle(0, 0, 20, 20)));

            ItemsTable.Add(new Axe((byte)ItemType.Tool, 
                                   false, 
                                   true,
                                   new List<String>() { "STICK", "STICK", "IRON BLOCK", "IRON BLOCK", "IRON BLOCK" }, 
                                   "Iron Axe", 
                                   400, 
                                   50, 
                                   ironAxeImage, 
                                   new Rectangle(0, 0, 20, 20)));
        }

        #region Scrolling Item Bar Logic/Drawing

        //Pre: scrolled amount is scroll amount at the current time and prevScrolledAmount is the scroll amount past updata
        //Post: Changes the equipped item index if the mouse is scrolled
        //Desc: If the mouse is scrolled forward or backward, the equipped item is adjusted accordingly
        public void ScrollEffectItemsBar(int scrolledAmount, int prevScrolledAmount)
        {
            //Is the scroll amount greater than or less than the previous amount
            if (scrolledAmount > prevScrolledAmount)
            {
                //If the item hasn't reached the max item number of 10, add one to its count and return
                if (equippedItemNumber != 9)
                {
                    ++equippedItemNumber;
                    return;
                }

                //Set the item number back to the start so it loops around
                equippedItemNumber = 0;
            }
            else if (scrolledAmount < prevScrolledAmount)
            {
                //If the item hasn't reached the minimum item number of 1, minus one to its count and return
                if (equippedItemNumber != 0)
                {
                    --equippedItemNumber;
                    return;
                }

                //Set the item number back to the end so it loops around
                equippedItemNumber = 9;
            }
        }

        //Pre: spriteBatch allows for images and text to be drawn
        //Post: The inventory bar in in game is draw and the equipped item box is bigger
        //Desc:
        public void DrawCurrentItemsBar(SpriteBatch spriteBatch)
        {
            //Temporary variables that hold the increase in size and the color change
            int increaseSize = 0;
            Color color = Color.White;

            //Loop through each bar box
            for (int i = 0; i < numInventoryBoxes; ++i)
            {
                //Is the equipped item the same as the item index being cheed rn
                if (equippedItemNumber == i)
                {
                    //Inrease size and change color
                    increaseSize = 8;
                    color = Color.Gold;
                }
                else
                {
                    //Set the incrase of size and color to default
                    increaseSize = 0;
                    color = Color.White;
                }

                //Draw the item box
                spriteBatch.Draw(invtBoxImg,
                                 new Rectangle((int)(barLocation.X * i) - (increaseSize / 2),
                                               (int)(barLocation.Y) - (increaseSize / 2),
                                               barLocation.Width + increaseSize,
                                               barLocation.Height + increaseSize),
                                 color);

                //Draw the first x amount of items that fit into the bar slots
                if (i < PlayerItems.Count())
                {
                    spriteBatch.Draw(ItemsTable[(int)PlayerItems[i].X].Image,
                                     new Rectangle((int)(barLocation.X * i) - (increaseSize / 2) + 6,
                                     (int)(barLocation.Y) - (increaseSize / 2) + 6,
                                     barLocation.Width + increaseSize - 12,
                                     barLocation.Height + increaseSize - 12),
                                     Color.White);
                }


            }
        }
        #endregion

        #region PlayerInventory Logic/Drawing

        //Pre: spriteBaatch allows for the drawing of images and texts
        //Post: Draws the boxes for inventory storage
        //Desc: Loops through a 2d away and draws the boxes for inventory storage
        public void DrawInventoryStorage(SpriteBatch spriteBatch)
        {
            //Draw the inventory title
            spriteBatch.DrawString(mediumFont, "INVENTORY", titelPos, Color.White);

            //Loop through the number of rows
            for (int i = 0; i < invtStorageBoxes.GetLength(0); ++i)
            {
                //Loop through the number of columns
                for (int j = 0; j < invtStorageBoxes.GetLength(1); ++j)
                {
                    //Draw the inventory box at that location
                    spriteBatch.Draw(invtBoxImg, invtStorageBoxes[i, j], Color.White);
                }
            }

            //Draw the inventory images
            DrawInventoryItems(spriteBatch);
        }

        //Pre: spriteBaatch allows for the drawing of images and texts
        //Post: Draws all the player owned items
        //Desc: Loops through the number of items the player has and draws them inorder in the inventory slots
        public void DrawInventoryItems(SpriteBatch spriteBatch)
        {
            //Temp variables to change a 2d array position into a 1d away position
            int row = 0;
            int col = 0;

            //Loop through the number of items
            for (int i = 0; i < PlayerItems.Count(); ++i)
            {
                //Calculate the row and column
                col = (int)(i / invtStorageBoxes.GetLength(0));

                row = i % invtStorageBoxes.GetLength(1);

                //Draw the item image
                spriteBatch.Draw(ItemsTable[(int)PlayerItems[i].X].Image, 
                                 new Rectangle(invtStorageBoxes[row, col].X + 5,
                                               invtStorageBoxes[row, col].Y + 5,
                                               invtStorageBoxes[row, col].Width - 10,
                                               invtStorageBoxes[row, col].Height - 10), 
                                 Color.White);

                //If the item is stackable, draw the number of items that are in the stack
                if (ItemsTable[(int)PlayerItems[i].X].Stackable)
                {
                    spriteBatch.DrawString(smallFont,
                                           Convert.ToString(PlayerItems[i].Y),
                                           new Vector2(invtStorageBoxes[row, col].X + invtStorageBoxes[row, col].Width - 14,
                                                       invtStorageBoxes[row, col].Y + invtStorageBoxes[row, col].Height - 14),
                                           Color.White);
                }
            }
        }

        //Pre: spriteBaatch allows for the drawing of images and texts. Position is where the stats will be draw
        //iList is the items list and choosenItem is the item whos stats should be displayed
        //Post: Draw the stats of player items
        //Desc: Check if the choosen item is a valid one and if it is, draw it's stats
        public void DrawInventoryStats(SpriteBatch spriteBatch, Vector2 position, List<Vector2> iList, int choosenItem)
        {
            //Is the choosenItem a valid item. Draw it's stats if it is
            if (choosenItem < iList.Count() && choosenItem != -1)
            {
                ItemsTable[(int)iList[choosenItem].X].DrawStats(spriteBatch, smallFont, position);
            }   
        }

        //Pre: spriteBaatch allows for the drawing of images and texts. Position is where the stats will be draw
        //iList is the items list and choosenItem is the item whos stats should be displayed
        //Post: Draw the stats of the searched items
        //Desc: Check if the choosen item is a valid one and if it is, draw it's stats
        public void DrawInventoryStats(SpriteBatch spriteBatch, Vector2 position, List<Item> iList, int choosenItem)
        {
            //Is the choosenItem a valid item. Draw it's stats if it is
            if (choosenItem < iList.Count() && choosenItem != -1)
            {
                iList[choosenItem].DrawStats(spriteBatch, smallFont, position);
            }
        }

        //Pre: mouse holds a reference to the mouse, including location and clicks
        //Post: return whether the mouse is ontop of an item. If it is, return the index that corresponds to that position and -1 if
        //no item exists in that spot
        //Desc: Check if the mouse collides with the inventory boxes. Then loops through each box and find which one it collided with
        //and return that value
        public int FindPlayerItem(MouseState mouse)
        {
            //Check if the mouse is in the area of the inventory boxes
            if (collision.CheckCollision(mouse, invtStorageFBox))
            {
                //Loop through the rows of the boxes
                for (int i = 0; i < invtStorageBoxes.GetLength(0); ++i)
                {
                    //Loop through the columns of the boxes
                    for (int j = 0; j < invtStorageBoxes.GetLength(1); ++j)
                    {
                        //Check if the mouse collided with that box specifically and return the index that corresponds with that value if it did
                        if (collision.CheckCollision(mouse, invtStorageBoxes[i, j]))
                        {
                            return (j * invtStorageBoxes.GetLength(0)) + i;
                        }
                    }                 
                }

                //didn't collide with item
                return -1;
            }

            //didn't collide with item
            return -1;
        }

        //Pre: N/A
        //Post: Switch spots of the two items being moved 
        //Desc: Check if the spot to move the item is valid and if it is, swap the location of the 2 items
        public void MoveItem()
        {
            //Is the spot to move the item valid
            if (SpotToMoveItem < PlayerItems.Count() && SpotToMoveItem != -1)
            {
                //Swap places of the items
                Vector2 tempItem = PlayerItems[SpotToMoveItem];
                PlayerItems[SpotToMoveItem] = PlayerItems[ClickedInventItem];
                PlayerItems[ClickedInventItem] = tempItem;
            }

            //Set moving item to false
            MovingItem = false;
        }

        //Pre: mouse holds a reference to the mouse, including location and clicks
        //Post: Determines whether to display the stats of the item or move the item
        //Desc: If the mouse if clicked, an item is potentially being moved, find the index of the item being moved if it is
        //If it is not being moved, determine the index of the item to dispaly its stats
        public void StatsOrMove(MouseState mouse)
        {
            //Was the mouse clicked
            if (collision.CheckMouseClick(mouse))
            {
                //If an item is not being moved, check if and item can be moved
                if (!MovingItem)
                {
                    ClickedInventItem = FindPlayerItem(mouse);
                }

                //If the item is valid, set moving to true
                if (ClickedInventItem != -1)
                {
                    MovingItem = true;
                }
                else
                {
                    MovingItem = false;
                }
            }
            else
            {
                //Find the index of the item being hovered over
                HoveredInventItem = FindPlayerItem(mouse);

                //If the item is moving find the spot to move the item and drop it there
                if (MovingItem)
                {
                    SpotToMoveItem = FindPlayerItem(mouse);
                    MoveItem();
                }
            }
        }

        #endregion

        #region Searching Item Logic/Drawing

        //Pre: spriteBatch allows for images and text to be drawn
        //Post: Draws everything associated with drawing
        //Desc: Drawas the name entering box and the searched items and icons
        public void DrawSearch(SpriteBatch spriteBatch)
        {
            //Draw the name entering box, items and icons
            spriteBatch.Draw(enterNameImg, SearchItemRec, Color.White);
            DrawSearchedItemsAndIcons(spriteBatch);        
        }

        //Pre: N/A
        //Post: If the item was searched, depending on it's type, update its location
        //Desc: If an item was searched for, update its location depending on it's type and update the number of 
        //items of that type that are being searched
        public void SearchForItem()
        {
            //Loop through all items in item table
            for (int i = 0; i < ItemsTable.Count(); ++i)
            {
                //Swtich statement that checks the type of each item. So the types are block,
                //miscellaneous, and tool
                switch ((ItemType)ItemsTable[i].OverallType)
                {
                    case ItemType.Block:
                        //If the current item contains the string values that were inputted, update the number of blocks and
                        //the location of the item depending on the number of blocks
                        if (ItemsTable[i].SearchedFor = ItemsTable[i].Name.Contains(SearchItemName))
                        {
                            numBlocks++;
                            ItemsTable[i].Rec = new Rectangle(blocksIconRec.X + 5,
                                                              blocksIconRec.Y + ((blocksIconRec.Height + 5) * numBlocks),
                                                              blocksIconRec.Width - 10,
                                                              blocksIconRec.Height - 10);
                        }

                        break;

                    case ItemType.Tool:
                        //If the current item contains the string values that were inputted, update the number of tools and
                        //the location of the item depending on the number of tools
                        if (ItemsTable[i].SearchedFor = ItemsTable[i].Name.Contains(SearchItemName))
                        {
                            ++numTool;
                            ItemsTable[i].Rec = new Rectangle(toolsIconRec.X + 5,
                                                              toolsIconRec.Y + ((toolsIconRec.Height + 5) * numTool),
                                                              toolsIconRec.Width - 10,
                                                              toolsIconRec.Height - 10);
                        }
                        break;

                    case ItemType.Miscellaneous:
                        //If the current item contains the string values that were inputted, update the number of missellaneous and
                        //the location of the item depending on the number of missellaneous
                        if (ItemsTable[i].SearchedFor = ItemsTable[i].Name.Contains(SearchItemName))
                        {
                            ++numMiscellaneous;
                            ItemsTable[i].Rec = new Rectangle(miscellaneousIconRec.X + 5,
                                                              miscellaneousIconRec.Y + ((miscellaneousIconRec.Height + 5) * numMiscellaneous),
                                                              miscellaneousIconRec.Width - 10,
                                                              miscellaneousIconRec.Height - 10);
                        }

                        break;       
                }
            }

            //Save the number of each type of item so they can be resetted instead of being decremented
            accNumBlocks = numBlocks;
            accNumTool = numTool;
            accNumMiscellaneous = numMiscellaneous;

            //Set all the counts back to 0
            numTool = numBlocks = numMiscellaneous = 0;
        }

        //Pre: spriteBatch allows for images and text to be drawn
        //Post: Draw the searched items and icons
        //Desc: Draw the searched items and icons
        public void DrawSearchedItemsAndIcons(SpriteBatch spriteBatch)
        {
            //Loops through all the items in item table
            for (int i = 0; i < ItemsTable.Count(); ++i)
            {
                //If that item was searched for
                if (ItemsTable[i].SearchedFor)
                {
                    //Draw the box to put the iamge in
                    spriteBatch.Draw(invtBoxImg, 
                                     new Rectangle(ItemsTable[i].Rec.X -5, 
                                                   ItemsTable[i].Rec.Y - 5,
                                                   ItemsTable[i].Rec.Width + 10,
                                                   ItemsTable[i].Rec.Height + 10), 
                                     Color.Yellow);

                    //Draw the image
                    spriteBatch.Draw(ItemsTable[i].Image, ItemsTable[i].Rec, Color.White);
                }
            }

            //Draw the icons
            spriteBatch.Draw(blocksIcon, blocksIconRec, Color.White);
            spriteBatch.Draw(toolsIcon, toolsIconRec, Color.White);
            spriteBatch.Draw(miscellaneousIcon, miscellaneousIconRec, Color.White);
        }

        //Pre: The current and previous mousestates
        //Post: Checks whether the mouse is in one of item types so that there is a chance an item is being hovered over
        //Desc: If the mouse is colliding with item type column, check if its over that type of item
        public void SearchedItemColCollision(MouseState mouse, MouseState prevMouse)
        {
            //Is the mouse colliding with each type of item column
            if (collision.CheckCollision(mouse, blocksSearchRec))
            {
                //CHeck if the mouse is over a block
                SearchedItemIndexStats(mouse, prevMouse, ItemType.Block);
            }
            else if (collision.CheckCollision(mouse, toolsSearchRec))
            {
                //CHeck if the mouse is over a tool
                SearchedItemIndexStats(mouse, prevMouse, ItemType.Tool);
            }
            else if (collision.CheckCollision(mouse, miscellaneousSearchRec))
            {
                //CHeck if the mouse is over a Miscellaneous item
                SearchedItemIndexStats(mouse, prevMouse, ItemType.Miscellaneous);
            }
            else
            {
                //Set the item indexs to invalid ones
                HoveredSearchItem = -1;
                ClickedSearchedItem = -1;
            }
        }

        //Pre: The current and previous mousestates and the type of item being checked for
        //Post: Sets the index of hovered items and clicked item
        //Desc: Check if the item was hovered on and/or clicked and save the index value if it was
        public void SearchedItemIndexStats(MouseState mouse, MouseState prevMouse, ItemType type)
        {
            //Loop through all items in the items tale
            for (int i = 0; i < ItemsTable.Count(); ++i)
            {
                //Check if that item was searched for and it is of the same type
                if (ItemsTable[i].SearchedFor &&
                   (ItemType)ItemsTable[i].OverallType == type)
                {
                    //Check if the mouse if hovering over it
                    if (collision.CheckCollision(mouse, ItemsTable[i].Rec))
                    {
                        //Check if the item was clicked and save the index if it was
                        if (collision.CheckMouseClick(mouse, prevMouse))
                        {
                            ClickedSearchedItem = i;
                        }

                        //Save the index of the item that was hovered over
                        HoveredSearchItem = i;
                        return;
                    }
                }
            }

            //Set the index values to invalid ones
            HoveredSearchItem = -1;
            ClickedSearchedItem = -1;
        }

        #endregion

        #region Crafting

        //Pre: N/A
        //Post: Add an item if the ingredients are met and the players inventory is not full
        //Desc: Check if the indegredients match what the player has and add an item if it does and removed the items
        //that are being used from the player inventory
        public void CraftNewItem()
        {
            //Check if the player has enough for the recipe
            if (CheckRecipe())
            {
                //Loop through each ingredient
                for (int i = 0; i < indexOfIngredients.Count(); ++i)
                {
                    //Remove the number of items that correspond with the item being used up
                    PlayerItems[(int)indexOfIngredients[i].X] = new Vector2(PlayerItems[(int)indexOfIngredients[i].X].X, PlayerItems[(int)indexOfIngredients[i].X].Y - indexOfIngredients[i].Y);

                    //If the item is completly used up, remove it from the player items lsit
                    if (PlayerItems[(int)indexOfIngredients[i].X].Y == 0)
                    {
                        PlayerItems.RemoveAt((int)indexOfIngredients[i].X); 
                    }
                }

                //If the inventory is not full, add the item, set picked up to true and clear values
                if (PlayerItems.Count != 25)
                {
                    PlayerItems.Add(new Vector2(ClickedSearchedItem, 1));
                    PickedUpItem = true;
                    ClickedSearchedItem = -1;
                    indexOfIngredients.Clear();
                }
                else
                {
                    //Set the serached item to invalid and clear the ingredients list
                    ClickedSearchedItem = -1;
                    indexOfIngredients.Clear();
                }
            }
            else
            {
                //Set the serached item to invalid and clear the ingredients list
                ClickedSearchedItem = -1;
                indexOfIngredients.Clear();
            }
        }

        //Pre: N/A
        //Post: Return true if the recipe can be used and false if it can't
        //Desc: Check if the item is valid and that the player has space in the inventory and then check if the player
        //has enough ingredients to make that choosen item
        public bool CheckRecipe()
        {
            //If the clicked item was valid and the inventory actually has space 
            if (ClickedSearchedItem < ItemsTable.Count() && ClickedSearchedItem != -1 &&
                PlayerItems.Count != 25)
            {
                //Set the default number for an ingredient to 1
                int numOfIngredients = 1;

                //Check if the item actually has a recipe
                if (ItemsTable[ClickedSearchedItem].Recipe != null)
                {
                    //Loops through through the recipe
                    for (int i = 0; i < ItemsTable[ClickedSearchedItem].Recipe.Count(); ++i)
                    {
                        //Check if there is more than 1 of that ingredient and add 1 to the number of that ingredient if there is
                        if (i + 1 != ItemsTable[ClickedSearchedItem].Recipe.Count() &&
                            ItemsTable[ClickedSearchedItem].Recipe[i] == ItemsTable[ClickedSearchedItem].Recipe[i + 1])
                        {
                            numOfIngredients++;
                        }
                        else
                        {
                            //If the player list of items doesnt have that ingredient, return false
                            if (!CheckIngredients(ItemsTable[ClickedSearchedItem].Recipe[i], numOfIngredients))
                            {
                                return false;
                            }

                            //Reset the number of that item
                            numOfIngredients = 1;
                        }
                    }
                    //The recipe was complete
                    return true;
                }              
            }

            //Recipe can't be made
            return false;
        }

        //Pre: ingredient is a string that holds all the items needed to make an item and the number of that ingredient needed
        //Post: return a true if the player has the items and false if they doesnt
        //Desc: Loops through all the player items and check if that ingredient exists. And then it checks if enough of that ingredient exists
        public bool CheckIngredients(string ingredient, int numIngredient)
        {
            //Loops through the player items
            for (int j = 0; j < PlayerItems.Count(); ++j)
            {
                //Check if the item matches the ingredients
                if (ItemsTable[(int)PlayerItems[j].X].Name == ingredient)
                {
                    //Check if there is enough of that item
                    if (PlayerItems[j].Y >= numIngredient)
                    {
                        //Add the ingredient to an ingredient list
                        indexOfIngredients.Add(new Vector2(j, numIngredient));
                        return true;
                    }

                    //Not enough of that ingredient
                    return false;
                }
            }

            //THe item doesn't exist
            return false;
        }

        //Pre: N/A
        //Post: If the added item matches a type within the player items and is stackable, add to the count
        //Desc:If a new item is picked up or made, check if another item exists that matches that type in the player
        //items list and add to its counts
        public void CheckStackAndPickUp()
        {
            //Was an item picked up and is that item stackable
            if (PickedUpItem &&
                ItemsTable[(int)PlayerItems[PlayerItems.Count() - 1].X].Stackable)
            {
                //Loop through all th player items
                for (int i = 0; i < PlayerItems.Count() - 1; ++i)
                {
                    //Check if the item matches one that was picke dup
                    if (ItemsTable[(int)PlayerItems[i].X].Name ==
                        ItemsTable[(int)PlayerItems[PlayerItems.Count() - 1].X].Name)
                    {      
                        //Add to the count of that item 
                        PlayerItems[i] = new Vector2(PlayerItems[i].X,
                                                     PlayerItems[i].Y +
                                                     PlayerItems[PlayerItems.Count() - 1].Y);

                        //Remove the item that was just added and set picke dup to false
                        PlayerItems.RemoveAt(PlayerItems.Count() - 1);
                        PickedUpItem = false;
                        break;
                    }
                }
            }
        }

        #endregion

        //Pre: N/A
        //Post: Return the index of the item that was removed and -1 if you cannot remove that type of item
        //Desc: Check if the type of item is a block and if there remove 1 of that item from the player list
        public int RemoveItem()
        {
            //Was the type of item a block
            if (equippedItemNumber < PlayerItems.Count())
            {
                if (ItemsTable[(int)PlayerItems[equippedItemNumber].X].OverallType == (byte)ItemType.Block)
                {
                    //Was there more than 1 item
                    if (PlayerItems[equippedItemNumber].Y != 1)
                    {
                        //Remove just 1 from the count of that item and return the type of that item
                        PlayerItems[equippedItemNumber] = new Vector2(PlayerItems[equippedItemNumber].X,
                                                                      PlayerItems[equippedItemNumber].Y - 1);

                        return (int)PlayerItems[equippedItemNumber].X;
                    }
                    else
                    {
                        //Remove the item from the player list and return the index of it
                        int returnIndex = (int)PlayerItems[equippedItemNumber].Y;
                        PlayerItems.RemoveAt(equippedItemNumber);
                        return returnIndex;
                    }
                }

            }
            //item wasn't a block
            return -1;
        }


    }
}
