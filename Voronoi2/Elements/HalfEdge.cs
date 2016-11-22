namespace Voronoi2
{
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
}