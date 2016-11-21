using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;

using CSPoint = System.Drawing.Point;

namespace Voronoi2
{
  /// <summary>
  /// Description of MainForm.
  /// </summary>
  public sealed partial class MainForm : Form
  {
    private static int _siteCount = 10;

    public readonly Random Random = new Random(42);
    public readonly Voronoi Voronoi = new Voronoi();

    public MainForm()
    {
      InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      SiteCounter.Value = _siteCount;
    }

    private void SpreadPoints()
    {
      var sites = new List<Point>();

      for (var i = 0; i < _siteCount; i++)
      {
        sites.Add(new Point(Random.NextDouble() * DiagramHost.Width, Random.NextDouble() * DiagramHost.Height));
      }

      var bitmap = new Bitmap(DiagramHost.Width, DiagramHost.Height);

      using (var graphics = Graphics.FromImage(bitmap))
      {
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.Clear(Color.White);

        var edges = Voronoi.GenerateVoronoi(sites, 0, DiagramHost.Width, 0, DiagramHost.Height);

        foreach (var edge in edges)
        {
          graphics.DrawLine(Pens.Black, (float) edge.P0.X, (float) edge.P0.Y, (float) edge.P1.X, (float) edge.P1.Y);
        }

        var dict = new Dictionary<int, List<PointF>>();

        foreach (var edge in edges)
        {
          var set = default(List<PointF>);

          if (!dict.TryGetValue(edge.SiteIndex0, out set))
          {
            dict[edge.SiteIndex0] = set = new List<PointF>();
          }

          set.Add(sites[edge.SiteIndex0]);
          set.Add(edge.P0);
          set.Add(edge.P1);

          if (!dict.TryGetValue(edge.SiteIndex1, out set))
          {
            dict[edge.SiteIndex1] = set = new List<PointF>();
          }

          set.Add(sites[edge.SiteIndex1]);
          set.Add(edge.P0);
          set.Add(edge.P1);
        }
        
        foreach (var set in dict.Values)
        {
          var brush = new SolidBrush(Color.FromArgb(-0x7F000000 + Random.Next(0x00FFFFFF)));
          var points = set.ToArray();

          //for (var index = 0; index < points.Length; index += 3)
          {
            //var p0 = points[index + 0];
            //var p1 = points[index + 1];
            //var p2 = points[index + 2];

            //var cross = Vector3.Cross(new Vector3(p1.X - p0.X, p1.Y - p0.Y, 0), new Vector3(p2.X - p0.X, p2.Y - p0.Y, 0));

            //if (cross.Z < 0)
            //{
            //  points[index + 1] = p2;
            //  points[index + 2] = p1;
            //}
          }

          //graphics.DrawPolygon(Pens.DimGray, points);
          graphics.FillPolygon(brush, points);
        }

        for (var i = 0; i < sites.Count; i++)
        {
          graphics.DrawString($"{i + 1}", SystemFonts.MenuFont, Brushes.Black, (float) sites[i].X, (float) sites[i].Y - 22F);
          graphics.FillEllipse(Brushes.Blue, (float) (sites[i].X - 1.5), (float) (sites[i].Y - 1.5), 3F, 3F);
        }
      }

      DiagramHost.Image?.Dispose();
      DiagramHost.Image = bitmap;
    }

    private void OnGenerateClick(object sender, EventArgs e) => SpreadPoints();

    private void OnSiteCounterChanged(object sender, EventArgs e)
    {
      _siteCount = (int) SiteCounter.Value;

      SpreadPoints();
    }

    private void OnDiagramHostMouseMove(object sender, MouseEventArgs e) => Text = $"Voronoi [{e.X},{e.Y}]";
  }
}
