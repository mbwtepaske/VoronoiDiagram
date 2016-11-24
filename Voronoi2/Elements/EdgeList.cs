using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voronoi2
{
  public class EdgeList
  {
    public readonly Voronoi Voronoi;

    private HalfEdge[] _edgeListHash;
    private int _edgeListHashSize;
    private HalfEdge _edgeListLeftHalfEdge;
    private HalfEdge _edgeListRightHalfEdge;

    public EdgeList(Voronoi voronoi, int hashSize)
    {
      Voronoi = voronoi;
      _edgeListHashSize = 2 * hashSize;
      _edgeListHash = new HalfEdge[_edgeListHashSize];
      _edgeListLeftHalfEdge = new HalfEdge();
      _edgeListRightHalfEdge = new HalfEdge();
      _edgeListLeftHalfEdge.Left = null;
      _edgeListLeftHalfEdge.Right = _edgeListRightHalfEdge;
      _edgeListRightHalfEdge.Left = _edgeListLeftHalfEdge;
      _edgeListRightHalfEdge.Right = null;
      _edgeListHash[0] = _edgeListLeftHalfEdge;
      _edgeListHash[_edgeListHashSize - 1] = _edgeListRightHalfEdge;
    }


    private static void InsertIntoEdgeList(HalfEdge halfEdge, HalfEdge newHalfEdge)
    {
      newHalfEdge.Left = halfEdge;
      newHalfEdge.Right = halfEdge.Right;

      halfEdge.Right.Left = newHalfEdge;
      halfEdge.Right = newHalfEdge;
    }

    public void DeleteFromEdgeList(HalfEdge halfEdge)
    {
      halfEdge.Left.Right = halfEdge.Right;
      halfEdge.Right.Left = halfEdge.Left;
      halfEdge.IsDeleted = true;
    }

    // Get entry from hash table, pruning any deleted nodes
    private HalfEdge GetHashFromEdgeList(int b)
    {
      if (b < 0 || b >= _edgeListHashSize)
      {
        return null;
      }

      var halfEdge = _edgeListHash[b];

      if (halfEdge == null || !halfEdge.IsDeleted)
      {
        return halfEdge;
      }

      // Hashtable points to deleted half edge. Patch as necessary.
      _edgeListHash[b] = null;

      return null;
    }

    private HalfEdge LeftBoundaryEdgeList(Point point)
    {
      // Use hash table to get close to desired halfedge use the hash function to find the place in the hash map that this HalfEdge should be.
      var bucket = (int) ((point.X - Voronoi._minimumX) / Voronoi._deltaX * _edgeListHashSize);

      // make sure that the bucket position is within the range of the hash-array
      if (bucket < 0)
      {
        bucket = 0;
      }

      if (bucket >= _edgeListHashSize)
      {
        bucket = _edgeListHashSize - 1;
      }

      var halfEdge = GetHashFromEdgeList(bucket);

      // if the HE isn't found, search backwards and forwards in the hash map for the first non-null entry
      if (halfEdge == null)
      {
        for (var i = 1; i < _edgeListHashSize; i++)
        {
          if ((halfEdge = GetHashFromEdgeList(bucket - i)) != null)
            break;

          if ((halfEdge = GetHashFromEdgeList(bucket + i)) != null)
            break;
        }
      }

      // Now search linear list of halfedges for the correct one
      if (halfEdge == _edgeListLeftHalfEdge || halfEdge != _edgeListRightHalfEdge && RightSideOf(halfEdge, point))
      {
        // keep going right on the list until either the end is reached, or you find the 1st edge which the point isn't to the right of
        do
        {
          halfEdge = halfEdge.Right;
        } while (halfEdge != _edgeListRightHalfEdge && RightSideOf(halfEdge, point));

        halfEdge = halfEdge.Left;
      }
      else // if the point is to the left of the HalfEdge, then search left for the HE just to the left of the point
      {
        do
        {
          halfEdge = halfEdge.Left;
        } while (halfEdge != _edgeListLeftHalfEdge && !RightSideOf(halfEdge, point));
      }

      // Update hash table and reference counts
      if (bucket > 0 && bucket < _edgeListHashSize - 1)
      {
        _edgeListHash[bucket] = halfEdge;
      }

      return halfEdge;
    }


    // Returns true if point is to right of halfedge edge
    private static bool RightSideOf(HalfEdge halfEdge, Point point)
    {
      var above = false;
      var edge = halfEdge.EdgeListEdge;
      var topsite = edge.Region1;
      var rightOfSite = point.X > topsite.Point.X;

      if (rightOfSite && halfEdge.EdgeListSide == Voronoi.LeftSide)
      {
        return true;
      }

      if (!rightOfSite && halfEdge.EdgeListSide == Voronoi.RightSide)
      {
        return false;
      }

      if (Voronoi.IsNearlyEqual(edge.A, 1D))
      {
        var deltaX = point.X - topsite.Point.X;
        var deltaY = point.Y - topsite.Point.Y;
        var fast = false;

        if (!rightOfSite && edge.B < 0D || rightOfSite && edge.B >= 0D)
        {
          above = deltaY >= edge.B * deltaX;
          fast = above;
        }
        else
        {
          above = point.X + point.Y * edge.B > edge.C;

          if (edge.B < 0D)
          {
            above = !above;
          }

          if (!above)
          {
            fast = true;
          }
        }

        if (!fast)
        {
          var dxs = topsite.Point.X - edge.Region0.Point.X;

          above = edge.B * (deltaX * deltaX - deltaY * deltaY) < dxs * deltaY * (1.0 + 2.0 * deltaX / dxs + edge.B * edge.B);

          if (edge.B < 0)
          {
            above = !above;
          }
        }
      }
      else // edge.b == 1.0
      {
        var yl = edge.C - edge.A * point.X;
        var t1 = point.Y - yl;
        var t2 = point.X - topsite.Point.X;
        var t3 = yl - topsite.Point.Y;

        above = t1 * t1 > t2 * t2 + t3 * t3;
      }

      return halfEdge.EdgeListSide == Voronoi.LeftSide ? above : !above;
    }
  }
}
