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
    /// If this animation is currently being preformed
    /// </summary>
    public bool Animating
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

        // Set the start to 0
            currentFrameNum = 0;
    }

	/// <summary>
	/// Resets the current animation to the start.
	/// </summary>
	public void Reset()
	{
        currentFrameNum = 0;
	}

	/// <summary>
	/// Gets the frame of the next step
	/// </summary>
	public Rectangle NextFrame(int dir)
    {
        // Check if the animation is done and if it loops reset it
            if (currentFrameNum >= numFrames-1 && loop)
                Reset();
            else if(currentFrameNum < numFrames && ++steps % animationSpeed == 0)
                currentFrameNum++;

        // Return the Current Frame
            return CurrentFrame(dir);
	}


    /// <summary>
    /// The rectangle of the current frame of the animation
    /// </summary>
    public Rectangle CurrentFrame(int dir)
    {
        return new Rectangle(currentFrameNum * GameVariables.TileSize, dir * GameVariables.TileSize, GameVariables.TileSize, GameVariables.TileSize);
    }

    /// <summary>
    /// If the animation is at the end. (Only if it doesn't loop)
    /// </summary>
    public bool End()
	{
        return currentFrameNum >= numFrames;
    }

}

