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
    /// The texture of this animation
    /// </summary>
    public Texture2D Texture
	{
		get;
		set;
	}

    /// <summary>
    /// The size of the character in the sheet
    /// </summary>
    private int characterSize;

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
	private float animationSpeed;

    /// <summary>
	/// The current steps of the animation
	/// </summary>
	private float elapsedTime;

    // Create an animation from the given texture and properties
    public Animation(Texture2D texture, int characterSize, int numFrames, float animationSpeed, bool loop)
    {
        // Store the given values
            Texture = texture;
            this.characterSize = characterSize;
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
	public Rectangle NextFrame(int dir, GameTime time)
    {
        // Check if the animation is done and if it loops reset it
            if ((elapsedTime += time.ElapsedGameTime.Milliseconds) >= animationSpeed)
            {
                if (currentFrameNum >= numFrames - 1 && loop)
                    Reset();
                else if (currentFrameNum < numFrames)
                {
                    elapsedTime = 0;
                    currentFrameNum++;
                }
            }

        // Return the Current Frame
            return CurrentFrame(dir);
	}


    /// <summary>
    /// The rectangle of the current frame of the animation
    /// </summary>
    public Rectangle CurrentFrame(int dir)
    {
        return new Rectangle(currentFrameNum * characterSize, dir * characterSize, characterSize, characterSize);
    }

    /// <summary>
    /// If the animation is at the end. (Only if it doesn't loop)
    /// </summary>
    public bool End()
	{
        return currentFrameNum >= numFrames;
    }

}

