using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MinotaurEscape
{
    /// <summary>
    /// The maze with all floor, wall, torch, and comrade tiles
    /// </summary>
    class Maze : Movable
    {
        Comrade comrade = new Comrade();

        /// <summary>
        /// the wall and floor tiles of the maze
        /// </summary>
        private bool[][] tiles;

        /// <summary>
        /// the width of the maze
        /// </summary>
        public static int width { get; set; }

        /// <summary>
        /// The height of the maze
        /// </summary>
        public static int height { get; set; }

        /// <summary>
        /// The position of the maze
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The position of the maze's exit
        /// </summary>
        private Vector2 exit;

        /// <summary>
        /// Everything that moves with the maze
        /// </summary>
        public List<Movable> Moveables
        {
            get;
        }

        /// <summary>
        /// Creates a maze object from the given stream
        /// </summary>
        public Maze(Stream mazeStream, Viewport viewport)
        {

            // load the maze's size
                BinaryReader reader = new BinaryReader(mazeStream);
                width = reader.ReadInt32();
                height = reader.ReadInt32();

            // load the entrance and set the position to it at the start
                position = new Vector2(-reader.ReadInt32() * GameVariables.TileSize - GameVariables.TileSize / 2 + viewport.Width/2, -reader.ReadInt32() * GameVariables.TileSize - GameVariables.TileSize / 2 + viewport.Height/2);

            // load the exit
                exit = new Vector2(reader.ReadInt32()*GameVariables.TileSize, reader.ReadInt32()*GameVariables.TileSize);

            // load the tile array
                tiles = new bool[width][];
                for (int x = 0; x < width; x++)
                {
                    tiles[x] = new bool[height];
                    for (int y = 0; y < height; y++)
                    {
                        tiles[x][y] = reader.ReadBoolean();
                    }
                }
                reader.Close();

            // Initalize the comrade and torch lists
                Moveables = new List<Movable>();
            Moveables.Add(comrade);
        }

        /// <summary>
        /// Gets if the given tile is intersecting with a wall.
        /// </summary>
        public bool IsInWall(AnimatedTile tile)
        {
            Vector2 relativePosition = new Vector2(tile.Rectangle.X - position.X, tile.Rectangle.Y - position.Y);
            return !tiles[(int)(relativePosition.X+GameVariables.TileSize/2) / GameVariables.TileSize][(int)(relativePosition.Y + GameVariables.TileSize / 2) / GameVariables.TileSize];
        }

        /// <summary>
        /// Gets if the given tile is intersecting with the exit
        /// </summary>
        public bool IsInExit(AnimatedTile tile)
        {
            return tile.Rectangle.Intersects(new Rectangle((exit+position).ToPoint(), new Point(GameVariables.TileSize)));
        }

        /// <summary>
        /// Draws the visible parts of the maze.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw each tile of the maze starting at the top left
                for(int x=0;x< width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        if (tiles[x][y])
                            spriteBatch.Draw(GameVariables.FloorTexture, position+new Vector2(x * GameVariables.TileSize, y * GameVariables.TileSize));
                        else
                            spriteBatch.Draw(getWallImage(x, y), position + new Vector2(x * GameVariables.TileSize, y * GameVariables.TileSize));
                    }
                }

            // Draw the exit
                spriteBatch.Draw(GameVariables.ExitTexture, position+exit);
        }

        // Gets the wall texture that should be in the given position
        private Texture2D getWallImage(int x, int y)
        {
            return GameVariables.WallTextures[(isWall(x - 1, y) ? "T" : "F") + (isWall(x, y - 1) ? "T" : "F") + (isWall(x, y + 1) ? "T" : "F") + (isWall(x + 1, y) ? "T" : "F")];
        }

        // Checks if the given tile is a wall
        private bool isWall(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height && !tiles[x][y];
        }

        /// <summary>
        /// Moves the maze by the given speed in the given directional axis (true = left + right, false = up + down)
        /// </summary>
        public void Move(GameTime gameTime, int speed, bool dir)
	    {
            // Change the maze's position according to the given speed and direction
                if (dir)
                    position.X += (int)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                else
                    position.Y += (int)(speed * gameTime.ElapsedGameTime.TotalSeconds);

            // Move everything else with the maze
                foreach (Movable moveable in Moveables)
                    moveable.Move(gameTime, speed, dir);
        }

    }
}
