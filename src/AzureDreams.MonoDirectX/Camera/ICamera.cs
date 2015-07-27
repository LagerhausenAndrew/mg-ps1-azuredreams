using System;
using Microsoft.Xna.Framework;

public interface ICamera
{
  float Zoom { get; }
  Vector2 Position { get; }
  Matrix Transform { get; }
  void Update(GameTime gameTime);
}
