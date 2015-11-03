using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// An animation sequence used by tiles to create animations
/// </summary>
public class Animation
{
	/// <summary>
	/// The rectangle of the current frame of the animation
	/// </summary>
	public virtual Rectangle CurrentFrame
	{
		get;
		set;
	}

	/// <summary>
	/// The width of every frame
	/// </summary>
	private int frameWidth;

	/// <summary>
	/// The height of every frame
	/// </summary>
	private int frameHeight;

	/// <summary>
	/// The amount to step each update for the animation
	/// </summary>
	private int stepAmount;

	/// <summary>
	/// The current direction of the thing being animated
	/// </summary>
	public virtual int Direction
	{
		get;
		set;
	}

	/// <summary>
	/// The texture of this animation
	/// </summary>
	public virtual Texture2D Texture
	{
		get;
		set;
	}

	/// <summary>
	/// If looping this animation
	/// </summary>
	private bool loop;

	public virtual AnimatedTile AnimatedTile
	{
		get;
		set;
	}

	/// <summary>
	/// Resets the current animation to the start.
	/// </summary>
	public virtual void Reset()
	{
		throw new System.NotImplementedException();
	}

	/// <summary>
	/// Gets the frame of the next step
	/// </summary>
	public virtual Rectangle NextFrame()
	{
		throw new System.NotImplementedException();
	}

	/// <summary>
	/// If the animation is at the end. (Only if it doesn't loop)
	/// </summary>
	public virtual bool End()
	{
		throw new System.NotImplementedException();
	}

}

