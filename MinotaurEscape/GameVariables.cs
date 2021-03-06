﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameVariables
{
    /// <summary>
    /// The size of tiles in the files
    /// </summary>
    public static int TileSize = 64;

    /// <summary>
    /// The size of characters in the files
    /// </summary>
    public static int CharacterSize = (int)(TileSize*3/4.0);

    /// <summary>
    /// The radius of the minimap's display
    /// </summary>
    public static int minimapRadius = 10;


    /// <summary>
    /// The size of the minimap's tiles
    /// </summary>
    public static int minimapSize = TileSize/10;

    /// <summary>
    /// The radius of the torch's light
    /// </summary>
    public static int TorchLightRadius = 4;

    /// <summary>
    /// The speed of the player
    /// </summary>
    public static int PlayerSpeed = 200;

    /// <summary>
    /// The speed of the minotuar's movement
    /// </summary>
    public static int MinotuarSpeed = 200;

    /// <summary>
    /// The rate of the minotuar's in the maze (higher means less)
    /// </summary>
    public static int MinotuarRate = 100;

    /// <summary>
    /// The rate of the comrade's in the maze (higher means less)
    /// </summary>
    public static int ComradeRate = 100;

    /// <summary>
    /// The texture of the floor
    /// </summary>
    public static Texture2D FloorTexture;
    
    /// <summary>
    /// The texture of the exit
    /// </summary>
    public static Texture2D ExitTexture;

    /// <summary>
    /// The texture of the wall
    /// </summary>
    public static Dictionary<string, Texture2D> WallTextures;

    /// <summary>
    /// The texture of the torch
    /// </summary>
    public static Texture2D TorchTexture;

	/// <summary>
	/// The texture of the player being Idle
	/// </summary>
	public static Texture2D PlayerIdleTexture;

	/// <summary>
	/// The texture of the minotuar while moving
	/// </summary>
	public static Texture2D MinotuarMovingTexture;

	/// <summary>
	/// The texture of the comrade being Idle
	/// </summary>
	public static Texture2D ComradeIdleTexture;

    /// <summary>
    /// The basic font of the game
    /// </summary>
    public static SpriteFont BasicFont;

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
    /// The textures of the main menu
    /// </summary>
    public static Texture2D MenuPlayButtonTexture;
    public static Texture2D MenuStopButtonTexture;
    public static Texture2D MenuTitleTexture;
    public static Texture2D PauseDeclareTexture;
    public static Texture2D WonDeclareTexture;
    public static Texture2D ReplayButtonTexture;

    /// <summary>
    /// The texture of soild white shapes
    /// </summary>
    public static Texture2D SoildWhiteTexture;

    /// <summary>
    /// The texture of soild black shapes
    /// </summary>
    public static Texture2D SoildBlackTexture;

    /// <summary>
    /// Loads the textures of the game
    /// </summary>
    public static void LoadTextures(ContentManager content, GraphicsDevice graphicsDevice)
    {
        // Load the basic font and a soild textures
            BasicFont = content.Load<SpriteFont>("Fonts/basic");
            SoildWhiteTexture = new Texture2D(graphicsDevice, 1, 1);
            SoildWhiteTexture.SetData(new Color[] { Color.White });
            SoildBlackTexture = new Texture2D(graphicsDevice, 1, 1);
            SoildBlackTexture.SetData(new Color[] { Color.Black });

        // Load the floor, exit and walls of the maze
            FloorTexture = content.Load<Texture2D>("MazeTiles/floor");
            ExitTexture = content.Load<Texture2D>("MazeTiles/exit");
            string[] wallTypes = new string[] { "TTTT", "TTTF", "TTFT", "TTFF", "TFTT", "TFFT", "TFTF", "TFFF", "FFFF", "FTTT", "FTTF", "FTFT", "FFTT", "FFFT", "FFTF", "FTFF" };
            WallTextures = new Dictionary<string, Texture2D>();
            foreach (string wallType in wallTypes)
                WallTextures.Add(wallType, content.Load<Texture2D>("MazeTiles/wall-" + wallType));

        // Load the player's textures
            PlayerMovingTexture = content.Load<Texture2D>("Characters/player run spritesheet");
            PlayerIdleTexture = content.Load<Texture2D>("Characters/player idle spritesheet");

        // Load the torch's, comrade's, and minotuar's textures
            TorchTexture = content.Load<Texture2D>("Characters/torch");
            ComradeIdleTexture = content.Load<Texture2D>("Characters/comrade");
            MinotuarMovingTexture = content.Load<Texture2D>("Characters/minotaur");

        //Load menu textures
            MenuPlayButtonTexture = content.Load<Texture2D>("Menu/playButton");
            MenuStopButtonTexture = content.Load<Texture2D>("Menu/stopButton");
            MenuTitleTexture = content.Load<Texture2D>("Menu/menuTitle");
            ReplayButtonTexture = content.Load<Texture2D>("Menu/replayButton");
            WonDeclareTexture = content.Load<Texture2D>("Menu/wonDeclare");
            PauseDeclareTexture = content.Load<Texture2D>("Menu/pauseDeclare");
    }

}

