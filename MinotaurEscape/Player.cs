using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The player that "moves", pickups, and places things.
/// </summary>
public class Player : AnimatedTile
{
    public int lives = 3; // player lives
    public int points; // 1 point for each comrade the player rescues 

    public int Lives
    {
        get;
        set;
    }
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
	public Animation IdleAnimation
	{
		get;
		set;
	}

	/// <summary>
	/// The animation of the player moving
	/// </summary>
	public Animation MovingAnimation
	{
		get;
		set;
	}

	/// <summary>
	/// The animation of the playerpicking up a comrade
	/// </summary>
	public Animation PickupAnimation
	{
		get;
		set;
	}

    /// <summary>
	/// Sets up the animations for the player
	/// </summary>
    public override void SetupAnimations()
    {
        IdleAnimation = new Animation(GameVariables.PlayerIdleTexture, 1, 1, true);
    }

    // Method to decrement lives if player collides with Minotaur
    public int DecrementLives()
    {
        lives--;
        return lives;
    }

    // Method to increment points if player collides with 
    public int IncrementPoint()
    {
        points++;
        return points;
    }
}

