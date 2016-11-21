using System;
using System.Collections.Generic;
using System.Linq;

namespace Voronoi2
{
  public class Voronoi
  {
    private const int LeftSide = 0;
    private const int RightSide = 1;

    // ************* Private members ******************
    private readonly List<GraphEdge> _allEdges = new List<GraphEdge>();
    private readonly double _minimumDistanceBetweenSites;
    private double _borderMinimumX;
    private double _borderMaximumX;
    private double _borderMinimumY;
    private double _borderMaximumY;

    private int _siteCount;
    private int _siteCountSquareRoot;
    private int _siteIndex;

    private double _deltaX;
    private double _deltaY;
    private double _minimumX;
    private double _maximumX;
    private double _minimumY;
    private double _maximumY;

    private int _edgeCount;
    private int _vertexCount;

    private Site[] _sites;
    private Site _bottomSite;

    private int _priorityQueueCount;
    private int _priorityQueueMinimum;
    private int _priorityQueueHashSize;
    private HalfEdge[] _priorityQueueHash;

    private HalfEdge[] _edgeListHash;
    private int _edgeListHashSize;
    private HalfEdge _edgeListLeftHalfEdge;
    private HalfEdge _edgeListRightHalfEdge;

    public Voronoi(double minimumDistanceBetweenSites = 0.1)
    {
      _minimumDistanceBetweenSites = minimumDistanceBetweenSites;
    }

    public List<GraphEdge> GenerateVoronoi(IReadOnlyList<Point> points
      , double borderMinimumX
      , double borderMaximumX
      , double borderMinimumY
      , double borderMaximumY)
    {
      if (borderMinimumX > borderMaximumX)
      {
        Swap(ref borderMinimumX, ref borderMinimumX);
      }

      if (borderMinimumY > borderMaximumY)
      {
        Swap(ref borderMinimumY, ref borderMaximumY);
      }

      _borderMinimumX = borderMinimumX;
      _borderMinimumY = borderMinimumY;
      _borderMaximumX = borderMaximumX;
      _borderMaximumY = borderMaximumY;

      _allEdges.Clear();
      _edgeCount = 0;
      _siteCount = points.Count;
      _siteCountSquareRoot = (int) Math.Sqrt(_siteCount + 4D);
      _siteIndex = 0;
      _sites = new Site[_siteCount];
      _vertexCount = 0;

      Sort(points);
      Generate();

      return _allEdges;
    }

    // implicit parameters: nsites, sqrt_nsites, xmin, xmax, ymin, ymax, deltax,
    // deltay (can all be estimates). 
    // Performance suffers if they are wrong; better to make nsites, deltax, and deltay too big than too small. (?)
    private void Generate()
    {
      var newIntersection = default(Point);

      HalfEdge leftBoundaryHalfEdge;
      Edge edge;

      InitializePriorityQueue();
      InitializeEdgeList();

      _bottomSite = NextSite();

      var nextSite = NextSite();

      while (true)
      {
        if (!IsEmptyPriorityQueue())
        {
          newIntersection = GetMinimumPriorityQueue();
        }

        Site bottomSite;
        Site site;
        HalfEdge rightBoundaryHalfEdge;

        // if the lowest site has a smaller Y value than the lowest vector intersection, process the site otherwise process the vector intersection.
        if (nextSite != null && (IsEmptyPriorityQueue() || nextSite.Point.Y < newIntersection.Y || IsNearlyEqual(nextSite.Point.Y, newIntersection.Y) && nextSite.Point.X < newIntersection.X))
        {
          // New site is smallest -this is a site event get the first HalfEdge to the LEFT of the new site
          leftBoundaryHalfEdge = LeftBoundaryEdgeList(nextSite.Point);

          // Get the first HalfEdge to the RIGHT of the new site
          rightBoundaryHalfEdge = leftBoundaryHalfEdge.Right;

          // if this halfedge has no edge, bot = bottom site (whatever that is)
          bottomSite = RightRegion(leftBoundaryHalfEdge);

          // create a new edge that bisects
          edge = Bisect(bottomSite, nextSite);

          // create a new HalfEdge, setting its EdgeListSide field to 0
          var bisector = CreateHalfEdge(edge, LeftSide);

          // insert this new bisector edge between the left and right vectors in a linked list
          InsertIntoEdgeList(leftBoundaryHalfEdge, bisector);

          // if the new bisector intersects with the left edge,
          // remove the left edge'site vertex, and put in the new one
          if ((site = Intersect(leftBoundaryHalfEdge, bisector)) != null)
          {
            DeletePriorityQueue(leftBoundaryHalfEdge);
            InsertPriorityQueue(leftBoundaryHalfEdge, site, Distance(site, nextSite));
          }

          leftBoundaryHalfEdge = bisector;

          // create a new HalfEdge, setting its EdgeListSide field to 1
          bisector = CreateHalfEdge(edge, RightSide);

          // insert the new HE to the right of the original bisector earlier in the IF stmt
          InsertIntoEdgeList(leftBoundaryHalfEdge, bisector);

          // if this new bisector intersects with the new HalfEdge
          if ((site = Intersect(bisector, rightBoundaryHalfEdge)) != null)
          {
            // push the HE into the ordered linked list of vertices
            InsertPriorityQueue(bisector, site, Distance(site, nextSite));
          }

          nextSite = NextSite();
        }
        else if (!IsEmptyPriorityQueue())/* intersection is smallest - this is a vector event */
        {
          // pop the HalfEdge with the lowest vector off the ordered list of vectors.
          leftBoundaryHalfEdge = ExtractHalfEdgePriorityQueue();

          // get the HalfEdge to the left of the above HE
          var llbnd = leftBoundaryHalfEdge.Left;

          // get the HalfEdge to the right of the above HE
          rightBoundaryHalfEdge = leftBoundaryHalfEdge.Right;

          // get the HalfEdge to the right of the HE to the right of the lowest HE
          var rrbnd = rightBoundaryHalfEdge.Right;

          // get the Site to the left of the left HE which it bisects
          bottomSite = LeftRegion(leftBoundaryHalfEdge);

          // get the Site to the right of the right HE which it bisects
          var top = RightRegion(rightBoundaryHalfEdge);

          var vertex = leftBoundaryHalfEdge.Vertex;

          // Set the vertex number - couldn't do this earlier since we didn't know when it would be processed
          vertex.SiteIndex = _vertexCount++;

          // Set the endpoint of the left HalfEdge to be this vector.
          Endpoint(leftBoundaryHalfEdge.EdgeListEdge, leftBoundaryHalfEdge.EdgeListSide, vertex);
          
          // Set the endpoint of the right HalfEdge to be this vector
          Endpoint(rightBoundaryHalfEdge.EdgeListEdge, rightBoundaryHalfEdge.EdgeListSide, vertex);

          // mark the lowest half-edge for deletion - can't delete yet because there might be pointers to it in Hash Map.
          DeleteFromEdgeList(leftBoundaryHalfEdge);

          // Remove all vertex events to do with the right HE
          DeletePriorityQueue(rightBoundaryHalfEdge);

          // mark the right HE for deletion - can't delete yet because there might be pointers to it in Hash Map
          DeleteFromEdgeList(rightBoundaryHalfEdge);

          var side = LeftSide;

          // if the site to the left of the event is higher than the Site
          if (bottomSite.Point.Y > top.Point.Y)
          {
            // to the right of it, then swap them and set the 'pm' variable to 1
            Swap(ref bottomSite, ref top);

            side = RightSide;
          }

          // create an Edge (or line) that is between the two Sites. 
          // This creates the formula of the line, and assigns a line number to it
          edge = Bisect(bottomSite, top);

          // create a HE from the Edge 'edge' and make it point to that edge with its EdgeListEdge field.
          var bisector = CreateHalfEdge(edge, side);

          // insert the new bisector to the
          // right of the left HE
          InsertIntoEdgeList(llbnd, bisector);

          // Set one endpoint to the new edge to be the vector point 'site'.
          // If the site to the left of this bisector is higher than the right Site, then this endpoint is put in position 0; otherwise in pos 1.
          Endpoint(edge, RightSide - side, vertex);

          // if left HE and the new bisector intersect, then delete the left HE, and reinsert it
          if ((site = Intersect(llbnd, bisector)) != null)
          {
            DeletePriorityQueue(llbnd);
            InsertPriorityQueue(llbnd, site, Distance(site, bottomSite));
          }

          // if right HE and the new bisector intersect, then reinsert it
          if ((site = Intersect(bisector, rrbnd)) != null)
          {
            InsertPriorityQueue(bisector, site, Distance(site, bottomSite));
          }
        }
        else
        {
          break;
        }
      }

      for (leftBoundaryHalfEdge = _edgeListLeftHalfEdge.Right; leftBoundaryHalfEdge != _edgeListRightHalfEdge; leftBoundaryHalfEdge = leftBoundaryHalfEdge.Right)
      {
        ClipEdge(edge = leftBoundaryHalfEdge.EdgeListEdge);
      }
    }

    private static double Shift(double value, ref double minimum, ref double maximum)
    {
      if (value < minimum)
      {
        minimum = value;
      }
      else if (value > maximum)
      {
        maximum = value;
      }

      return maximum - minimum;
    }

    private static void Swap<T>(ref T left, ref T right)
    {
      var temp = left;

      left = right;
      right = temp;
    }

    private void Sort(IReadOnlyList<Point> points)
    {
      _maximumX = _minimumX = points[0].X;
      _maximumY = _minimumY = points[0].Y;

      for (var siteIndex = 0; siteIndex < _siteCount; siteIndex++)
      {
        _sites[siteIndex] = new Site(points[siteIndex], siteIndex);
        _deltaX = Shift(points[siteIndex].X, ref _minimumX, ref _maximumY);
        _deltaY = Shift(points[siteIndex].Y, ref _minimumY, ref _maximumX);
      }

      _sites = _sites.OrderBy(site => site.Point.Y).ThenBy(site => site.Point.X).ToArray();
    }

    private Site NextSite() => _siteIndex < _siteCount
      ? _sites[_siteIndex++]
      : default(Site);

    private Edge Bisect(Site s1, Site s2)
    {
      var newEdge = new Edge
      {
        Region =
        {
          [0] = s1,
          [1] = s2
        },
        EndPoint =
        {
          [0] = default(Site),
          [1] = default(Site)
        }
      };

      var deltaX = s2.Point.X - s1.Point.X;
      var deltaY = s2.Point.Y - s1.Point.Y;

      newEdge.C = s1.Point.X * deltaX + s1.Point.Y * deltaY + (deltaX * deltaX + deltaY * deltaY) * 0.5;

      if ((deltaX > 0 ? deltaX : -deltaX) > (deltaY > 0 ? deltaY : -deltaY))
      {
        newEdge.A = 1.0;
        newEdge.B = deltaY / deltaX;
        newEdge.C /= deltaX;
      }
      else
      {
        newEdge.A = deltaX / deltaY;
        newEdge.B = 1.0;
        newEdge.C /= deltaY;
      }

      newEdge.EdgeIndex = _edgeCount++;

      return newEdge;
    }

    private void MakeVertex(Site site) => site.SiteIndex = _vertexCount++;

    private void InitializePriorityQueue()
    {
      _priorityQueueCount = 0;
      _priorityQueueMinimum = 0;
      _priorityQueueHashSize = 4 * _siteCountSquareRoot;
      _priorityQueueHash = new HalfEdge[_priorityQueueHashSize];

      for (var i = 0; i < _priorityQueueHashSize; i++)
      {
        _priorityQueueHash[i] = new HalfEdge();
      }
    }

    private int GetPriorityQueueBucket(HalfEdge halfEdge)
    {
      var bucket = (int) ((halfEdge.Ystar - _minimumY) / _deltaY * _priorityQueueHashSize);

      if (bucket < 0)
      {
        bucket = 0;
      }

      if (bucket >= _priorityQueueHashSize)
      {
        bucket = _priorityQueueHashSize - 1;
      }

      if (bucket < _priorityQueueMinimum)
      {
        _priorityQueueMinimum = bucket;
      }

      return bucket;
    }

    private void InsertPriorityQueue(HalfEdge halfEdge, Site site, double offset)
    {
      halfEdge.Vertex = site;
      halfEdge.Ystar = site.Point.Y + offset;

      var last = _priorityQueueHash[GetPriorityQueueBucket(halfEdge)];

      for (var next = last.Next
        ; next != null &&
        (
          halfEdge.Ystar > next.Ystar ||
          (
            IsNearlyEqual(halfEdge.Ystar, next.Ystar) && site.Point.X > next.Vertex.Point.X
          )
        )
        ; next = last.Next)
      {
        last = next;
      }

      halfEdge.Next = last.Next;
      last.Next = halfEdge;

      _priorityQueueCount++;
    }

    // remove the HalfEdge from the list of vertices
    private void DeletePriorityQueue(HalfEdge halfEdge)
    {
      if (halfEdge.Vertex != default(Site))
      {
        var last = _priorityQueueHash[GetPriorityQueueBucket(halfEdge)];

        while (last.Next != halfEdge)
        {
          last = last.Next;
        }

        last.Next = halfEdge.Next;
        _priorityQueueCount--;
        halfEdge.Vertex = null;
      }
    }

    private bool IsEmptyPriorityQueue() => _priorityQueueCount == 0;

    private Point GetMinimumPriorityQueue()
    {
      while (_priorityQueueHash[_priorityQueueMinimum].Next == null)
      {
        _priorityQueueMinimum++;
      }

      return new Point(_priorityQueueHash[_priorityQueueMinimum].Next.Vertex.Point.X, _priorityQueueHash[_priorityQueueMinimum].Next.Ystar);
    }

    private HalfEdge ExtractHalfEdgePriorityQueue()
    {
      var halfEdge = _priorityQueueHash[_priorityQueueMinimum].Next;

      _priorityQueueHash[_priorityQueueMinimum].Next = halfEdge.Next;
      _priorityQueueCount--;

      return halfEdge;
    }

    private static HalfEdge CreateHalfEdge(Edge edge, int pm) => new HalfEdge
    {
      EdgeListEdge = edge,
      EdgeListSide = pm,
      Next = null,
      Vertex = null
    };

    private void InitializeEdgeList()
    {
      _edgeListHashSize = 2 * _siteCountSquareRoot;
      _edgeListHash = new HalfEdge[_edgeListHashSize];

      for (var i = 0; i < _edgeListHashSize; i++)
      {
        _edgeListHash[i] = null;
      }

      _edgeListLeftHalfEdge = CreateHalfEdge(null, 0);
      _edgeListRightHalfEdge = CreateHalfEdge(null, 0);
      _edgeListLeftHalfEdge.Left = null;
      _edgeListLeftHalfEdge.Right = _edgeListRightHalfEdge;
      _edgeListRightHalfEdge.Left = _edgeListLeftHalfEdge;
      _edgeListRightHalfEdge.Right = null;
      _edgeListHash[0] = _edgeListLeftHalfEdge;
      _edgeListHash[_edgeListHashSize - 1] = _edgeListRightHalfEdge;
    }

    private Site LeftRegion(HalfEdge halfEdge)
    {
      return halfEdge.EdgeListEdge != null
        ? (halfEdge.EdgeListSide == LeftSide ? halfEdge.EdgeListEdge.Region[LeftSide] : halfEdge.EdgeListEdge.Region[RightSide])
        : _bottomSite;
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

    /* Get entry from hash table, pruning any deleted nodes */
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
      var bucket = (int) ((point.X - _minimumX) / _deltaX * _edgeListHashSize);

      // make sure that the bucket position is within the range of the hash-array
      if (bucket < 0)
      {
        bucket = 0;
      }

      if (bucket >= _edgeListHashSize)
      {
        bucket = _edgeListHashSize - 1;
      }

      var he = GetHashFromEdgeList(bucket);

      // if the HE isn't found, search backwards and forwards in the hash map
      // for the first non-null entry
      if (he == null)
      {
        for (var i = 1; i < _edgeListHashSize; i++)
        {
          if ((he = GetHashFromEdgeList(bucket - i)) != null)
            break;
          if ((he = GetHashFromEdgeList(bucket + i)) != null)
            break;
        }
      }

      /* Now search linear list of halfedges for the correct one */
      if (he == _edgeListLeftHalfEdge || (he != _edgeListRightHalfEdge && RightSideOf(he, point)))
      {
        // keep going right on the list until either the end is reached, or
        // you find the 1st edge which the point isn't to the right of
        do
        {
          he = he.Right;
        }
        while (he != _edgeListRightHalfEdge && RightSideOf(he, point));
        he = he.Left;
      }
      else
      // if the point is to the left of the HalfEdge, then search left for
      // the HE just to the left of the point
      {
        do
        {
          he = he.Left;
        }
        while (he != _edgeListLeftHalfEdge && !RightSideOf(he, point));
      }

      /* Update hash table and reference counts */
      if (bucket > 0 && bucket < _edgeListHashSize - 1)
      {
        _edgeListHash[bucket] = he;
      }

      return he;
    }

    private void ClipEdge(Edge edge)
    {
      var x1 = edge.Region[0].Point.X;
      var y1 = edge.Region[0].Point.Y;
      var x2 = edge.Region[1].Point.X;
      var y2 = edge.Region[1].Point.Y;
      var x = x2 - x1;
      var y = y2 - y1;

      // if the distance between the two points this line was created from is less than the square root of 2, then ignore it
      if (Math.Sqrt(x * x + y * y) < _minimumDistanceBetweenSites)
      {
        return;
      }

      Site s1;
      Site s2;

      if (IsNearlyEqual(edge.A, 1D) && edge.B >= 0D)
      {
        s1 = edge.EndPoint[1];
        s2 = edge.EndPoint[0];
      }
      else
      {
        s1 = edge.EndPoint[0];
        s2 = edge.EndPoint[1];
      }

      if (IsNearlyEqual(edge.A, 1D))
      {
        y1 = _borderMinimumY;

        if (s1 != null && s1.Point.Y > _borderMinimumY)
        {
          y1 = s1.Point.Y;
        }

        if (y1 > _borderMaximumY)
        {
          y1 = _borderMaximumY;
        }

        x1 = edge.C - edge.B * y1;
        y2 = _borderMaximumY;

        if (s2 != null && s2.Point.Y < _borderMaximumY)
        {
          y2 = s2.Point.Y;
        }

        if (y2 < _borderMinimumY)
        {
          y2 = _borderMinimumY;
        }

        x2 = edge.C - edge.B * y2;

        if (x1 > _borderMaximumX && x2 > _borderMaximumX || x1 < _borderMinimumX && x2 < _borderMinimumX)
        {
          return;
        }

        if (x1 > _borderMaximumX)
        {
          x1 = _borderMaximumX;
          y1 = (edge.C - x1) / edge.B;
        }

        if (x1 < _borderMinimumX)
        {
          x1 = _borderMinimumX;
          y1 = (edge.C - x1) / edge.B;
        }

        if (x2 > _borderMaximumX)
        {
          x2 = _borderMaximumX;
          y2 = (edge.C - x2) / edge.B;
        }

        if (x2 < _borderMinimumX)
        {
          x2 = _borderMinimumX;
          y2 = (edge.C - x2) / edge.B;
        }
      }
      else
      {
        x1 = _borderMinimumX;

        if (s1 != null && s1.Point.X > _borderMinimumX)
        {
          x1 = s1.Point.X;
        }

        if (x1 > _borderMaximumX)
        {
          x1 = _borderMaximumX;
        }

        y1 = edge.C - edge.A * x1;
        x2 = _borderMaximumX;

        if (s2 != null && s2.Point.X < _borderMaximumX)
        {
          x2 = s2.Point.X;
        }

        if (x2 < _borderMinimumX)
        {
          x2 = _borderMinimumX;
        }

        y2 = edge.C - edge.A * x2;

        if (y1 > _borderMaximumY & y2 > _borderMaximumY || y1 < _borderMinimumY & y2 < _borderMinimumY)
        {
          return;
        }

        if (y1 > _borderMaximumY)
        {
          y1 = _borderMaximumY;
          x1 = (edge.C - y1) / edge.A;
        }

        if (y1 < _borderMinimumY)
        {
          y1 = _borderMinimumY;
          x1 = (edge.C - y1) / edge.A;
        }

        if (y2 > _borderMaximumY)
        {
          y2 = _borderMaximumY;
          x2 = (edge.C - y2) / edge.A;
        }

        if (y2 < _borderMinimumY)
        {
          y2 = _borderMinimumY;
          x2 = (edge.C - y2) / edge.A;
        }
      }

      _allEdges.Add(new GraphEdge(new Point(x1, y1), new Point(x2, y2), edge.Region[0].SiteIndex, edge.Region[1].SiteIndex));
    }

    private void Endpoint(Edge edge, int side, Site site)
    {
      edge.EndPoint[side] = site;

      if (edge.EndPoint[RightSide - side] == null)
        return;

      ClipEdge(edge);
    }

    // Returns true if point is to right of halfedge edge
    private static bool RightSideOf(HalfEdge el, Point p)
    {
      bool above;

      var e = el.EdgeListEdge;
      var topsite = e.Region[1];

      var rightOfSite = p.X > topsite.Point.X;

      if (rightOfSite && el.EdgeListSide == LeftSide)
      {
        return true;
      }

      if (!rightOfSite && el.EdgeListSide == RightSide)
      {
        return false;
      }

      if (IsNearlyEqual(e.A, 1D))
      {
        var dxp = p.X - topsite.Point.X;
        var dyp = p.Y - topsite.Point.Y;

        var fast = false;

        if ((!rightOfSite & (e.B < 0.0)) | (rightOfSite & (e.B >= 0.0)))
        {
          above = dyp >= e.B * dxp;
          fast = above;
        }
        else
        {
          above = p.X + p.Y * e.B > e.C;
          if (e.B < 0.0)
            above = !above;
          if (!above)
            fast = true;
        }
        if (!fast)
        {
          var dxs = topsite.Point.X - e.Region[0].Point.X;

          above = e.B * (dxp * dxp - dyp * dyp) < dxs * dyp * (1.0 + 2.0 * dxp / dxs + e.B * e.B);

          if (e.B < 0)
          {
            above = !above;
          }
        }
      }
      else // edge.b == 1.0
      {
        var yl = e.C - e.A * p.X;
        var t1 = p.Y - yl;
        var t2 = p.X - topsite.Point.X;
        var t3 = yl - topsite.Point.Y;

        above = t1 * t1 > t2 * t2 + t3 * t3;
      }

      return el.EdgeListSide == LeftSide ? above : !above;
    }

    private Site RightRegion(HalfEdge halfEdge)
    {
      // if this halfedge has no edge, return the bottom site (whatever that is).
      if (halfEdge.EdgeListEdge == null)
      {
        return _bottomSite;
      }

      // if the EdgeListSide field is zero, return the site 0 that this edge bisects, otherwise return site number 1.
      return halfEdge.EdgeListSide == LeftSide
        ? halfEdge.EdgeListEdge.Region[RightSide]
        : halfEdge.EdgeListEdge.Region[LeftSide];
    }

    public double Distance(Site s, Site t)
    {
      var dx = s.Point.X - t.Point.X;
      var dy = s.Point.Y - t.Point.Y;

      return Math.Sqrt(dx * dx + dy * dy);
    }

    private const double DefaultTolerance = 1e-10;

    private static bool IsNearlyEqual(double x, double y, double tolerance = DefaultTolerance) => Math.Abs(x - y) <= tolerance;

    // create a new site where the HalfEdges halfEdge0 and halfEdge1 intersect - note that the Point in the argument list is not used, don't know why it'site there.
    private static Site Intersect(HalfEdge halfEdge0, HalfEdge halfEdge1)
    {
      var e1 = halfEdge0.EdgeListEdge;
      var e2 = halfEdge1.EdgeListEdge;

      // if the two edges bisect the same parent, return null
      if (e1 == null || e2 == null || e1.Region[1] == e2.Region[1])
        return null;

      var d = e1.A * e2.B - e1.B * e2.A;

      if (-DefaultTolerance < d && d < DefaultTolerance)
        return null;

      var intersectX = (e1.C * e2.B - e2.C * e1.B) / d;
      var intersectY = (e2.C * e1.A - e1.C * e2.A) / d;

      Edge edge;
      HalfEdge halfEdge;

      if (e1.Region[1].Point.Y < e2.Region[1].Point.Y || (IsNearlyEqual(e1.Region[1].Point.Y, e2.Region[1].Point.Y) && e1.Region[1].Point.X < e2.Region[1].Point.X))
      {
        halfEdge = halfEdge0;
        edge = e1;
      }
      else
      {
        halfEdge = halfEdge1;
        edge = e2;
      }

      var rightOfSite = intersectX >= edge.Region[1].Point.X;

      if (rightOfSite && halfEdge.EdgeListSide == LeftSide || !rightOfSite && halfEdge.EdgeListSide == RightSide)
      {
        return null;
      }

      // Create a new site at the point of intersection - this is a new vector event waiting to happen
      return new Site(intersectX, intersectY);
    }

  }
}