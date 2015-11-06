using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The minotaur that trys to kill the player
/// </summary>
public class Minotaur : AnimatedTile, Movable
{

    /// <summary>
    /// The animation of the minotaur moving
    /// </summary>
    public static Animation MovingAnimation
    {
        get;
        set;
    }

    /// <summary>
    /// Creates a minotaur with the given position
    /// </summary>
    public Minotaur(Vector2 position) : base(position, MovingAnimation)
    { }

    /// <summary>
    /// Move the minotuar by the given speed in the given directional axis (true = left + right, false = up + down)
    /// </summary>
    public virtual void Move(GameTime gameTime, int speed, bool dir)
	{
        // Change the comrade's position according to the given speed and direction
            if (dir)
                Position += new Vector2((int)(speed * gameTime.ElapsedGameTime.TotalSeconds), 0);
            else
                Position += new Vector2(0, (int)(speed * gameTime.ElapsedGameTime.TotalSeconds));
    }

	/// <summary>
	/// Sets up the animations for the minotaur
	/// </summary>
	public static void SetupAnimations()
	{
        MovingAnimation = new Animation(GameVariables.MinotuarMovingTexture, 1, 1, true);
    }

}

