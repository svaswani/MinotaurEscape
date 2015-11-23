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
    /// If the animation is currently being preformed
    /// </summary>
    public bool Animating
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
	/// The position of this tile
	/// </summary>
	public Vector2 Position
	{
		get;
		set;
	}

    /// <summary>
    /// Creates an animated tile with the given position and current animation
    /// </summary>
    public AnimatedTile(Vector2 position, Animation animation)
    {
        Position = position;
        Animation = animation;
    }

    /// <summary>
    /// Draws the current frame of this tile to the given sprite batch
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
	{
        Rectangle drawRect = new Rectangle(Position.ToPoint(),new Point(GameVariables.CharacterSize));

        if (Animating)
            spriteBatch.Draw(Animation.Texture, drawRect, Animation.NextFrame(Direction), Color.White);
        else
            spriteBatch.Draw(Animation.Texture, drawRect, Animation.CurrentFrame(Direction), Color.White);
    }
}

