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
    public readonly Site[] EndPoint;
    public readonly Site[] Region;

    public Edge(double a, double b, double c, int edgeIndex)
    {
      A = a;
      B = b;
      C = c;
      EdgeIndex = edgeIndex;
      EndPoint = new Site[2];
      Region = new Site[2];
    }

    public string ToDebuggerString() => $"{EdgeIndex}: A={A:G3} B={B:G3} C={C:G3}";

    //public static bool operator ==(Edge left, Edge right) => left?.Equals(ref right) ?? right == null;

    //public static bool operator !=(Edge left, Edge right) => !left?.Equals(ref right) ?? right != null;

    //public bool Equals(Edge other) => Equals(ref other);

    //public bool Equals(ref Edge other) 
    //  => EdgeIndex == other.EdgeIndex
    //  && EndPoint[0] == other.EndPoint[0]
    //  && EndPoint[1] == other.EndPoint[1]
    //  && Region[0] == other.Region[0]
    //  && Region[1] == other.Region[1]
    //  && Voronoi.IsNearlyEqual(A, other.A)
    //  && Voronoi.IsNearlyEqual(B, other.B)
    //  && Voronoi.IsNearlyEqual(C, other.C);
  }
}