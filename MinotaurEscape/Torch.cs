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
	/// Moves the torch the given speed
	/// </summary>
	public virtual void Move(GameTime gameTime, int speed)
	{
		throw new System.NotImplementedException();
	}

}

