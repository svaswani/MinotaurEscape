using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// A torch that lights up part of the maze
/// </summary>
public class Torch : AnimatedTile, Movable
{



    /// <summary>
    /// The animation of the torch idle
    /// </summary>
    public static Animation IdleAnimation
    {
        get;
        set;
    }

    /// <summary>
    /// Creates a torch with the given position
    /// </summary>
    public Torch(Vector2 position)
    {
        Position = position;
        Animation = IdleAnimation;
    }

    /// <summary>
    /// Moves the torch the given speed in the given dir (true = right + left, false = up + down)
    /// </summary>
    public virtual void Move(GameTime gameTime, int speed, bool dir)
	{
        // Change the torch's position according to the given speed and direction
            if (dir)
                Position += new Vector2((int)(speed * gameTime.ElapsedGameTime.TotalSeconds), 0);
            else
                Position += new Vector2(0, (int)(speed * gameTime.ElapsedGameTime.TotalSeconds));
    }

    /// <summary>
	/// Sets up the animations for the player
	/// </summary>
    public static void SetupAnimations()
    {
        IdleAnimation = new Animation(GameVariables.TorchTexture, 1, 1, true);
    }
}

