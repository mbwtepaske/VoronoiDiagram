using System.Diagnostics;

namespace Voronoi2
{
  [DebuggerDisplay("{ToDebuggerString(),nq}")]
  public class Edge
  {
    public const double EqualityTolerance = 1E-10;

    public readonly double A;
    public readonly double B;
    public readonly double C;
    public readonly int EdgeIndex;
    public Site EndPointLeftSide;
    public Site EndPointRightSide;
    public Site RegionLeftSide;
    public Site RegionRightSide;

    public Edge(double a, double b, double c, int edgeIndex)
    {
      A = a;
      B = b;
      C = c;
      EdgeIndex = edgeIndex;
    }

    public string ToDebuggerString() => $"{EdgeIndex}: A={A:F3} B={B:F3} C={C:F3}";
  }
}