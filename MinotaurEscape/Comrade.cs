using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinotaurEscape
{
    /// <summary>
    /// The comrades that can be picked up by the player
    /// </summary>
    public class Comrade : AnimatedTile, Movable
    {
        
        /// <summary>
        /// The animation of the comrade Idle
        /// </summary>
        public virtual Animation IdleAnimation
        {
            get;
            set;
        }

        /// <summary>
        /// The animation of the comrade being picked up
        /// </summary>
        public virtual Animation PickupAnimation
        {
            get;
            set;
        }
        public Vector2 position;
        
        public float posx { get; set; }
        public float posy { get; set; }
        /// <summary>
        /// Moves the comrade the given speed in the given dir (true = right + left, false = up + down)
        /// </summary>
        public virtual void Move(GameTime gameTime, int speed, bool dir)
        {

            // Change the maze's position according to the given speed and direction
            if (dir)
            {
                position.X += (int)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                //posx = position.X;
            }
            else
            {
                position.Y += (int)(speed * gameTime.ElapsedGameTime.TotalSeconds);
               // posy = position.Y;
            }

            Rectangle = new Rectangle((int) posx + (int) position.X +  (int)(speed * gameTime.ElapsedGameTime.TotalSeconds),(int) posy + (int)position.Y +(int)(speed * gameTime.ElapsedGameTime.TotalSeconds), GameVariables.TileSize, GameVariables.TileSize);
            //Rectangle = new Rectangle(Rectangle.X + (int)(speed * gameTime.ElapsedGameTime.TotalSeconds), Rectangle.Y, Rectangle.Width, Rectangle.Height);
            //Rectangle = new Rectangle(26 + (int)(speed * gameTime.ElapsedGameTime.TotalSeconds), (int)position.Y + (int)(speed * gameTime.ElapsedGameTime.TotalSeconds), GameVariables.TileSize, GameVariables.TileSize);
        }

        /// <summary>
        /// Sets up the animations for this comrade
        /// </summary>
        public override void SetupAnimations()
        {
            IdleAnimation = new Animation(GameVariables.ComradeIdleTexture, 1, 1, true);
        }

        public void random()
        {
            Random ran = new Random();
            posx = ran.Next(0, 55);

            //posx = position.X;
            //position = new Vector2(posx, posy);
            posy = ran.Next(0, 55);
        }
        





    }
}

