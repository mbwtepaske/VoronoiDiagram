using System.Diagnostics;
using System.Drawing;

namespace Voronoi2
{
  [DebuggerDisplay("{ToDebuggerString()}")]
  public struct Point
  {
    public readonly double X;
    public readonly double Y;

    public Point(double x, double y)
    {
      X = x;
      Y = y;
    }

    public string ToDebuggerString() => $"[{X:F3},{Y:F3}]";

    public static implicit operator PointF(Point point) => new PointF((float) point.X, (float) point.Y);
  }
}