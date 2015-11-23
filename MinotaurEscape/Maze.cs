using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinotaurEscape
{
    /// <summary>
    /// The maze with all floor, wall, torch, and comrade tiles
    /// </summary>
    class Maze : Movable
    {

        /// <summary>
        /// Random used to get random cells
        /// </summary>
        private Random rand = new Random();

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
        /// The comrades in the maze
        /// </summary>
        private List<Comrade> comrades;


        /// <summary>
        /// The minotaurs in the maze
        /// </summary>
        private List<Minotaur> minotaurs;

        /// <summary>
        /// The tiles rendered in the last draw call
        /// </summary>
        private List<Vector2> tilesRendered;

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
                Vector2 entrance = new Vector2(reader.ReadInt32(), reader.ReadInt32());
                position = new Vector2(-entrance.X * GameVariables.TileSize - GameVariables.TileSize / 2 + viewport.Width/2, -entrance.Y * GameVariables.TileSize - GameVariables.TileSize / 2 + viewport.Height/2);
            
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

            // Initalize the movables, torch, minotuars, and comrade lists
                Moveables = new List<Movable>();
                torches = new List<Torch>();
                comrades = new List<Comrade>();
                minotaurs = new List<Minotaur>();

            // Create the comrades in the maze
                for (int i = 0; i < width * height / GameVariables.ComradeRate; i++)
                {
                    Comrade comrade = new Comrade(getRandomCell(entrance));
                    comrades.Add(comrade);
                    Moveables.Add(comrade);
                }

            // Create the minotaurs in the maze
                for (int i = 0; i < width * height / GameVariables.MinotuarRate; i++)
                {
                    Minotaur minotaur = new Minotaur(getRandomCell(entrance));
                    minotaurs.Add(minotaur);
                    Moveables.Add(minotaur);
                }
        }

        /// <summary>
        /// Gets if the given tile is intersecting with a wall.
        /// </summary>
        public bool IsInWall(AnimatedTile tile)
        {
            Vector2 relativePosition = new Vector2(tile.Position.X - position.X, tile.Position.Y - position.Y);
            return !tiles[(int)(relativePosition.X+GameVariables.CharacterSize/2) / GameVariables.TileSize][(int)(relativePosition.Y + GameVariables.CharacterSize / 2) / GameVariables.TileSize];
        }

        /// <summary>
        /// Gets if the given tile is intersecting with the exit
        /// </summary>
        public bool IsInExit(AnimatedTile tile)
        {
            return new Rectangle(tile.Position.ToPoint(), new Point(GameVariables.TileSize)).Intersects(new Rectangle((exit+position).ToPoint(), new Point(GameVariables.TileSize)));
        }

        /// <summary>
        /// Gets the comrade intersecting with the given tile (returns null if none)
        /// </summary>
        public Comrade IntersectingComrade(AnimatedTile tile)
        {
            Comrade[] comrade = comrades.Where(c => new Rectangle(tile.Position.ToPoint(), new Point(GameVariables.CharacterSize)).Intersects(new Rectangle(c.Position.ToPoint(), new Point(GameVariables.CharacterSize)))).ToArray();
            return comrade.Length > 0 ? comrade[0] : null;
        }

        /// <summary>
        /// Gets the minotuar intersecting with the given tile (returns null if none)
        /// </summary>
        public Minotaur IntersectingMinotuar(AnimatedTile tile)
        {
            Minotaur[] minotaur = minotaurs.Where(m => new Rectangle(tile.Position.ToPoint(), new Point(GameVariables.CharacterSize)).Intersects(new Rectangle(m.Position.ToPoint(), new Point(GameVariables.CharacterSize)))).ToArray();
            return minotaur.Length > 0 ? minotaur[0] : null;
        }

        /// <summary>
        /// Draws the visible parts of the maze.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the tiles around the center of the vieport first
                tilesRendered = getAndDrawTiles(spriteBatch, new Vector2(spriteBatch.GraphicsDevice.Viewport.Width/2, spriteBatch.GraphicsDevice.Viewport.Height/2));

            // Draw the tiles around every torch in the maze
                foreach (Torch torch in torches)
                    tilesRendered.AddRange(getAndDrawTiles(spriteBatch, torch.Position));

            // Draw each torch in the maze now
                foreach (Torch torch in torches)
                    torch.Draw(spriteBatch);

            // Draw the minotaurs, comrades, and exit now
                foreach (Comrade comrade in comrades)
                    comrade.Draw(spriteBatch);
                foreach (Minotaur minotaur in minotaurs)
                     minotaur.Draw(spriteBatch);
                spriteBatch.Draw(GameVariables.ExitTexture, position + exit);

            // Draw a "cover" on the screen where tiles where not rendered to hide the minotuars and comrades
                Vector2 topLeft = new Vector2((int)-position.X / GameVariables.TileSize, (int)-position.Y / GameVariables.TileSize);
                for (int x = -1; x < spriteBatch.GraphicsDevice.Viewport.Width / GameVariables.TileSize+1; x++)
                    for (int y = -1; y < spriteBatch.GraphicsDevice.Viewport.Height / GameVariables.TileSize+1; y++)
                        if (!tilesRendered.Contains(new Vector2(topLeft.X+x, topLeft.Y + y)))
                            spriteBatch.Draw(GameVariables.SoildBlackTexture, new Rectangle((position+(topLeft+new Vector2(x, y)) * GameVariables.TileSize).ToPoint(), new Point(GameVariables.TileSize)), Color.White);
            
        }

        /// <summary>
        /// Draws the minimap of the maze (with the given radius and position)
        /// </summary>
        public void DrawMinimap(SpriteBatch spriteBatch, int radius, Vector2 realPosition, Vector2 mapCenter)
        {
            // Get the position in relation to the maze
                Vector2 centerPosition = (mapCenter - position) / GameVariables.TileSize-new Vector2(radius);

            // Loop through every tile to draw
                for (int x = 0; x < radius*2; x++)
                {
                    for (int y = 0; y < radius*2; y++)
                    {

                            // Draw the current tile
                            int drawX = (int)centerPosition.X + x, drawY = (int)centerPosition.Y + y;
                            if (drawX < 0 || drawY < 0 || drawX >= width || drawY >= height || !tilesRendered.Contains(new Vector2(drawX, drawY)))
                                continue;
                            if (tiles[drawX][drawY])
                                spriteBatch.Draw(GameVariables.FloorTexture, new Rectangle((realPosition + new Vector2(x * GameVariables.minimapSize, y * GameVariables.minimapSize)).ToPoint(), new Point(GameVariables.minimapSize)), Color.White);
                            else
                                spriteBatch.Draw(getWallImage(drawX, drawY), new Rectangle((realPosition + new Vector2(x * GameVariables.minimapSize, y * GameVariables.minimapSize)).ToPoint(), new Point(GameVariables.minimapSize)), Color.White);
                    }
                }
        }

        // Gets and draws the tiles around the given position
        private List<Vector2> getAndDrawTiles(SpriteBatch spriteBatch, Vector2 center)
        {
            // Create List for holding the tiles
                List<Vector2> tilesRendered = new List<Vector2>();

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
                                    spriteBatch.Draw(GameVariables.FloorTexture, new Rectangle((position + new Vector2(drawX * GameVariables.TileSize, drawY * GameVariables.TileSize)).ToPoint(), new Point(GameVariables.TileSize)), Color.White);
                                else
                                    spriteBatch.Draw(getWallImage(drawX, drawY), new Rectangle((position + new Vector2(drawX * GameVariables.TileSize, drawY * GameVariables.TileSize)).ToPoint(), new Point(GameVariables.TileSize)), Color.White);
                            

                                // Add the tile to the list
                                    tilesRendered.Add(new Vector2(drawX, drawY));
                        }
                    }
                }

            // Return the tiles rendered
                return tilesRendered;
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


        /// <summary>
        /// Gets a the position of a random cell in the maze 
        /// </summary>
        public Vector2 getRandomCell(Vector2 entrance)
        {
            // Get the collection of cells
                List<Vector2> cells = tiles.SelectMany((col, x) => col.Select((tile, y) => tile ? new Vector2(x, y) : new Vector2(-1, -1))).Where(cell => !cell.Equals(new Vector2(-1, -1))).ToList();

            // Remove the exit, all the comrades and minotaurs, and the start
                cells.Remove(exit);
                cells.Remove(entrance);
                foreach (Comrade comrade in comrades)
                {
                    Vector2 tilePos = (comrade.Position - position) / GameVariables.TileSize;
                    tilePos = new Vector2((int)tilePos.X, (int)tilePos.Y);
                    cells.Remove(tilePos);
                }
                foreach (Minotaur minotaur in minotaurs)
                {
                    Vector2 tilePos = (minotaur.Position - position) / GameVariables.TileSize;
                    tilePos = new Vector2((int)tilePos.X, (int)tilePos.Y);
                    cells.Remove(tilePos);
                }

            // Pick a random cell, convert to screen location, and return it
                return cells[rand.Next(cells.Count)] * GameVariables.TileSize+position;

        }

        /// <summary>
        /// Removes the given comrade from the maze
        /// </summary>
        public void RemoveComrade(Comrade comrade)
        {
            Moveables.Remove(comrade);
            comrades.Remove(comrade);
        }

        /// <summary>
        /// Moves the minotuars randomly in the maze
        /// </summary>
        public void MoveMinotuars(GameTime gameTime)
        {
            foreach (Minotaur minotaur in minotaurs)
            {
                // Move the minotaur forward
                    if(minotaur.Direction==0)
                        minotaur.Move(gameTime, -GameVariables.MinotuarSpeed, false);
                    else if(minotaur.Direction==1)
                        minotaur.Move(gameTime, -GameVariables.MinotuarSpeed, true);
                    else if (minotaur.Direction == 2)
                        minotaur.Move(gameTime, GameVariables.MinotuarSpeed, true);
                    else if (minotaur.Direction == 3)
                        minotaur.Move(gameTime, GameVariables.MinotuarSpeed, false);

                // If the minotuar hit a wall move back and pick a new direction
                    if (IsInWall(minotaur))
                    {
                        if (minotaur.Direction == 0)
                            minotaur.Move(gameTime, GameVariables.MinotuarSpeed, false);
                        else if (minotaur.Direction == 1)
                            minotaur.Move(gameTime, GameVariables.MinotuarSpeed, true);
                        else if (minotaur.Direction == 2)
                            minotaur.Move(gameTime, -GameVariables.MinotuarSpeed, true);
                        else if (minotaur.Direction == 3)
                            minotaur.Move(gameTime, -GameVariables.MinotuarSpeed, false);
                        minotaur.Direction = rand.Next(4);
                    }
                    else
                    {
                        // Check if the minotaur has hit a comrade and remove it if it has
                            Comrade comrade = IntersectingComrade(minotaur);
                            if (comrade != null)
                                RemoveComrade(comrade);
                    }
                
            }
        }
    }
}
