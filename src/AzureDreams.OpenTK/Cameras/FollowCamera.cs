using System;
using OpenTK;

public sealed class FollowCamera : ICamera
{
  private float viewportHeight;
  private float viewportWidth;
  private float zoom;

  public float Zoom
  {
    get { return zoom; }
    set
    {
      zoom = value;
      if (zoom < 0.1f)
      {
        zoom = 0.1f;
      }
    }
  }

  public float Rotation { get; set; }
  public Vector2 Position { get; set; }
  public Vector2 Focus { get; set; }
  public float MoveSpeed { get; set; }
  public Matrix4 Transform { get; private set; }
  public Vector2 Origin { get; private set; }
  public Vector2 ScreenCenter { get; private set; }

  public FollowCamera(GameWindow window)
  {
    viewportWidth = (float)window.Width;
    viewportHeight = (float)window.Height;
    ScreenCenter = new Vector2(viewportWidth / 2f, viewportHeight / 2f);
    Zoom = 1f;
    MoveSpeed = 10.25f;
    Rotation = 0f;
    Position = Vector2.Zero;
  }

  public void Update(double elapsedSeconds)
  {
    float time = (float)elapsedSeconds;

    CameraInputs inputs = CameraInputs.Instance;
    if (inputs.IsZoomIn())
    {
      Zoom += time;
    }
    if (inputs.IsZoomOut())
    {
      Zoom -= time;
    }
 
    Transform = 
      Matrix4.CreateTranslation(-Position.X, -Position.Y, 0f) *
      Matrix4.CreateRotationZ(Rotation) * 
      Matrix4.CreateTranslation(Origin.X, Origin.Y, 0f) *
      Matrix4.CreateScale(Zoom);

    Origin = ScreenCenter / Zoom;

    Vector2 position = Position;
    position.X = Position.X + (Focus.X - Position.X) * MoveSpeed * time;
    position.Y = Position.Y + (Focus.Y - Position.Y) * MoveSpeed * time;
    Position = position;
  }
}
