using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public sealed class StaticCamera : ICamera
{
  private float zoom;
  private Viewport viewport;

  public float Zoom
  {
    get { return this.zoom; }
    set
    {
      this.zoom = value;
      if (this.zoom < 0.1f)
      {
        this.zoom = 0.1f;
      }
    }
  }

  public float Rotation { get; set; }
  public Vector2 Position { get; set; }

  public Matrix Transform
  {
    get
    {
      return Matrix.CreateTranslation(new Vector3(-this.Position.X, -this.Position.Y, 0f)) *
             Matrix.CreateRotationZ(Rotation) *
             Matrix.CreateScale(new Vector3(this.Zoom, this.Zoom, 0f)) *
             Matrix.CreateTranslation(new Vector3((float)this.viewport.Width / 2f, (float)this.viewport.Height / 2f, 0f));
    }
  }

  public StaticCamera(Viewport viewport)
  {
    this.zoom = 1f;
    this.Rotation = 0f;
    this.Position = Vector2.Zero;
    this.viewport = viewport;
  }

  public void Update(GameTime gameTime)
  {
    CameraInputs inputs = CameraInputs.Instance;
    float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
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
    this.Move(movement);
  }

  public void Move(Vector2 amount)
  {
    this.Position += amount;
  }
}