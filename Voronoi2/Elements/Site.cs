using System.Diagnostics;

namespace Voronoi2
{
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
}