using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinotaurEscape
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // atributes
        Maze maze;
        Player player;

        // Menu Items
        MenuItem playButton;
        MenuItem stopButton;
        MenuItem menuTitle;
        MenuItem pauseDeclare;
        MenuItem wonDeclare;
        MenuItem replayButton;

        // Used for keyboard input
        private KeyboardState kbState, previousKbState;

        // Used for player move input
        private List<Keys> moveInput = new List<Keys>();

        public enum GameState
        {
            MainMenu,
            Play,
            Pause,
            GameWon
        }

        GameState stateGame;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            // Create the player with 5 torches and 5 lives to start
                player = new Player(5, new Vector2((GraphicsDevice.Viewport.Width-GameVariables.TileSize)/2, (GraphicsDevice.Viewport.Height - GameVariables.TileSize) / 2));
                player.Torches = 5;

            //Set up the menu
            playButton = new MenuItem(GameVariables.MenuPlayButtonTexture);
            stopButton = new MenuItem(GameVariables.MenuStopButtonTexture);
            menuTitle = new MenuItem(GameVariables.MenuTitleTexture);
            replayButton = new MenuItem(GameVariables.ReplayButtonTexture);
            wonDeclare = new MenuItem(GameVariables.WonDeclareTexture);
            pauseDeclare = new MenuItem(GameVariables.PauseDeclareTexture);

            //Set initial state
            stateGame = GameState.MainMenu;

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

            // Load the textures of the game
                GameVariables.LoadTextures(Content, GraphicsDevice);

            // Setup all the animations of the game
                Player.SetupAnimations();
                Torch.SetupAnimations();
                Comrade.SetupAnimations();
                Minotaur.SetupAnimations();
                player.Animation = Player.MovingAnimation; // Do this here for now, will be remove when Idle animation is created

            //Menu Textures
                playButton.ButtonGraphic = GameVariables.MenuPlayButtonTexture;
                stopButton.ButtonGraphic = GameVariables.MenuStopButtonTexture;
                menuTitle.ButtonGraphic = GameVariables.MenuTitleTexture;
                pauseDeclare.ButtonGraphic = GameVariables.PauseDeclareTexture;
                replayButton.ButtonGraphic = GameVariables.ReplayButtonTexture;
                wonDeclare.ButtonGraphic = GameVariables.WonDeclareTexture;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            // Store the previous keyboard state and get the new one
                previousKbState = kbState;
                kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.Escape) && previousKbState.IsKeyUp(Keys.Escape) && stateGame == GameState.MainMenu)
                Exit();

            if (stateGame == GameState.MainMenu )
            {
                IsMouseVisible = true;
                if (playButton.IsMouseInside() == true && playButton.stateMouse.LeftButton == ButtonState.Pressed)
                {
                    stateGame = GameState.Play;
                    LoadMap();
                }
                if (stopButton.IsMouseInside() == true && playButton.stateMouse.LeftButton == ButtonState.Pressed)
                {
                    Exit();
                }
            }


            else if(stateGame == GameState.Play)
            {
                // Process input
                    ProcessInput(gameTime);

                // Check if player has hit or "collected" a comrade
                    Comrade comrade = maze.IntersectingComrade(player);
                    if (comrade != null)
                    {
                        // Add to the player's torches and remove the comrade
                            player.Torches++;
                            maze.RemoveComrade(comrade);

                    }

                // Check if a player has hit a minotuar then move the minotuars and check again
                    Minotaur minotaur = maze.IntersectingMinotuar(player);
                    if (minotaur != null)
                            stateGame = GameState.MainMenu; // Return to main menu
                    maze.MoveMinotuars(gameTime);
                    minotaur = maze.IntersectingMinotuar(player);
                    if (minotaur != null)
                        stateGame = GameState.MainMenu; // Return to main menu

                // Check if the player has won
                    if(maze.IsInExit(player))
                        stateGame = GameState.GameWon; // Return to main menu
            }

            else if (stateGame == GameState.Pause)
            {
                if ((kbState.IsKeyDown(Keys.LeftControl) && previousKbState.IsKeyUp(Keys.LeftControl)) || (kbState.IsKeyDown(Keys.RightControl) && previousKbState.IsKeyUp(Keys.RightControl)))
                {
                    stateGame = GameState.Play;
                }
            }

            else if(stateGame == GameState.GameWon)
            {
                if (replayButton.IsMouseInside() == true && replayButton.stateMouse.LeftButton == ButtonState.Pressed)
                {
                    stateGame = GameState.Play;
                    LoadMap();
                }

                if (stopButton.IsMouseInside() == true && stopButton.stateMouse.LeftButton == ButtonState.Pressed)
                {
                    Exit();
                }
            }

            base.Update(gameTime);
        }

        public void LoadMap()
        {
            /* Temporaily ask the player for a maze to load if maze doesn't exist */

            // Create a stream for loading the file
            Stream loadStream;

            // Create a dialog for asking for save location
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "maze files (*.maz)|*.maz";
            dialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\DefaultMazes\\";

            // Ask the user for a file to load
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && (loadStream = dialog.OpenFile()) != null)
                maze = new Maze(loadStream, GraphicsDevice.Viewport);
            else
                LoadMap();
        }

        public void ProcessInput(GameTime gameTime)
        {

            // Check if quit was pressed
                if (kbState.IsKeyDown(Keys.Escape) && previousKbState.IsKeyUp(Keys.Escape))
                {
                    stateGame = GameState.MainMenu;
                    return;
                }
                if ((kbState.IsKeyDown(Keys.LeftControl) && previousKbState.IsKeyUp(Keys.LeftControl)) || (kbState.IsKeyDown(Keys.RightControl) && previousKbState.IsKeyUp(Keys.RightControl)))
                {
                    stateGame = GameState.Pause;
                    return;
                }

            // Add the arrows just pressed to the list of arrows pressed
                if (previousKbState.IsKeyUp(Keys.W) && kbState.IsKeyDown(Keys.W))
                    moveInput.Add(Keys.W);
                if (previousKbState.IsKeyUp(Keys.A) && kbState.IsKeyDown(Keys.A))
                    moveInput.Add(Keys.A);
                if (previousKbState.IsKeyUp(Keys.D) && kbState.IsKeyDown(Keys.D))
                    moveInput.Add(Keys.D);
                if (previousKbState.IsKeyUp(Keys.S) && kbState.IsKeyDown(Keys.S))
                    moveInput.Add(Keys.S);

            // Remove the arrows released from the list of arrows pressed
                if (kbState.IsKeyUp(Keys.W))
                    moveInput.Remove(Keys.W);
                if (kbState.IsKeyUp(Keys.S))
                    moveInput.Remove(Keys.S);
                if (kbState.IsKeyUp(Keys.A))
                    moveInput.Remove(Keys.A);
                if (kbState.IsKeyUp(Keys.D))
                    moveInput.Remove(Keys.D);

            // Check if the player is moving and either idle or already moving
                if (moveInput.Count != 0 && (player.Animation == Player.MovingAnimation || player.Animation == Player.IdleAnimation))
                {
                    // Set the animation to moving just incase 
                        player.Animation = Player.MovingAnimation;
                        player.Animating = true;

                    // Check which was the last arrow pressed and move in that direction
                        switch (moveInput[moveInput.Count - 1])
                        {
                            case Keys.W:
                                maze.AttemptMove(gameTime, GameVariables.PlayerSpeed, false, player);
                                player.Direction = 0;
                                break;

                            case Keys.S:
                                maze.AttemptMove(gameTime, -GameVariables.PlayerSpeed, false, player);
                                player.Direction = 3;
                                break;

                            case Keys.D:
                                maze.AttemptMove(gameTime, -GameVariables.PlayerSpeed, true, player);
                                player.Direction = 2;
                                break;

                            case Keys.A:
                                maze.AttemptMove(gameTime, GameVariables.PlayerSpeed, true, player);
                                player.Direction = 1;
                                break;
                        }
                }

            // Check if torch button was pressed and the player has a torch
                if(previousKbState.IsKeyUp(Keys.Space) && kbState.IsKeyDown(Keys.Space) && player.Torches > 0 && maze.CanPlaceTorch(player.Position + new Vector2(GameVariables.CharacterSize / 2), player.Direction))
                {
                        // Add a torch to the maze from the player position and direction and then remove a torch from the player
                            maze.AddTorch(player.Position+new Vector2(GameVariables.CharacterSize / 2), player.Direction);
                            player.Torches--;
                }
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (stateGame == GameState.Play)
            {
                // Make background black
                    GraphicsDevice.Clear(Color.Black);

                // Draw the maze
                   maze.Draw(spriteBatch);

                // Place the player at the center of the screen and draw him
                    player.Position = new Vector2((GraphicsDevice.Viewport.Width - GameVariables.TileSize) / 2, (GraphicsDevice.Viewport.Height - GameVariables.TileSize) / 2);
                    player.Draw(spriteBatch);

                // Draw the player's torch count, live count, and point count
                    spriteBatch.Draw(GameVariables.SoildWhiteTexture, new Rectangle(GraphicsDevice.Viewport.Width-160, 10, 150, 100), Color.White);
                    spriteBatch.DrawString(GameVariables.BasicFont, "Lives: " + player.Lives, new Vector2(GraphicsDevice.Viewport.Width - 120, 25), Color.Black);
                    spriteBatch.DrawString(GameVariables.BasicFont, "Torches: " + player.Torches, new Vector2(GraphicsDevice.Viewport.Width - 120, 50), Color.Black);
                    spriteBatch.DrawString(GameVariables.BasicFont, "Points: " + player.Points, new Vector2(GraphicsDevice.Viewport.Width - 120, 75), Color.Black);

                // Draw the minimap
                    spriteBatch.Draw(GameVariables.SoildWhiteTexture, new Rectangle(new Point(10), new Point(GameVariables.minimapRadius*GameVariables.minimapSize*2)), Color.White);
                    maze.DrawMinimap(spriteBatch, GameVariables.minimapRadius, new Vector2(10), player.Position);

            }
            if (stateGame == GameState.Pause)
            {
                GraphicsDevice.Clear(Color.White);
                pauseDeclare.Rectangle = new Rectangle((GraphicsDevice.Viewport.Width - 750) / 2, (GraphicsDevice.Viewport.Height - 80) / 2, 750, 80);
                pauseDeclare.Draw(spriteBatch);
            }

            if (stateGame == GameState.MainMenu)
            {
                GraphicsDevice.Clear(Color.White);
                playButton.Rectangle = new Rectangle((GraphicsDevice.Viewport.Width - 32) / 2, (GraphicsDevice.Viewport.Height - 32) / 2, 32, 32);
                stopButton.Rectangle = new Rectangle((GraphicsDevice.Viewport.Width - 32) / 2, (playButton.Rectangle.Y + playButton.Rectangle.Height + 50), 32, 32);
                menuTitle.Rectangle = new Rectangle((GraphicsDevice.Viewport.Width - 490) / 2, 20, 490, 78);
                stopButton.Draw(spriteBatch);
                playButton.Draw(spriteBatch);
                menuTitle.Draw(spriteBatch);
            }

            if (stateGame == GameState.GameWon)
            {
                GraphicsDevice.Clear(Color.White);
                wonDeclare.Rectangle = new Rectangle((GraphicsDevice.Viewport.Width - 480) / 2, (GraphicsDevice.Viewport.Height - 180) / 2, 480, 180);
                stopButton.Rectangle = new Rectangle(wonDeclare.Rectangle.X, wonDeclare.Rectangle.Y + wonDeclare.Rectangle.Height + 50, 32, 32);
                replayButton.Rectangle = new Rectangle(wonDeclare.Rectangle.X + (wonDeclare.Rectangle.Width - 32), wonDeclare.Rectangle.Y + wonDeclare.Rectangle.Height + 50, 32, 32);
                wonDeclare.Draw(spriteBatch);
                stopButton.Draw(spriteBatch);
                replayButton.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

    }

}
