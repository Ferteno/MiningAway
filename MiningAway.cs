//Author: Ali Sher, Phillip Kalmonson, Ubaidullah Baryar, Joshua Ng
//File Name: Game1.cs
//Project Name: MiningAwayISU
//Creation Date: Jan. 2018
//Modified Date: Jan. 2018
//Description: This is a imitation of the game terraria but to a much smaller scale in terms of time count, map size,
//objects, and overall capability in the game
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TempISU12;

namespace MiningAwayISU
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Hold the button images
        Texture2D newGameBtnImg;
        Texture2D loadGameBtnImg;
        Texture2D createGameBtnImg;
        Texture2D optionsBtnImg; 
        Texture2D guideBtnImg; 
        Texture2D exitBtnImg; 
        Texture2D continueBtnImg;
        Texture2D returnBtnImg;

        //Hold the background images
        Texture2D newGameBgImg;
        Texture2D menuBgImage;
        Texture2D inventBgImage;
        Texture2D guideBgImage;

        //Hold the character sprite
        Texture2D femaleCharWalking;
        Texture2D maleCharWalking;

        //Hold the fonts
        SpriteFont mediumFont;
        SpriteFont smallFont;

        //used to animated each gender of player
        Animation femaleHero;
        Animation maleHero;

        //Rectangles and position to enter the name
        Rectangle nameBoxRec;
        Vector2 ngNameposition;

        //Continue button rec
        Rectangle nameScreenContinueRec;
        
        //Max values for when entering a name
        int ngMaxNameLength = 7;
        int ngMaxNameLengthPixels = 153;

        //Positions to draw the guide to ingame and inventory
        Vector2 guideInGame;
        Vector2 guideInventory;

        //Bool to check if the fade is occuring. Elaspsed time to see how long the fade has been going on and fade color
        //is the actual fade
        bool fadeStatus;
        int elapsedFadeTime;
        Color fadeColor = Color.White;

        //DELETE
        int h;
        bool z = true;

        //Timers to set up the flashing line when entering a text
        int flashingLineTimerOn;
        int flashingLineTimerOff = 1;

        //Bools to track if the name is being enterd and drawn
        bool enteringPhrase;
        bool drawName;

        //Keys list that keeps track of the first key pressed
        Keys[] keys = new Keys[1];

        Flyer enemy1;
        Walker enemy2;

        //The size of the screen
        int screenWidth = 800;
        int screenHeight = 480;

        //Rectangle to draw the character
        Rectangle femaleChooseCharacterRec;
        Rectangle maleChooseCharacterRec;

        //Rectangle for the expantion and minimization of choosen charact
        Rectangle notChoosenCharacterRecF;
        Rectangle notChoosenCharacterRecM;
        Rectangle choosenCharacterHighlightF;
        Rectangle choosenCharacterHighlightM;
       
        //Variables to hold the size of the background and the menu boxes 
        Rectangle bgRec;
        Rectangle[] menuButtonBoxes = new Rectangle[6];
       
        //Set up an instance of the collision class
        Collision collision = new Collision();

        //A stack to keep track of gamestate
        Stack<ScreenType> gameState = new Stack<ScreenType>();
       
        //the nexxt screen that the game will go to
        ScreenType nextScreen;

        //Variables to keep track of the current and previous keyboards
        KeyboardState kb;
        KeyboardState prevKb;
       
        //Variables to keep track of the current and previous mice
        MouseState mouse;
        MouseState prevMouse;
                
        //Soundeffects and instances
        SoundEffect companionCollectionSE;
        SoundEffectInstance companionCollectionSEI;
        SoundEffect mouseClickSE;
        SoundEffectInstance mouseClickSEI;

        //int to track companions
        int companionsCollected = 0;

        //Background song
        Song bgSong;

        //terrain builder obj
        TerrainBuilder tB;

        //image for companions
        Texture2D compImg;

        //List of companions
        List<Companion> companions = new List<Companion>();

        //map dimensions
        int mapWidth = 300;
        int mapHeight = 300;

        //distance to generate new chunk
        int minDist = 800;

        Map map;
        List<CollisionTiles> allTiles = new List<CollisionTiles>();

        // Variable for each class.
        Player player;
        HitTile hitTile;
        Camera camera;
        //Building build;

        int tempTimer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Set the size of the screen
            this.graphics.PreferredBackBufferWidth = screenWidth;
            this.graphics.PreferredBackBufferHeight = screenHeight;

            //Shows mouse in program
            this.IsMouseVisible = true;

            //Applies graphical changes
            //this.graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Set the background rec size
            bgRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //Setup the menu boxes
            menuButtonBoxes[0] = new Rectangle(45, 175, 125, 50);
            menuButtonBoxes[1] = new Rectangle(45, 240, 125, 33);
            menuButtonBoxes[2] = new Rectangle(45, 283, 125, 33);
            menuButtonBoxes[3] = new Rectangle(45, 326, 125, 33);
            menuButtonBoxes[4] = new Rectangle(45, 369, 125, 33);
            menuButtonBoxes[5] = new Rectangle(45, 414, 125, 33);

            //Set up the female and male character selection boxes
            femaleChooseCharacterRec = new Rectangle(80, 240, 134, 224);
            maleChooseCharacterRec = new Rectangle(600, 240, 134, 224);

            //Set up the small rectangles for the males and female
            notChoosenCharacterRecM = new Rectangle(maleChooseCharacterRec.X,
                                                    maleChooseCharacterRec.Y,
                                                    maleChooseCharacterRec.Width,
                                                    maleChooseCharacterRec.Height);
            notChoosenCharacterRecF = new Rectangle(femaleChooseCharacterRec.X,
                                                    femaleChooseCharacterRec.Y,
                                                    femaleChooseCharacterRec.Width,
                                                    femaleChooseCharacterRec.Height);

            //Set up the big rectangles for the males and female that is choosen
            choosenCharacterHighlightM = new Rectangle(maleChooseCharacterRec.X - 15,
                                                       maleChooseCharacterRec.Y - 15,
                                                       maleChooseCharacterRec.Width + 30,
                                                       maleChooseCharacterRec.Height + 30);
            choosenCharacterHighlightF = new Rectangle(femaleChooseCharacterRec.X - 15,
                                                       femaleChooseCharacterRec.Y - 15,
                                                       femaleChooseCharacterRec.Width + 30,
                                                       femaleChooseCharacterRec.Height + 30);

            //Set up the name location and rec
            nameBoxRec = new Rectangle(290, 118, 219, 67);
            nameScreenContinueRec = new Rectangle(290, 400, 219, 60);
            ngNameposition = new Vector2(310, 130);

            //Set up the position to draw the instructions for ingame and inventory
            guideInGame = new Vector2(20, 20);
            guideInventory = new Vector2(20, 200);

            enemy1 = new Flyer(800, 600, player);
            enemy2 = new Walker(800, 600, allTiles, player);


            //Set the beginging screen to menu
            gameState.Push(ScreenType.Menu);


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load in all images, fonts and sounds
            LoadBgImages(Content);
            LoadBtnAndRecImages(Content);
            LoadFonts(Content);
            LoadSprites(Content);
            LoadSounds(Content);

            //Play song, lower volume for user comfort and put it on loop to play on repeat
            MediaPlayer.Play(bgSong);
            MediaPlayer.Volume = 0.05f;
            MediaPlayer.IsRepeating = true;

            map = new Map(Content, smallFont, mediumFont, collision);

            femaleHero = new Animation(femaleCharWalking, 1, 12, 12, 0, 0, -1, 10, femaleChooseCharacterRec, true, 33, 56);
            maleHero = new Animation(maleCharWalking, 1, 13, 13, 0, 0, -1, 10, maleChooseCharacterRec, true, 33, 56);
          
            Tiles.Content = Content;

            camera = new Camera(GraphicsDevice.Viewport);

            //initialize terrain builder object (terrainWidth, terrainHeight)
            tB = new TerrainBuilder(mapWidth, mapHeight);

            //generate the map from 0, 0
            allTiles = map.Generate(tB.Build(0, 0, TerrainBuilder.GEN_RIGHT), Tiles.TILE_SIZE);
            player = new Player(800, 600, allTiles);
           
            companions.Add(new Companion(new Vector2(player.Position.X, player.Position.Y)));

            player.Load(Content);
            enemy1.Load(Content);
            enemy2.Load(Content);

            //gameState.Push(ScreenType.InGame);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            //Set the previous keyboard and update the keybaord
            prevKb = kb;
            kb = Keyboard.GetState();

            //Set the previous mouse and update the mouse
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //Depending on the gameState that the player is currently on, only do that update code
            switch (gameState.Top())
            {
                case ScreenType.Menu:
                    UpdateMenu();
                    break;

                case ScreenType.NewGame:
                    UpdateNewGame(gameTime);
                    break;

                case ScreenType.Guide:
                    UpdateGuide();
                    break;

                case ScreenType.InGame:
                    UpdateInGame(gameTime);
                    break;

                case ScreenType.Inventory:
                    UpdateInventory(gameTime);
                    break;

                case ScreenType.Exit:
                    this.Exit();
                    break;
            }            
            
            //Update the player and camera
            player.Update(gameTime);
            camera.Update(new Vector2(player.GetRectangle().X, player.GetRectangle().Y), mapWidth * Tiles.TILE_SIZE, mapHeight * Tiles.TILE_SIZE);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);

            //Swtich statement for the gameState. Draw the things for that screen that are locked with the camera
            switch (gameState.Top())
            {
                case ScreenType.InGame:
                    DrawInGame(spriteBatch);

                    //Draw the player
                    player.Draw(spriteBatch);
                    break;
            }

            spriteBatch.End();

            //These things are not cameraa depended
            spriteBatch.Begin();

            //Swtich statement for the gameState. Draw the things for that screen type
            switch (gameState.Top())
            {
                case ScreenType.Menu:
                    DrawMenu(spriteBatch);
                    break;

                case ScreenType.NewGame:
                    DrawNewGame(spriteBatch);
                    break;

                case ScreenType.Guide:
                    DrawGuide(spriteBatch);
                    break;

                case ScreenType.Inventory:
                    DrawInventory(spriteBatch);

                    //If the item count it reached, display that the inventory is full
                    if (map.GetInventory().PlayerItems.Count() == 25)
                    {
                        spriteBatch.DrawString(mediumFont, "INVENTORY IS FULL", new Vector2(10, 430), Color.White);
                    }
                    break;

                case ScreenType.InGame:
                    map.GetInventory().DrawCurrentItemsBar(spriteBatch);

                    //If the item count it reached, display that the inventory is full
                    if (map.GetInventory().PlayerItems.Count() == 25)
                    {
                        spriteBatch.DrawString(mediumFont, "INVENTORY IS FULL", new Vector2(10, 70), Color.White);
                    }
                    break;
            }

            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        #region Load All Images
        //Pre: Content manger to load things
        //Post: Loads buttons and rectangles
        //Desc: Loads buttons adn rectangeles
        public void LoadBtnAndRecImages(ContentManager content)
        {
            //Buttons
            createGameBtnImg = Content.Load<Texture2D>("Images/Buttons/CreateButton");
            exitBtnImg = Content.Load<Texture2D>("Images/Buttons/ExitButton");
            guideBtnImg = Content.Load<Texture2D>("Images/Buttons/GuideButton");
            loadGameBtnImg = Content.Load<Texture2D>("Images/Buttons/LoadButton");
            newGameBtnImg = Content.Load<Texture2D>("Images/Buttons/NewGameButton");
            optionsBtnImg = Content.Load<Texture2D>("Images/Buttons/OptionsButton");
            continueBtnImg = Content.Load<Texture2D>("Images/Buttons/ContinueButton");
            returnBtnImg = Content.Load<Texture2D>("Images/Buttons/ReturnButton");
        }

        //Pre: Content manger to load things
        //Post: Loads bg images
        //Desc: Loads bg images
        public void LoadBgImages(ContentManager content)
        {
            menuBgImage = Content.Load<Texture2D>("Images/BgImages/MenuImage");
            newGameBgImg = Content.Load<Texture2D>("Images/BgImages/NewGameScreen");
            inventBgImage = Content.Load<Texture2D>("Images/BgImages/inventBg");
            guideBgImage = Content.Load<Texture2D>("Images/BgImages/guideBg");
        }

        //Pre: Content manger to load things
        //Post: Loads sprties
        //Desc: Loads sprties
        public void LoadSprites(ContentManager content)
        {
            //Player walking Images
            femaleCharWalking = Content.Load<Texture2D>("Images/Sprites/PlayerS/Female_Hero");
            maleCharWalking = Content.Load<Texture2D>("Images/Sprites/PlayerS/Male_Hero");
            
            //Companion image
            compImg = Content.Load<Texture2D>("Images/companionImg");
        }

        //Pre: Content manger to load things
        //Post: Loads fonts
        //Desc: Loads fonts
        public void LoadFonts(ContentManager content)
        {
            mediumFont = Content.Load<SpriteFont>("Fonts/mediumFont");
            smallFont = Content.Load<SpriteFont>("Fonts/smallFont");
        }

        //Pre: Content manager
        //Post: none
        //Desc: load all sounds and set their instances
        public void LoadSounds(ContentManager content)
        {
            //load companion collection sound effect and set its instance
            companionCollectionSE = Content.Load<SoundEffect>("Sounds/companionCollection");
            companionCollectionSEI = companionCollectionSE.CreateInstance();

            //load mouse click sound effect
            mouseClickSE = Content.Load<SoundEffect>("Sounds/mouseClickSound");
            mouseClickSEI = mouseClickSE.CreateInstance();

            //load background song
            bgSong = Content.Load<Song>("Sounds/backgroundSong");
        }

        #endregion

        #region Menu Fuctionality And Drawing

        //Pre: N/A
        //Post: Update the menu
        //Desc: Check for button clicks to change the gameState and fades
        public void UpdateMenu()
        {           
            //If fade is true, fade the screen
            if (fadeStatus)
            {
                FadeAway(nextScreen);
            }
            else
            {
                //Check if the mouse was clicked
                if (collision.CheckMouseClick(mouse, prevMouse))
                {
                    //Loop through all the menu buttons
                    for (int i = 0; i < menuButtonBoxes.Length; ++i)
                    {
                        //Check a collision with those boxes and if there was a collision, set fade to true,
                        //set the next screen and play a sound
                        if (collision.CheckCollision(mouse, menuButtonBoxes[i]))
                        {
                            fadeStatus = true;
                            nextScreen = (ScreenType)(i + 1);
                            mouseClickSEI.Play();
                        }
                    }
                }
            }
        }

        //Pre: spriteBatch allows for images and text to be drawn
        //Post: Draw the menu
        //Desc: Draw the background image and all the buttons
        public void DrawMenu(SpriteBatch spriteBatch)
        {           
            //Draw the background
            spriteBatch.Draw(menuBgImage, bgRec, fadeColor);

            //Draw the buttons
            spriteBatch.Draw(newGameBtnImg, menuButtonBoxes[0], fadeColor);
            spriteBatch.Draw(loadGameBtnImg, menuButtonBoxes[1], fadeColor);
            spriteBatch.Draw(createGameBtnImg, menuButtonBoxes[2], fadeColor);
            spriteBatch.Draw(optionsBtnImg, menuButtonBoxes[3], fadeColor);
            spriteBatch.Draw(guideBtnImg, menuButtonBoxes[4], fadeColor);
            spriteBatch.Draw(exitBtnImg, menuButtonBoxes[5], fadeColor);
        }
        #endregion

        #region New Game Functionality And Drawing

        //Pre: gameTime keeps track of the amount of time passed since last update
        //Post: Update the newGame screen
        //Desc: Update fades, animations, check for mouse clicks and key presses
        public void UpdateNewGame(GameTime gameTime)
        {
            //If fade is true, start the fade
            if (fadeStatus)
            {
                FadeAway(nextScreen);
            }
            else
            {
                //Update the male and female animations
                femaleHero.Update(gameTime);
                maleHero.Update(gameTime);

                //Check if the button was clicked
                if (collision.CheckMouseClick(mouse, prevMouse))
                {
                    //Check collision with the name box
                    enteringPhrase = collision.CheckCollision(mouse, nameBoxRec);

                    //If there was no collision, set the flashing line timers to false
                    if (!enteringPhrase)
                    {
                        flashingLineTimerOff = 1;
                        flashingLineTimerOn = 0;
                        drawName = false;

                        //play mouse click sound effect
                        mouseClickSEI.Play();
                    }
                }

                //If the palyer is entering a phrase
                if (enteringPhrase)
                {
                    //Check if the key is valid, and if it is, update the player name
                    if (keys.Length != 0 &&
                        (prevKb.IsKeyDown(keys[0]) && kb.IsKeyUp(keys[0])))
                    {
                        player.SetName(AddToString(keys[0], player.GetName()));
                    }

                    //Update the keys pressed
                    keys = kb.GetPressedKeys();

                    //Update the flashing line
                    FlashingLineUpdate();
                }

                //Check for a mouse click
                if(collision.CheckMouseClick(mouse, prevMouse))
                {
                    //Choose a character
                    ChooseCharacterOptions();

                    //play mouse click sound effect
                    mouseClickSEI.Play();
                }

                //Depending on the gender that was choosen, make the choosen one bigger and the other one smaller
                if (player.GetGender() == "male")
                {
                    maleHero.destRec = choosenCharacterHighlightM;

                    femaleHero.destRec = notChoosenCharacterRecF;
                }
                else if (player.GetGender() == "female")
                {
                    femaleHero.destRec = choosenCharacterHighlightF;

                    maleHero.destRec = notChoosenCharacterRecM;
                }
            }
        }

        //Pre: spriteBatch allows for images and text to be drawn
        //Post: Draw the NewGameScreen
        //Desc: Draw the background, the choice of characters, the phrase and entering name box. Display to the user
        //if a name or character have not been choose/entereed
        public void DrawNewGame(SpriteBatch spriteBatch)
        {
            //Draw the background and the continue button
            spriteBatch.Draw(newGameBgImg, bgRec, fadeColor);
            spriteBatch.Draw(continueBtnImg, nameScreenContinueRec, fadeColor);

            //Draw the male and female characters
            femaleHero.Draw(spriteBatch, fadeColor, SpriteEffects.None);
            maleHero.Draw(spriteBatch, fadeColor, SpriteEffects.None);

            //Draw the entered name
            DrawEnteredPhrase(spriteBatch, mediumFont, player.GetName(), 
                              ngNameposition, ngMaxNameLength, ngMaxNameLengthPixels);

            //If the gender has not been choosen, inform the user of such
            if (player.GetGender() == "")
            {
                spriteBatch.DrawString(smallFont, 
                                       "Please Choose A Character", 
                                       new Vector2(260, 200), 
                                       Color.Gold);
            }

            //If the name has not been entered, inform the user of such
            if (player.GetName() == "")
            {
                spriteBatch.DrawString(smallFont,
                                       "Please Enter A Name",
                                       new Vector2(300, 220),
                                       Color.Gold);
            }
        }

        //Pre: N/A
        //Post: Sets the gender of the player and moves to inGame
        //Desc: Checks collision with the 2 types of characters to determine gender and with the button to move on
        public void ChooseCharacterOptions()
        {
            //Check if the player clicked the female or male character and set the gender accordingly
            if (collision.CheckCollision(mouse, femaleChooseCharacterRec))
            {
                player.SetGender("female");
            }
            else if (collision.CheckCollision(mouse, maleChooseCharacterRec))
            {
                player.SetGender("male");
            }

            //Check if the player clicked the continue button
            if (collision.CheckCollision(mouse, nameScreenContinueRec))
            {
                //If both a gender and name have been choose/entered, move to ingame and start the fade
                if (player.GetGender() != "" && 
                    player.GetName() != "")                
                {
                    fadeStatus = true;
                    nextScreen = ScreenType.InGame;
                }
            }
        }      

        #endregion

        #region Guide Functionality and Drawing
        //Pre: N/A
        //Post: If the button was clicked, the player goes back to the meanu
        //Desc: If the button was clicked, the palyer goes back to the menu, if it wasn't clicked, the player stays at the guide screen
        public void UpdateGuide()
        {
            //Check if the mouse was clicked on the return button and if it was, return to the menu
            if(collision.CheckMouseClick(mouse, prevMouse) &&
                collision.CheckCollision(mouse, nameScreenContinueRec))
            {
                gameState.Pop();
            }
         
        }

        //Pre: spriteBatch allows for images and text to be drawn
        //Post: Draw the guide screen
        //Desc: Draws text to show the player how to play the game and use the inventory
        public void DrawGuide(SpriteBatch spriteBatch)
        {
            //Draw the background image
            spriteBatch.Draw(guideBgImage, bgRec, Color.IndianRed);

            //Draw in the game instructions
            spriteBatch.DrawString(mediumFont, "In Game Instructions", guideInGame, Color.Yellow);
            spriteBatch.DrawString(smallFont, "1. 'A' and 'S' to move side to side.", new Vector2(guideInGame.X, guideInGame.Y + 30), Color.GreenYellow);
            spriteBatch.DrawString(smallFont, "2. Space bar to jump.", new Vector2(guideInGame.X, guideInGame.Y + 50), Color.GreenYellow);
            spriteBatch.DrawString(smallFont, "3. 'Esc' to exit.", new Vector2(guideInGame.X, guideInGame.Y + 70), Color.GreenYellow);
            spriteBatch.DrawString(smallFont, "4.'E' to acceess inventory ", new Vector2(guideInGame.X, guideInGame.Y + 90), Color.GreenYellow);
            spriteBatch.DrawString(smallFont, "5.Right-Click to destory a block", new Vector2(guideInGame.X, guideInGame.Y + 110), Color.GreenYellow);
            spriteBatch.DrawString(smallFont, "6.Left-Click to place a block", new Vector2(guideInGame.X, guideInGame.Y + 130), Color.GreenYellow);

            //Draw the inventroy instructions
            spriteBatch.DrawString(mediumFont, "Inventory Instructions", guideInventory, Color.Yellow);
            spriteBatch.DrawString(smallFont, "1.'E' to exit inventory ", new Vector2(guideInventory.X, guideInventory.Y + 30), Color.GreenYellow);
            spriteBatch.DrawString(smallFont, "2.Items in the inventory slot can be moved by dragging and placing", new Vector2(guideInventory.X, guideInventory.Y + 50), Color.GreenYellow);
            spriteBatch.DrawString(smallFont, "3.Items can be searched by name", new Vector2(guideInventory.X, guideInventory.Y + 70), Color.GreenYellow);
            spriteBatch.DrawString(smallFont, "4.Hovering over items, reveals it's stats", new Vector2(guideInventory.X, guideInventory.Y + 90), Color.GreenYellow);

            //Draw the back button
            spriteBatch.Draw(returnBtnImg, nameScreenContinueRec, Color.White);
        
        }
        #endregion

        #region InGame Functionality And Drawing

        //Pre: gameTime keeps track of the amount of time passed since last update
        //Post: Do any logic involvec with updateing the game
        //Desc: Check for buttons clicks to exit the game/go into inventory, check for destruction of blocks, the placement of blocks,
        //the picking up of blocks, update the terrain and the player
        public void UpdateInGame(GameTime gameTime)
        {
            //lower the volume of the background music
            MediaPlayer.Volume = 0.1f;

            //If e is clicked, go to the inventory. If escape is clicked, exit the game
            if (prevKb.IsKeyDown(Keys.E) &&
               kb.IsKeyUp(Keys.E))
            {
                gameState.Push(ScreenType.Inventory);
                map.GetInventory().SearchForItem();
            }
            else if (prevKb.IsKeyDown(Keys.Escape) &&
                     kb.IsKeyUp(Keys.Escape))
            {
                this.Exit();
            }

            // Check to see if player clicks the building button.
            if(collision.CheckRightMouseClick(mouse,prevMouse))
            {
                map.Update(spriteBatch, mouse);
            }

            // Check to see if player clicks destruction button.
            if(collision.CheckMouseClick(mouse,prevMouse))
            {
                map.Update(spriteBatch, mouse);
            }
            if (mouse.ScrollWheelValue != prevMouse.ScrollWheelValue)
            {
                map.GetInventory().ScrollEffectItemsBar(mouse.ScrollWheelValue, prevMouse.ScrollWheelValue);
            }

            //find the distance between the last loaded chunk and player (divide tile int by 3 to get pixels)
            float distanceRight = (tB.lastTileRight * Tiles.TILE_SIZE) - player.GetRectangle().X;
            //float distanceLeft = player.GetRectangle().X - (tB.lastTileLeft * Tiles.TILE_SIZE);

            //if the character is closer to the right of the last loaded chunk, load from the right, and if there is enough space to generate
            if (distanceRight < minDist && mapWidth - tB.lastTileRight > TerrainBuilder.LENGTH_OF_CHUNKS)
            {
                ///map.Generate(tB.Build(tB.lastTileRight, 0, TerrainBuilder.GEN_RIGHT), Tiles.TILE_SIZE);
                //allTiles = map.collisionTilesList;
                //player.UpdateMap(allTiles);

                //add a new companion for the chunk
                companions.Add(new Companion(tB.GetCompanionLoc(tB.lastTileRight)));
            }

            player.Update(gameTime);
            enemy1.Update(gameTime);
            enemy2.Update(gameTime);

            //if a companion is collected, fade it out and remove it
            foreach (Companion companion in companions)
            {
                if (collision.CheckCollision(companion.GetRect(), player.GetRectangle()))
            {
                    //set fade status to true
                    fadeStatus = true;

                    //do the fade away for the companion
                    FadeAway(companion);
                }
            }


            //TEMPPP!!! ONLY FOR TESTING CRAFTING
            ++tempTimer;
            if (tempTimer % 4 == 0)
            {
                map.GetInventory().PlayerItems.Add(new Vector2(1, 1));
                map.GetInventory().PickedUpItem = true;
            }
            else if (tempTimer % 5 == 0)
            {
                map.GetInventory().PlayerItems.Add(new Vector2(2, 1));
                map.GetInventory().PickedUpItem = true;
            }
            else if (tempTimer % 6 == 0)
            {
                map.GetInventory().PlayerItems.Add(new Vector2(3, 1));
                map.GetInventory().PickedUpItem = true;
            }
            else if (tempTimer % 7 == 0)
            {
                map.GetInventory().PlayerItems.Add(new Vector2(4, 1));
                map.GetInventory().PickedUpItem = true;
            }
            //TESTING GOES UNTIL THIS POINT



            //Check if the player picked up anything
            map.GetInventory().CheckStackAndPickUp();
        }

        //Pre: spriteBatch allows images and text to be drawn
        //Post: Draw everything for the ingame state
        //Desc: draw the map, the player and the companion
        public void DrawInGame(SpriteBatch spriteBatch)
        {
            //Draw the map and player
            map.Draw(spriteBatch);
            player.Draw(spriteBatch);


            //draw the companions if they are not yet saved
            foreach (Companion companion in companions)
            {
                if (companion.GetIsSaved() == false)
                {
                spriteBatch.Draw(compImg, companion.GetRect(), fadeColor);

                    //draw the companion counter
                    spriteBatch.DrawString(smallFont, "Companions collected: " + companionsCollected, new Vector2(companion.GetRect().X, companion.GetRect().Y + 30), Color.White);
            }

            }
        }
        #endregion

        #region Inventory Functionality And Drawing

        //Pre: gameTime keeps track of the amount of time passed since last update
        //Post: Inventory is updataed
        //Desc: CHecks button clicks, mouse position to output stats, craft items, or move items
        public void UpdateInventory(GameTime gameTime)
        {
            //If e is clicked, go back to the in game screen and set whatever is searched to blank
            if (prevKb.IsKeyDown(Keys.E) &&
               kb.IsKeyUp(Keys.E) && !enteringPhrase)
            {
                gameState.Pop();
                map.GetInventory().SearchItemName = "";
            }

            //Check there was a click, check if it was with the item entering search box
            if (collision.CheckMouseClick(mouse, prevMouse))
            {
                enteringPhrase = collision.CheckCollision(mouse, map.GetInventory().SearchItemRec);
            }

            //Is the player entering an item name
            if (enteringPhrase)
            {
                //If the keys are valid, add them to the search item string
                if (keys.Length != 0 &&
                   (prevKb.IsKeyDown(keys[0]) && kb.IsKeyUp(keys[0])))
                {
                    map.GetInventory().SearchItemName = AddToString(keys[0], map.GetInventory().SearchItemName);
                }

                //Get the keys pressed
                keys = kb.GetPressedKeys();

                //Update the flashing line
                FlashingLineUpdate();

                //Search for an item
                map.GetInventory().SearchForItem();
            }
            else
            {
                //Reset the flashing line values
                flashingLineTimerOff = 1;
                flashingLineTimerOn = 0;
                drawName = false;
            }

            //Check if the item is being moved or the stats need to be drawn
            map.GetInventory().StatsOrMove(mouse);

            //Check if the searched item is being clicked/hovered over
            map.GetInventory().SearchedItemColCollision(mouse, prevMouse);

            //Craft and item and check if an item needs to be picked up/stacked
            map.GetInventory().CraftNewItem();
            map.GetInventory().CheckStackAndPickUp();
        }

        //Pre: spriteBatch allows for images and text to be drawn
        //Post: Draw the inventory
        //Desc: Draw the bg image, the inventory slots, stats and the name entering part
        public void DrawInventory(SpriteBatch spriteBatch)
        {
            //Draw the background image
            spriteBatch.Draw(inventBgImage, bgRec, Color.SandyBrown);

            //Draw the inventory storage and stats
            map.GetInventory().DrawInventoryStorage(spriteBatch);
            map.GetInventory().DrawInventoryStats(spriteBatch, 
                                                        map.GetInventory().StatsPosPlayer, 
                                                        map.GetInventory().PlayerItems, 
                                                        map.GetInventory().HoveredInventItem);
            //Draw the search bar the phrase
            map.GetInventory().DrawSearch(spriteBatch);
            DrawEnteredPhrase(spriteBatch, mediumFont, map.GetInventory().SearchItemName,
                              map.GetInventory().ItemNameSearchPlacement, 10, 217);

            //Draw the searched item stats
            map.GetInventory().DrawInventoryStats(spriteBatch,
                                                  map.GetInventory().StatsPosSearched,
                                                  map.GetInventory().ItemsTable,
                                                  map.GetInventory().HoveredSearchItem);
            
        }
        #endregion

        #region Support Subprograms For UI

        //Pre: nextScreen is the next scrren the game should move to
        //Post: Once the fade ends, move to the next state
        //Desc: Keep fading the color until it gets past a certain point and then move to the next screen
        public void FadeAway(ScreenType nextScreen)
        {
            //Update timer
            elapsedFadeTime++;

            //Check if the A value of the color is low enough. If it is, set the fade to false and move to the next gameState
            if (fadeColor.A <= 1)
            {
                fadeStatus = false;
                gameState.Push(nextScreen);
            }

            //If the elasped time has reached a certain frame count, start changing the fade color
            if (elapsedFadeTime >= 30)
            {
                fadeColor.A -= 2;
            }
        }

        //Pre: Companion is the companion with the player
        //Post: plays a sound
        //Desc: Fades the image and once the image has faded, play a sound
        public void FadeAway(Companion companion)
        {
            //Update timer
            elapsedFadeTime++;

            //Check if the A value of the color is low enough. If it is, set the fade to false and play a sound

            if (fadeColor.A <= 1)
            {
                fadeStatus = false;

                //set the compaions status to true and play the sound effect
                companion.SetIsSaved(true);
                companionCollectionSEI.Play();
                companionsCollected++;
            }

            //If the elasped time has reached a certain frame count, start changing the fade color
            if (elapsedFadeTime >= 30)
            {
                fadeColor.A -= 2;
            }
        }

        //Pre: N/A
        //Post: timers are set to true or false to make a line flash
        //Desc: Once the flash has gone on long enough, set it to false and keep the flash off for a certain amount of
        //time and the go back to flashing and repeat the process
        public void FlashingLineUpdate()
        {           
            //Has enough time passed for the flashing on light. If enough time has passed, start the off timer
            if (flashingLineTimerOn % 30 == 0)
            {
                flashingLineTimerOff++;
            }

            //Has enough time passed for the flashing off light. If enough time has passed, start the on timer and set draw to true
            if (flashingLineTimerOff % 30 == 0)
            {
                drawName = true;
                flashingLineTimerOn++;
                return;
            }

            //Set draw to false
            drawName = false;
        }

        //Pre: keys is the key that was pressed and phrase is the string being modified
        //Post: phrase is updated with the keys and then it is returned
        //Desc: Depending on the key pressed, the phrase may be updated
        public string AddToString(Keys keys, string phrase)
        {
            //Temp char variable
            char charValue;

            //Depending on the key entered, do different things
            if (char.TryParse(keys.ToString(), out charValue))
            {
                //Add the value of key to the phrase
                phrase += keys.ToString();
            }
            else if (keys == Keys.Back && phrase.Length != 0)
            {
                //Remove the last letter of the phrase
                phrase = phrase.Remove(phrase.Length - 1);
            }
            else if (keys == Keys.Space)
            {
                //Add a space into the phrase
                phrase += " ";
            }

            //return phrase
            return phrase;
        }

        //Pre: spritebatch allows text to be drawn, font is the font to be used, phrase is the phrase being outputted,
        //position is where the text is being outputted, maxStringSize is the number of letters before the text loops and
        //maxVal is the maxPositions of the flashing line
        //Post: Displace the entered phrase and have the flashing line going
        //Desc: Displace the entered phrase and have the flashing line going
        public void DrawEnteredPhrase(SpriteBatch spriteBatch, SpriteFont font, string phrase,
                                      Vector2 position, int maxStringSize, int maxVal)
        {

            //If the phrase is longer than or equal to the max size, subtring the end part of it
            if (phrase.Length >= maxStringSize)
            {
                phrase = phrase.Substring(phrase.Length - maxStringSize);
            }

            //If name is being draw, draw the flashing line
            if (drawName)
            {
                spriteBatch.DrawString(font,
                                       "_",
                                       new Vector2(position.X + Math.Min(font.MeasureString(phrase).X,
                                                                         maxVal),
                                                   position.Y - 11),
                                       fadeColor);
            }

            //Draw the phrase
            spriteBatch.DrawString(font,
                                   phrase,
                                   position,
                                   fadeColor);
        }

        #endregion
    }
}

