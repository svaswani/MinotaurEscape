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
    /// Moves the torch the given speed in the given dir (true = right + left, false = up + down)
    /// </summary>
    public virtual void Move(GameTime gameTime, int speed, bool dir)
	{
		throw new System.NotImplementedException();
	}

    /// <summary>
    /// Sets up the animations for this torch
    /// </summary>
    public override void SetupAnimations()
    {
        throw new NotImplementedException();
    }
}

