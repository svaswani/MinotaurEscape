using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        MainMenu menu;

        // atributes
        Texture2D image;
        bool[] wasd = { false, false, false, false };
        string[] wasdStr = { "W", "A", "S", "D" };
        Maze maze;
        Player player;

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
            // TODO: Add your initialization logic here
            menu = new MainMenu();
            menu.Show();

            /* Temporaily ask the player for a maze to load if maze doesn't exist */

            // Create a stream for loading the file
                Stream loadStream;

            // Create a dialog for asking for save location
                System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Filter = "maze files (*.maz)|*.maz";

            // Ask the user for a file to load
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && (loadStream = dialog.OpenFile()) != null)
                    maze = new Maze(loadStream, GraphicsDevice.Viewport);
                else
                    Exit();

            // Create the player
                player = new Player();
                player.Animating = false;
                
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

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            // Draw the maze
                maze.Draw(spriteBatch);

            // Place the player at the center of the screen and draw him
                player.Rectangle = new Rectangle((GraphicsDevice.Viewport.Width - GameVariables.TileSize) / 2, (GraphicsDevice.Viewport.Height - GameVariables.TileSize) / 2, GameVariables.TileSize, GameVariables.TileSize);
                player.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
