using System;
using Microsoft.Xna.Framework.Input;

public class CameraInputs
{
  private static readonly Lazy<CameraInputs> stateValue;
  public static CameraInputs Instance
  {
    get { return stateValue.Value; }
  }

  static CameraInputs()
  {
    stateValue = new Lazy<CameraInputs>(() => new CameraInputs(), true);
  }

  private CameraInputs()
  {
  }

  public Func<bool> MoveLeftOverride { get; set; }
  public bool IsMoveLeft()
  {
    if (MoveLeftOverride != null)
    {
      return MoveLeftOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.Left);
  }

  public Func<bool> MoveRightOverride { get; set; }
  public bool IsMoveRight()
  {
    if (MoveRightOverride != null)
    {
      return MoveRightOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.Right);
  }

  public Func<bool> MoveUpOverride { get; set; }
  public bool IsMoveUp()
  {
    if (MoveUpOverride != null)
    {
      return MoveUpOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.Up);
  }

  public Func<bool> MoveDownOverride { get; set; }
  public bool IsMoveDown()
  {
    if (MoveDownOverride != null)
    {
      return MoveDownOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.Down);
  }

  public Func<bool> MoveZoomInOverride { get; set; }
  public bool IsZoomIn()
  {
    if (MoveZoomInOverride != null)
    {
      return MoveZoomInOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.Q);
  }

  public Func<bool> ZoomOutOverride { get; set; }
  public bool IsZoomOut()
  {
    if (ZoomOutOverride != null)
    {
      return ZoomOutOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.E);
  }

  public Func<bool> CameraResetOverride { get; set; }
  public bool IsCameraReset()
  {
    if (CameraResetOverride != null)
    {
      return CameraResetOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.R);
  }

  public Func<bool> MoveLeftEndOverride { get; set; }
  public bool IsMoveLeftEnd()
  {
    if (MoveLeftEndOverride != null)
    {
      return MoveLeftEndOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.Home);
  }

  public Func<bool> MoveRightEndOverride { get; set; }
  public bool IsMoveRightEnd()
  {
    if (MoveRightEndOverride != null)
    {
      return MoveRightEndOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.End);
  }

  public Func<bool> MoveUpEndOverride { get; set; }
  public bool IsMoveUpEnd()
  {
    if (MoveUpEndOverride != null)
    {
      return MoveUpEndOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.PageUp);
  }

  public Func<bool> MoveDownEndOverride { get; set; }
  public bool IsMoveDownEnd()
  {
    if (MoveDownEndOverride != null)
    {
      return MoveDownEndOverride();
    }
    return Keyboard.GetState().IsKeyDown(Keys.PageDown);
  }
}