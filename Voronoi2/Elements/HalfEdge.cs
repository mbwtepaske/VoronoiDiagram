using System;
using System.Diagnostics;
using System.Text;

namespace Voronoi2
{
  [DebuggerDisplay("{ToDebuggerString(),nq}")]
  public class HalfEdge
  {
    public readonly Edge EdgeListEdge;
    public readonly int EdgeListSide;

    public bool IsDeleted;
    public HalfEdge Left;
    public HalfEdge Right;
    public HalfEdge Next;
    public Site Vertex;
    public double Ystar;
    
    public HalfEdge(Edge edge = default(Edge), int side = 0)
    {
      EdgeListEdge = edge;
      EdgeListSide = side;
    }

    public string ToDebuggerString()
    {
      var stringBuilder = new StringBuilder();

      if (IsDeleted)
      {
        stringBuilder.Append("[Deleted] ");
      }

      if (EdgeListEdge != null)
      {
        stringBuilder.Append($"Index: {EdgeListEdge.EdgeIndex}, Side: ");
        stringBuilder.Append(EdgeListSide == 0 ? "Left" : "Right");
      }

      if (Math.Abs(Ystar) > Voronoi.DefaultTolerance)
      {
        if (stringBuilder.Length > 0)
        {
          stringBuilder.Append(", ");
        }

        stringBuilder.Append(", Y*: " + Ystar);
      }

      return stringBuilder.ToString();
    }
  }
}