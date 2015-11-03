using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface Movable 
{
	/// <summary>
	/// Move the given object the given speed in the given dir (true = right + left, false = up + down)
	/// </summary>
	void Move(GameTime gameTime, int speed, bool dir);

}

