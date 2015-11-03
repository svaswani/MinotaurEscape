using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
        /// The height of the mazw
        /// </summary>
        private int height;

        /// <summary>
        /// All the torches in the maze currently
        /// </summary>
        public virtual List<Torch> Torches
        {
            get;
            set;
        }

        /// <summary>
        /// The comrades in the maze
        /// </summary>
        public virtual List<Comrade> Comrades
        {
            get;
            set;
        }

        /// <summary>
        /// Gets if the given position can be occuipied by the player.
        /// </summary>
        public virtual void IsPassable(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Draws the visible parts of the maze.
        /// </summary>
        public virtual void Draw()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets if the given position is visible
        /// </summary>
        public virtual void IsVisible(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Moves the maze by the given speed
        /// </summary>
        public virtual void Move(GameTime gameTime, int speed)
	{
		throw new System.NotImplementedException();
	}

}
}
