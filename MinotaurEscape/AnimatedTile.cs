using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// A tile that has an animation and direction
/// </summary>
public class AnimatedTile
{
	/// <summary>
	/// The animation of this tile
	/// </summary>
	public virtual Animation Animation
	{
		get;
		set;
	}

	/// <summary>
	/// If this tile is currently looping through an animation
	/// </summary>
	public virtual bool Animating
	{
		get;
		set;
	}

	/// <summary>
	/// The size and position of this tile
	/// </summary>
	public Rectangle Rectangle
	{
		get;
		set;
	}

	/// <summary>
	/// The Position of this tile on the X axis
	/// </summary>
	public int X
	{
		get;
		set;
	}

	/// <summary>
	/// The position of this tile on the Y axis
	/// </summary>
	public int Y
	{
		get;
		set;
	}

	/// <summary>
	/// Draws the current frame of this tile to the given sprite batch
	/// </summary>
	public void Draw(SpriteBatch spriteBatch)
	{
		throw new System.NotImplementedException();
	}

}

