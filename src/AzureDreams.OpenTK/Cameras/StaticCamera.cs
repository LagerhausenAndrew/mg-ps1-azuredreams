using System;
using OpenTK;

public sealed class StaticCamera : ICamera
{
  private float zoom;
  private GameWindow window;

  public float Zoom
  {
    get { return this.zoom; }
    set
    {
      this.zoom = value;
      if (this.zoom < 0.01f)
      {
        this.zoom = 0.01f;
      }
    }
  }

  public float Rotation { get; set; }
  public Vector2 Position { get; set; }

  public Matrix4 Transform
  {
    get
    {
      return Matrix4.CreateTranslation(new Vector3(-this.Position.X, -this.Position.Y, 0f)) *
             Matrix4.CreateRotationZ(Rotation) *
             Matrix4.CreateScale(new Vector3(this.Zoom, this.Zoom, 0f)) *
             Matrix4.CreateTranslation(new Vector3((float)this.window.Width / 2f, (float)this.window.Height / 2f, 0f));
    }
  }

  public StaticCamera(GameWindow window)
  {
    Reset();
    this.window = window;
  }

  private void Reset()
  {
    this.zoom = 1f;
    this.Rotation = 0f;
    this.Position = Vector2.Zero;
  }

  public void Update(double elapsedSeconds)
  {
    CameraInputs inputs = CameraInputs.Instance;
    float time = (float)elapsedSeconds;
    float speed = time * 500f;
    Vector2 movement = Vector2.Zero;
    if (inputs.IsMoveLeft())
    {
      movement.X -= speed;
    }
    if (inputs.IsMoveRight())
    {
      movement.X += speed;
    }
    if (inputs.IsMoveUp())
    {
      movement.Y -= speed;
    }
    if (inputs.IsMoveDown())
    {
      movement.Y += speed;
    }
    if (inputs.IsZoomIn())
    {
      this.Zoom += time;
    }
    if (inputs.IsZoomOut())
    {
      this.Zoom -= time;
    }
    if (inputs.IsCameraReset())
    {
      Reset();
      movement = Vector2.Zero;
    }
    this.Move(movement);
  }

  public void Move(Vector2 amount)
  {
    this.Position += amount;
  }
}