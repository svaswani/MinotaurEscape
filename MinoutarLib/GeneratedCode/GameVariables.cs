﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameVariables
{
	/// <summary>
	/// The texture of the floor
	/// </summary>
	public static Texture2D FloorTexture;

	/// <summary>
	/// The texture of the wall
	/// </summary>
	public static Dictonary<string, Texture2D> WallTextures;

	/// <summary>
	/// The texture of the torch
	/// </summary>
	public static Texture2D TorchTexture;

	/// <summary>
	/// The texture of the player being Idle
	/// </summary>
	public static Texture2D PlayerIdleTexture;

	/// <summary>
	/// The texture of the minotuar
	/// </summary>
	public static Texture2D MinotuarTexture;

	/// <summary>
	/// The texture of the comrade being Idle
	/// </summary>
	public static Texture2D ComradeIdleTexture;

	/// <summary>
	/// The radius of the torch's light
	/// </summary>
	public static int TorchLightRadius;

	/// <summary>
	/// The basic font of the game
	/// </summary>
	public static SpriteFont BasicFont;

	/// <summary>
	/// The speed of the player
	/// </summary>
	public static int PlayerSpeed;

	/// <summary>
	/// The speed of the minotuar's movement
	/// </summary>
	public static int MinotuarSpeed;

	/// <summary>
	/// Texture of the comrade being picked up
	/// </summary>
	public static Texture2D ComradePickupTexture;

	/// <summary>
	/// The texture of the player moving
	/// </summary>
	public static Texture2D PlayerMovingTexture;

	/// <summary>
	/// The texture of the player picking up a comrade
	/// </summary>
	public static Texture2D PlayerPickupTexture;

	/// <summary>
	/// Loads the textures of the game
	/// </summary>
	public virtual void LoadTextures()
	{
		throw new System.NotImplementedException();
	}

}

