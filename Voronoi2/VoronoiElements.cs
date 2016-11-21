using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

// ReSharper disable ImpureMethodCallOnReadonlyValueField

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

  [DebuggerDisplay("{ToDebuggerString()}")]
  public class Site
  {
    public readonly Point Point;

    public int SiteIndex
    {
      get;
      set;
    }
    
    public Site(double x, double y, int siteIndex = 0) 
      : this(new Point(x, y), siteIndex)
    {
    }

    public Site(Point point, int siteIndex = 0)
    {
      Point = point;
      SiteIndex = siteIndex;
    }

    public string ToDebuggerString() => $"{SiteIndex}: {Point.ToDebuggerString()}";
  }

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

  public class HalfEdge
  {
    public readonly Edge EdgeListEdge;
    public readonly int EdgeListSide;

    public bool IsDeleted
    {
      get;
      set;
    }

    public HalfEdge Left
    {
      get;
      set;
    }

    public HalfEdge Right
    {
      get;
      set;
    }

    public HalfEdge Next
    {
      get;
      set;
    }

    public Site Vertex
    {
      get;
      set;
    }

    public double Ystar
    {
      get;
      set;
    }
    
    public HalfEdge(Edge edge = default(Edge), int side = 0)
    {
      EdgeListEdge = edge;
      EdgeListSide = side;
    }
  }

  public struct GraphEdge
  {
    public readonly Point P0;
    public readonly Point P1;
    public readonly int SiteIndex0;
    public readonly int SiteIndex1;

    public GraphEdge(Point p0, Point p1, int siteIndex0, int siteIndex1)
    {
      P0 = p0;
      P1 = p1;
      SiteIndex0 = siteIndex0;
      SiteIndex1 = siteIndex1;
    }
  }
}