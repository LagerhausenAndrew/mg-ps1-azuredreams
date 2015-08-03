using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AzureDreams.MonoDirectX
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class AzureDreamsGame : Game
  {
    const int Width = 32;
    const int Height = 32;

    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;
    KeyboardState lastKeyboard;

    ICamera[] cameras;
    int currentCameraIndex;

    Generator generator;

    bool done = false;
    IEnumerator<bool> iter;
    double totalElapsedTime = 0;

    public AzureDreamsGame()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
    }

    private void ResetGenerator()
    {
      iter = generator.Generate().GetEnumerator();
      done = false;
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      generator = new Generator(10);
      ResetGenerator();
      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);

      // create the cameras
      cameras = new ICamera[2];
      cameras[0] = new StaticCamera(GraphicsDevice.Viewport);
      cameras[1] = new FollowCamera(GraphicsDevice.Viewport);

      // set the index
      currentCameraIndex = 0;
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      KeyboardState currentKeyboard = Keyboard.GetState();
      if (currentKeyboard.IsKeyDown(Keys.Tab) && lastKeyboard.IsKeyUp(Keys.Tab))
      {
        ResetGenerator();
      }

      totalElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
      if (totalElapsedTime >= 100)
      {
        totalElapsedTime -= 100;
        if (!done)
        {
          done = !iter.MoveNext();
        }
      }

      cameras[currentCameraIndex].Update(gameTime);

      lastKeyboard = currentKeyboard;
      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);
      spriteBatch.Begin(cameras[currentCameraIndex].Transform);

      foreach (var cell in generator.Cells)
      {
        var color = Color.Green;
        switch (cell.Type)
        {
          case CellType.Room: { color = Color.Yellow; break; }
          case CellType.Door: { color = Color.Red; break; }
          case CellType.Wall: { color = Color.Lerp(Color.Yellow, Color.Black, 0.5f); break; }
        }

        spriteBatch.FillRectangle(cell.Column * Width, cell.Row * Height,
          Width, Height, color);
      }

      spriteBatch.End();
      base.Draw(gameTime);
    }
  }
}
