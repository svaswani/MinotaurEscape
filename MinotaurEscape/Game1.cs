using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

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
        bool[] wasd = { false, false, false, false };
        string[] wasdStr = { "W", "A", "S", "D" };
        Maze maze;
        Player player;
        MenuButton playButton;
        MenuButton stopButton;
        MenuButton menuTitle;
        public enum GameState
        {
            MainMenu,
            Play
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

            // Create the player
                player = new Player();
                player.Animating = false;

            //Set up the menu
            playButton = new MenuButton(GameVariables.MenuPlayButtonTexture);
            stopButton = new MenuButton(GameVariables.MenuStopButtonTexture);
            menuTitle = new MenuButton(GameVariables.MenuTitleTexture);

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
                GameVariables.LoadTextures(Content);

            // Setup all the animations of the game
                player.SetupAnimations();
                player.Animation = player.IdleAnimation; // Do this here for now, will be moved later to a more apporiate place

            //Menu Textures
            playButton.ButtonGraphic = GameVariables.MenuPlayButtonTexture;
            stopButton.ButtonGraphic = GameVariables.MenuStopButtonTexture;
            menuTitle.ButtonGraphic = GameVariables.MenuTitleTexture;
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (stateGame == GameState.MainMenu )
            {
                IsMouseVisible = true;
                if (playButton.IsMouseInside() == true && playButton.stateMouse.LeftButton == ButtonState.Pressed)
                {
                    stateGame = GameState.Play;

                    /* Temporaily ask the player for a maze to load if maze doesn't exist */

                    // Create a stream for loading the file
                        Stream loadStream;

                    // Create a dialog for asking for save location
                        System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
                        dialog.Filter = "maze files (*.maz)|*.maz";
                        dialog.InitialDirectory = Directory.GetCurrentDirectory()+"\\DefaultMazes\\";

                    // Ask the user for a file to load
                        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && (loadStream = dialog.OpenFile()) != null)
                            maze = new Maze(loadStream, GraphicsDevice.Viewport);
                        else
                            Exit();

                }
                if (stopButton.IsMouseInside() == true && playButton.stateMouse.LeftButton == ButtonState.Pressed)
                {
                    Exit();
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && stateGame == GameState.Play)
            {
                stateGame = GameState.MainMenu;
            }

            // TODO: Add your update logic here
            ProcessInput(gameTime);

            base.Update(gameTime);
        }

        public void ProcessInput(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();
            if (kbState.IsKeyDown(Keys.W))
            {
                wasd[0] = true;
                maze.Move(gameTime, 100, false);
            }
            else wasd[0] = false;

            if (kbState.IsKeyDown(Keys.A))
            {
                wasd[1] = true;
                maze.Move(gameTime, 100, true);

            }
            else wasd[1] = false;


            if (kbState.IsKeyDown(Keys.S))
            {
                wasd[2] = true;
                maze.Move(gameTime, -100, false);
            }
            else wasd[2] = false;
            if (kbState.IsKeyDown(Keys.D))
            {
                wasd[3] = true;
                maze.Move(gameTime, -100, true);
            }
            else wasd[3] = false;
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            if (stateGame == GameState.Play)
            {
                // TODO: Add your drawing code here

                // Draw the maze
                maze.Draw(spriteBatch);

                // Place the player at the center of the screen and draw him
                player.Rectangle = new Rectangle((GraphicsDevice.Viewport.Width - GameVariables.TileSize) / 2, (GraphicsDevice.Viewport.Height - GameVariables.TileSize) / 2, GameVariables.TileSize, GameVariables.TileSize);
                player.Draw(spriteBatch);

            }

            if (stateGame == GameState.MainMenu)
            {
                playButton.Rectangle = new Rectangle((GraphicsDevice.Viewport.Width - GameVariables.TileSize) / 2, (GraphicsDevice.Viewport.Height - GameVariables.TileSize) / 2, GameVariables.TileSize, GameVariables.TileSize);
                stopButton.Rectangle = new Rectangle((GraphicsDevice.Viewport.Width - GameVariables.TileSize) / 2, (playButton.Rectangle.Y + playButton.Rectangle.Height + 50), GameVariables.TileSize, GameVariables.TileSize);
                menuTitle.Rectangle = new Rectangle((GraphicsDevice.Viewport.Width - 490) / 2, 20, 490, 78);
                stopButton.Draw(spriteBatch);
                playButton.Draw(spriteBatch);
                menuTitle.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }

}
