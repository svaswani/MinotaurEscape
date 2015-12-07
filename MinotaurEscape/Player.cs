using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The player that "moves", pickups, and places things.
/// </summary>
public class Player : AnimatedTile
{
    public int Lives { get; set; }// player lives
    public int Points { get; set; } // 1 point for each comrade the player rescues 

    /// <summary>
    /// The number of torches the player has
    /// </summary>
    public int Torches
	{
		get;
		set;
	}

	/// <summary>
	/// The number of comrades that have been rescued
	/// </summary>
	public int ComradesRescued
	{
		get;
		set;
	}

	/// <summary>
	/// The number of comrades picked up but not rescued yet
	/// </summary>
	public int ComradesHeld
	{
		get;
		set;
	}

	/// <summary>
	/// The animation of the player standing still
	/// </summary>
	public static Animation IdleAnimation
	{
		get;
		set;
	}

	/// <summary>
	/// The animation of the player moving
	/// </summary>
	public static Animation MovingAnimation
	{
		get;
		set;
	}

	/// <summary>
	/// The animation of the playerpicking up a comrade
	/// </summary>
	public static Animation PickupAnimation
	{
		get;
		set;
	}

    /// <summary>
    /// Creates a player with the given position
    /// </summary>
    public Player(int lives, Vector2 position) : base(position, MovingAnimation)
    {
        Lives = lives;
    }


    /// <summary>
	/// Sets up the animations for the player
	/// </summary>
    public static void SetupAnimations()
    {
        MovingAnimation = new Animation(GameVariables.PlayerMovingTexture, 48, 4, 250, true);
        IdleAnimation = new Animation(GameVariables.PlayerIdleTexture, 48, 1, 1, true);
    }
}

