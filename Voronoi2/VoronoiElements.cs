using System;
using System.Collections.Generic;
using System.Drawing;

namespace Voronoi2
{
  public struct Point
  {
    public readonly double X;
    public readonly double Y;

    public Point(double x, double y)
    {
      X = x;
      Y = y;
    }

    public static implicit operator PointF(Point point) => new PointF((float) point.X, (float) point.Y);
  }

  public class Site
  {
    public Point Point
    {
      get;
      set;
    }

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
  }

  public class Edge
  {
    public double A
    {
      get;
      set;
    }

    public double B
    {
      get;
      set;
    }

    public double C
    {
      get;
      set;
    }

    public Site[] EndPoint
    {
      get;
      set;
    }

    public Site[] Region
    {
      get;
      set;
    }

    public int EdgeIndex
    {
      get;
      set;
    }

    public Edge()
    {
      EndPoint = new Site[2];
      Region = new Site[2];
    }
  }

  public class HalfEdge
  {
    public Edge EdgeListEdge;
    public int EdgeListSide;
    public bool IsDeleted;
    public HalfEdge Left;
    public HalfEdge Right;
    public HalfEdge Next;
    public Site Vertex;
    public double Ystar;

    public HalfEdge()
    {
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