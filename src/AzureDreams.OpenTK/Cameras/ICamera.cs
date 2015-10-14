using System;
using OpenTK;

public interface ICamera
{
  float Zoom { get; }
  Vector2 Position { get; }
  Matrix4 Transform { get; }
  void Update(double elapsedSeconds);
}
