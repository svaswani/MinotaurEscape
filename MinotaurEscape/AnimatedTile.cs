using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// A tile that has an animation and direction
/// </summary>
public abstract class AnimatedTile
{
	/// <summary>
	/// The animation of this tile
	/// </summary>
	public Animation Animation
	{
		get;
		set;
	}
    

    /// <summary>
    /// The current direction of the the animated tile
    /// </summary>
    public int Direction
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
	/// Draws the current frame of this tile to the given sprite batch
	/// </summary>
	public void Draw(SpriteBatch spriteBatch)
	{
        if (Animation.Animating)
            spriteBatch.Draw(Animation.Texture, Rectangle, Animation.NextFrame(Direction), Color.White);
        else
            spriteBatch.Draw(Animation.Texture, Rectangle, Animation.CurrentFrame(Direction), Color.White);
    }

    /// <summary>
	/// Sets up the animations for this tile
	/// </summary>
	public abstract void SetupAnimations();
}

