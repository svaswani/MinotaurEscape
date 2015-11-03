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
	public Rectangle CurrentFrame
	{
		get;
		set;
	}

	/// <summary>
	/// The current direction of the thing being animated
	/// </summary>
	public int Direction
	{
		get;
		set;
	}

	/// <summary>
	/// The texture of this animation
	/// </summary>
	public Texture2D Texture
	{
		get;
		set;
	}

	/// <summary>
	/// If looping this animation
	/// </summary>
	private bool loop;

    /// <summary>
	/// The number of frames in this animation
	/// </summary>
	private int numFrames;

    /// <summary>
	/// The number of the current frame
	/// </summary>
	private int currentFrameNum;

    /// <summary>
	/// The amount of steps between frames 
	/// </summary>
	private int animationSpeed;

    /// <summary>
	/// The current steps of the animation
	/// </summary>
	private int steps;

    // Create an animation from the given texture and properties
    public Animation(Texture2D texture, int numFrames, int animationSpeed, bool loop)
    {
        // Store the given values
            Texture = texture;
            this.numFrames = numFrames;
            this.loop = loop;
            this.animationSpeed = animationSpeed;

        // Get the starting frame
            CurrentFrame = new Rectangle(0, 0, GameVariables.TileSize, GameVariables.TileSize);
    }

	/// <summary>
	/// Resets the current animation to the start.
	/// </summary>
	public void Reset()
	{
        currentFrameNum = 0;
        CurrentFrame = new Rectangle(0, 0, GameVariables.TileSize, GameVariables.TileSize);
	}

	/// <summary>
	/// Gets the frame of the next step
	/// </summary>
	public Rectangle NextFrame()
	{
        // Check if the animation is done and if it loops reset it
            if (currentFrameNum >= numFrames && loop)
                    Reset();
            else if(currentFrameNum < numFrames && ++steps % animationSpeed == 0)
                CurrentFrame = new Rectangle(++currentFrameNum * GameVariables.TileSize, Direction * GameVariables.TileSize, GameVariables.TileSize, GameVariables.TileSize);

        // Return the Current Frame
            return CurrentFrame;
	}

	/// <summary>
	/// If the animation is at the end. (Only if it doesn't loop)
	/// </summary>
	public bool End()
	{
        return currentFrameNum >= numFrames;
    }

}

