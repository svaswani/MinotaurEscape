﻿using Microsoft.Xna.Framework;
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

	/// <summary>
	/// Moves the comrade the given speed
	/// </summary>
	public virtual void Move(GameTime gameTime, int speed)
	{
		throw new System.NotImplementedException();
	}

}

