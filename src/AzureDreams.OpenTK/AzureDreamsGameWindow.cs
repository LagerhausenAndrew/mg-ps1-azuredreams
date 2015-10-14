using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace AzureDreams.OpenGL
{
  public class AzureDreamsGameWindow : GameWindow
  {
    const int CellWidth = 64;
    const int CellHeight = 64;

    Generator generator;
    bool done = false;
    IEnumerator<bool> iter;
    double totalElapsedTime = 0;

    KeyboardState previousKeyboard;

    ICamera[] cameras;
    int currentCameraIndex;

    TimeSpan targetTime;

    public AzureDreamsGameWindow()
    {
      targetTime = TimeSpan.FromMilliseconds(100);

      generator = new Generator(16);
      ResetGenerator();

      // create the cameras
      cameras = new ICamera[2];
      cameras[0] = new StaticCamera(this);
      cameras[1] = new FollowCamera(this);

      // set the index
      currentCameraIndex = 0;
    }

    private void ResetGenerator()
    {
      iter = generator.Generate().GetEnumerator();
      done = false;
    }

    protected override void OnLoad(EventArgs e)
    {
      VSync = VSyncMode.On;
      Title = "AzureDreams";
      Width = 800;
      Height = 600;

      base.OnLoad(e);
    }

    protected override void OnResize(EventArgs e)
    {
      GL.Viewport(0, 0, Width, Height);
      base.OnResize(e);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      var currentKeyboard = OpenTK.Input.Keyboard.GetState();
      if (currentKeyboard[Key.Tab] && !previousKeyboard[Key.Tab])
      {
        ResetGenerator();
      }

      totalElapsedTime += e.Time;
      if (totalElapsedTime >= targetTime.TotalSeconds)
      {
        totalElapsedTime -= targetTime.TotalSeconds;
        if (!done)
        {
          done = !iter.MoveNext();
        }
      }

      cameras[currentCameraIndex].Update(e.Time);

      previousKeyboard = currentKeyboard;
      base.OnUpdateFrame(e);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      GL.ClearColor(Color4.CornflowerBlue);

      Graphics.Begin(Width, Height);

      var camera = cameras[currentCameraIndex];
      var transform = camera.Transform;
      GL.MultMatrix(ref transform);

      foreach (var cell in generator.Cells)
      {
        var color = Color4.Green;
        switch (cell.Type)
        {
          case CellType.Room: { color = Color4.Yellow; break; }
          case CellType.Door: { color = Color4.Red; break; }
          case CellType.Wall: { color = Colors.Lerp(Color4.Yellow, Color4.Black, 0.5f); break; }
        }

        Graphics.FillRectangle(color,
          cell.Column * CellWidth,
          cell.Row * CellHeight,
          CellWidth, CellHeight);
      }

      Graphics.End();

      SwapBuffers();
      base.OnRenderFrame(e);
    }
  }
}
