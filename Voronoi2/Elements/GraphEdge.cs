using System;
using System.Collections.Generic;

// ReSharper disable ImpureMethodCallOnReadonlyValueField

namespace Voronoi2
{
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