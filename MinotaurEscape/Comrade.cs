using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The comrades that can be picked up by the player
/// </summary>
public class Comrade : AnimatedTile, Movable
{
	/// <summary>
	/// The animation of the comrade Idle
	/// </summary>
	public static Animation IdleAnimation
	{
		get;
		set;
	}

	/// <summary>
	/// The animation of the comrade being picked up
	/// </summary>
	public static Animation PickupAnimation
	{
		get;
		set;
	}

    /// <summary>
    /// Creates a comrade with the given position
    /// </summary>
    public Comrade(Vector2 position) : base(position, IdleAnimation)
    {}

    /// <summary>
    /// Moves the comrade the given speed in the given dir (true = right + left, false = up + down)
    /// </summary>
    public void Move(GameTime gameTime, int speed, bool dir)
	{
        // Change the comrade's position according to the given speed and direction
            if (dir)
                Position += new Vector2((int)(speed * gameTime.ElapsedGameTime.TotalSeconds), 0);
            else
                Position += new Vector2(0, (int)(speed * gameTime.ElapsedGameTime.TotalSeconds));
    }

    /// <summary>
    /// Sets up the animations for this comrade
    /// </summary>
    public static void SetupAnimations()
    {
        IdleAnimation = new Animation(GameVariables.ComradeIdleTexture, 1, 1, true);
    }

}

