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

        /// <summary>
        /// the wall and floor tiles of the maze
        /// </summary>
        private bool[][] tiles;

        /// <summary>
        /// the width of the maze
        /// </summary>
        private int width;

        /// <summary>
        /// The height of the maze
        /// </summary>
        private int height;

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
        /// The torches in the maze
        /// </summary>
        private List<Torch> torches;

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

            // Initalize the movables and torch lists
                Moveables = new List<Movable>();
                torches = new List<Torch>();
        }

        /// <summary>
        /// Gets if the given tile is intersecting with a wall.
        /// </summary>
        public bool IsInWall(AnimatedTile tile)
        {
            Vector2 relativePosition = new Vector2(tile.Position.X - position.X, tile.Position.Y - position.Y);
            return !tiles[(int)(relativePosition.X+GameVariables.TileSize/2) / GameVariables.TileSize][(int)(relativePosition.Y + GameVariables.TileSize / 2) / GameVariables.TileSize];
        }

        /// <summary>
        /// Gets if the given tile is intersecting with the exit
        /// </summary>
        public bool IsInExit(AnimatedTile tile)
        {
            return new Rectangle(tile.Position.ToPoint(), new Point(GameVariables.TileSize)).Intersects(new Rectangle((exit+position).ToPoint(), new Point(GameVariables.TileSize)));
        }

        /// <summary>
        /// Draws the visible parts of the maze.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Viewport viewport)
        {
            // Draw the tiles around the center of the vieport first
                DrawAround(spriteBatch, new Vector2(viewport.Width/2, viewport.Height/2));

            // Draw the tiles around every torch in the maze
                foreach (Torch torch in torches)
                    DrawAround(spriteBatch, torch.Position);

            // Draw each torch in the maze now
                foreach (Torch torch in torches)
                    torch.Draw(spriteBatch);
        }

        // Draws the tiles around the given position (In relation to the screen)
        private void DrawAround(SpriteBatch spriteBatch, Vector2 center)
        {
            // Get the position in relation to the maze
                Vector2 centerPosition = (center - position) / GameVariables.TileSize;

            // Loop through every tile to draw
                for (int x = -GameVariables.TorchLightRadius; x < GameVariables.TorchLightRadius; x++)
                {
                    for (int y = -GameVariables.TorchLightRadius; y < GameVariables.TorchLightRadius; y++)
                    {

                        // Check if the current x and y will be in a sphere instead of a box
                            if (Math.Abs(x) + Math.Abs(y) < GameVariables.TorchLightRadius)
                            {

                                // Draw the current tile
                                    int drawX = (int)centerPosition.X + x, drawY = (int)centerPosition.Y + y;
                                    if (drawX < 0 || drawY < 0 || drawX >= width || drawY >= height)
                                        continue;
                                    if (tiles[drawX][drawY])
                                        spriteBatch.Draw(GameVariables.FloorTexture, position + new Vector2(drawX * GameVariables.TileSize, drawY * GameVariables.TileSize));
                                    else
                                        spriteBatch.Draw(getWallImage(drawX, drawY), position + new Vector2(drawX * GameVariables.TileSize, drawY * GameVariables.TileSize));

                                // Draw the exit if in sight
                                    if (new Vector2(x, drawY).Equals(exit))
                                        spriteBatch.Draw(GameVariables.ExitTexture, position + exit);
                            }
                    }
                }
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

        /// <summary>
        /// Moves the maze by the given speed in the given directional axis (true = left + right, false = up + down) but only if the given animatied tile will not be in a wall.
        /// </summary>
        public void AttemptMove(GameTime gameTime, int speed, bool dir, AnimatedTile tile)
        {
            // Attempt to move
                Move(gameTime, speed, dir);

            // Check if the given tile is in a wall and if he is undo move
                if (IsInWall(tile))
                    Move(gameTime, -speed, dir);
        }

        /// <summary>
        /// Adds a torch to the maze from the given position in the given direction
        /// </summary>
        public void AddTorch(Vector2 fromPosition, int dir)
        {
            // Get the position in relation to the maze
                Vector2 fromTilePosition = (fromPosition - position) / GameVariables.TileSize;
                fromTilePosition = new Vector2((int)fromTilePosition.X, (int)fromTilePosition.Y);

            // Check if wall at current position and if not move in the given direction
                if (tiles[(int)fromTilePosition.X][(int)fromTilePosition.Y])
                {
                    // Change the position depending on the direction
                        if (dir == 0)
                            fromTilePosition.Y--;
                        else if (dir == 1)
                            fromTilePosition.X--;
                        else if (dir == 2)
                            fromTilePosition.X++;
                        else if (dir == 3)
                            fromTilePosition.Y++;
                }

            // Create a torch with the position and add it to the torch list and movables list
                Torch torch = new Torch(fromTilePosition* GameVariables.TileSize+position);
                Moveables.Add(torch);
                torches.Add(torch);
        }

        /// <summary>
        /// Checks if a torch can be placed in the maze from the given position in the given direction
        /// </summary>
        public bool CanPlaceTorch(Vector2 fromPosition, int dir)
        {
            // Get the position in relation to the maze
                Vector2 fromTilePosition = (fromPosition - position) / GameVariables.TileSize;
                fromTilePosition = new Vector2((int)fromTilePosition.X, (int)fromTilePosition.Y);

            // Check if wall at current position and if not move in the given direction
                if (tiles[(int)fromTilePosition.X][(int)fromTilePosition.Y])
                {
                    // Change the position depending on the direction
                        if (dir == 0)
                            fromTilePosition.Y--;
                        else if (dir == 1)
                            fromTilePosition.X--;
                        else if (dir == 2)
                            fromTilePosition.X++;
                        else if (dir == 3)
                            fromTilePosition.Y++;
                }

            // Make sure there is a wall where placing torch
                if (tiles[(int)fromTilePosition.X][(int)fromTilePosition.Y])
                    return false;

            // Return if there is a torch with the position
                return torches.Where(torch => torch.Position.Equals(fromTilePosition * GameVariables.TileSize + position)).Count() <= 0;
        }
    }
}
