using System.Diagnostics;

namespace Voronoi2
{
  [DebuggerDisplay("{ToDebuggerString()}")]
  public class Edge// : IEquatable<Edge>
  {
    public const double EqualityTolerance = 1E-10;

    public readonly double A;
    public readonly double B;
    public readonly double C;
    public readonly int EdgeIndex;
    public Site EndPoint0;
    public Site EndPoint1;
    public readonly Site[] Region;
    public Site Region0;
    public Site Region1;

    public Edge(double a, double b, double c, int edgeIndex)
    {
      A = a;
      B = b;
      C = c;
      EdgeIndex = edgeIndex;
      Region = new Site[2];
    }

    public string ToDebuggerString() => $"{EdgeIndex}: A={A:G3} B={B:G3} C={C:G3}";
  }
}