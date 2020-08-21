using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;

using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;


using MeshClassLibrary;



/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public class Script_Instance : GH_ScriptInstance
{
    #region Utility functions
    /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
    /// <param name="text">String to print.</param>
    private void Print(string text) { /* Implementation hidden. */ }
    /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
    /// <param name="format">String format.</param>
    /// <param name="args">Formatting parameters.</param>
    private void Print(string format, params object[] args) { /* Implementation hidden. */ }
    /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
    /// <param name="obj">Object instance to parse.</param>
    private void Reflect(object obj) { /* Implementation hidden. */ }
    /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
    /// <param name="obj">Object instance to parse.</param>
    private void Reflect(object obj, string method_name) { /* Implementation hidden. */ }
    #endregion

    #region Members
    /// <summary>Gets the current Rhino document.</summary>
    private readonly RhinoDoc RhinoDocument;
    /// <summary>Gets the Grasshopper document that owns this script.</summary>
    private readonly GH_Document GrasshopperDocument;
    /// <summary>Gets the Grasshopper script component that owns this script.</summary>
    private readonly IGH_Component Component;
    /// <summary>
    /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
    /// Any subsequent call within the same solution will increment the Iteration count.
    /// </summary>
    private readonly int Iteration;
    #endregion

    /// <summary>
    /// This procedure contains the user code. Input parameters are provided as regular arguments,
    /// Output parameters as ref arguments. You don't have to assign output parameters,
    /// they will have a default value.
    /// </summary>
    private void RunScript(List<Point3d> x, double y, bool z, double u, ref object A, ref object B, ref object C, ref object D, ref object E, ref object F)
    {
        try
        {
            List<Line> ls = new List<Line>();//frofile
            List<Polyline> pls = new List<Polyline>();//frofile
            //////
            List<Polyline> pls01 = new List<Polyline>();//frame1
            List<Polyline> pls02 = new List<Polyline>();//frame2
            ////////
            pls.Add(box(new Line(x[0], x[1])));
            pls.Add(box(new Line(x[1], x[2])));
            Polyline pl = new Polyline();
            pl = box(new Line((x[0] + x[1]) / 2, (x[2] + x[1]) / 2));
            pl.Transform(Transform.Translation(0, 0, -y));
            pls.Add(pl);
            ////////
            ls.Add(new Line(pls[2][0], pls[0][0]));
            ls.Add(new Line(pls[2][1], pls[0][1]));
            ls.Add(new Line(pls[2][2], pls[0][2]));
            ls.Add(new Line(pls[2][3], pls[0][3]));
            ls.Add(new Line(pls[2][0], pls[1][0]));
            ls.Add(new Line(pls[2][1], pls[1][1]));
            ls.Add(new Line(pls[2][2], pls[1][2]));
            ls.Add(new Line(pls[2][3], pls[1][3]));
            ////////
            for (double i = -y; i <= 0.4; i += 0.4)
            {
                Plane p = Plane.WorldXY;
                p.Transform(Transform.Translation(0, 0, i));
                double t = 0;
                Polyline pl0 = new Polyline();
                Rhino.Geometry.Intersect.Intersection.LinePlane(ls[0], p, out t);
                pl0.Add(ls[0].PointAt(t));
                Rhino.Geometry.Intersect.Intersection.LinePlane(ls[1], p, out t);
                pl0.Add(ls[1].PointAt(t));
                Rhino.Geometry.Intersect.Intersection.LinePlane(ls[2], p, out t);
                pl0.Add(ls[2].PointAt(t));
                Rhino.Geometry.Intersect.Intersection.LinePlane(ls[3], p, out t);
                pl0.Add(ls[3].PointAt(t)); pl0.Add(pl0[0]);
                pls01.Add(pl0);
                pl0 = new Polyline();
                Rhino.Geometry.Intersect.Intersection.LinePlane(ls[4], p, out t);
                pl0.Add(ls[4].PointAt(t));
                Rhino.Geometry.Intersect.Intersection.LinePlane(ls[5], p, out t);
                pl0.Add(ls[5].PointAt(t));
                Rhino.Geometry.Intersect.Intersection.LinePlane(ls[6], p, out t);
                pl0.Add(ls[6].PointAt(t));
                Rhino.Geometry.Intersect.Intersection.LinePlane(ls[7], p, out t);
                pl0.Add(ls[7].PointAt(t)); pl0.Add(pl0[0]);
                pls02.Add(pl0);
            }
            ///////////////////////////////////////
            Mesh mesh1 = new Mesh();//frame
            Mesh mesh2 = new Mesh();//screw
            Mesh mesh3 = new Mesh();//Joint
            List<Line> output1 = new List<Line>();//screw M6*40
            List<Polyline> output2 = new List<Polyline>();//frame1&frame2
            List<Line> output3 = new List<Line>();//screw M4*40
            for (int i = 0; i < pls01.Count; i++)
            {
                Polyline pll = offset(pls01[i], 0.1);
                pll.Transform(Transform.Translation(0, 0, 0.1));
                pls01[i] = pll;
                mesh1.Append(desk(pll, 0.2, 0.2));
                if (i > 0)
                {
                    output1.AddRange(Intersect(pls01[i], pls01[i - 1]));
                }
            }
            pls02.RemoveAt(0);
            for (int i = 0; i < pls02.Count; i++)
            {
                Polyline pll = offset(pls02[i], 0.1);
                pll.Transform(Transform.Translation(0, 0, -0.1));
                pls02[i] = pll;
                mesh1.Append(desk(pll, 0.2, 0.2));
                if (i > 0)
                {
                    output1.AddRange(Intersect(pls02[i], pls02[i - 1]));
                }
            }
            ////////////////////////////////////////////////////////
            for (int i = 1; i < pls01.Count; i++)
            {
                mesh3.Append(Joint(pls01[i - 1], pls01[i], 0.2, 0.2, 0.5));
            }
            for (int i = 1; i < pls02.Count; i++)
            {
                Polyline PL01 = new Polyline();
                PL01.Add(pls02[i - 1][2]); PL01.Add(pls02[i - 1][3]); PL01.Add(pls02[i - 1][0]); PL01.Add(pls02[i - 1][1]);
                Polyline PL02 = new Polyline();
                PL02.Add(pls02[i][2]); PL02.Add(pls02[i][3]); PL02.Add(pls02[i][0]); PL02.Add(pls02[i][1]);
                mesh3.Append(Joint(PL01, PL02, 0.2, 0.2, 0.5));
            }
            ///////////////////////////////////////////////////////
            output2.AddRange(pls01); output2.AddRange(pls02);

            for (int i = 0; i < output1.Count; i++)
            {
                mesh2.Append(MT.MeshPipe(output1[i], 0.02));
            }
            output1.Sort(CompareDinosByHeight1);
            output2.Sort(CompareDinosByHeight2);

            for (int i = 1; i < output2.Count; i++)
            {
                output3.AddRange(Intersect(output2[i], output2[i - 1]));
            }

            output3.AddRange(output1);
            output3.Sort(CompareDinosByHeight1);




            A = output2; B = output1; C = output3;
            D = mesh1; E = mesh2; F = mesh3;
        }
        catch (Exception ex)
        {
            Print(ex.ToString());
        }

    }

    // <Custom additional code> 
    public MeshTools MT = new MeshTools();
    public static int CompareDinosByHeight2(Polyline x, Polyline y)
    {
        if (x == null) { if (y == null) { return 0; } else { return -1; } }
        else
        {
            if (y == null) { return 1; }
            else
            {
                if (x[0].Z > y[0].Z) return 1;
                if (x[0].Z == y[0].Z) return 0;
                if (x[0].Z < y[0].Z) return -1;
                else return 0;
            }
        }
    }
    public static int CompareDinosByHeight1(Line x, Line y)
    {
        if (x == null) { if (y == null) { return 0; } else { return -1; } }
        else
        {
            if (y == null) { return 1; }
            else
            {
                if (x.From.Z > y.From.Z) return 1;
                if (x.From.Z == y.From.Z) return 0;
                if (x.From.Z < y.From.Z) return -1;
                else return 0;
            }
        }
    }
    Mesh Joint(Polyline pl1, Polyline pl2, double u, double v, double w)
    {
        Mesh mesh = new Mesh();
        Vector3d v11 = new Vector3d(pl1[2] - pl1[0]);
        v11.Unitize(); v11 *= (Math.Sqrt(2) * u) / 2;
        Line l12 = new Line(pl1[0] - v11, pl1[0] + v11);
        Line l11 = new Line(l12.From, l12.To);
        Line l13 = new Line(l12.From, l12.To);
        Vector3d v12 = pl1[1] - pl1[0];
        v12.Unitize(); v12 *= w;
        Vector3d v13 = pl1[3] - pl1[0];
        v13.Unitize(); v13 *= w;
        l13.Transform(Transform.Translation(v12));
        l11.Transform(Transform.Translation(v13));
        l11.Transform(Transform.Translation(new Vector3d(0, 0, v / 2)));
        l12.Transform(Transform.Translation(new Vector3d(0, 0, v / 2)));
        l13.Transform(Transform.Translation(new Vector3d(0, 0, v / 2)));
        mesh.Append(MT.MeshLoft(l11, l12));
        mesh.Append(MT.MeshLoft(l12, l13));
        Vector3d v21 = new Vector3d(pl2[2] - pl2[0]);
        v21.Unitize(); v21 *= (Math.Sqrt(2) * u) / 2;
        Line l22 = new Line(pl2[0] - v21, pl2[0] + v21);
        Line l21 = new Line(l22.From, l22.To);
        Line l23 = new Line(l22.From, l22.To);
        Vector3d v22 = pl2[1] - pl2[0];
        v22.Unitize(); v22 *= w;
        Vector3d v23 = pl2[3] - pl2[0];
        v23.Unitize(); v23 *= w;
        l23.Transform(Transform.Translation(v22));
        l21.Transform(Transform.Translation(v23));
        l21.Transform(Transform.Translation(new Vector3d(0, 0, -v / 2)));
        l22.Transform(Transform.Translation(new Vector3d(0, 0, -v / 2)));
        l23.Transform(Transform.Translation(new Vector3d(0, 0, -v / 2)));
        mesh.Append(MT.MeshLoft(l21, l22));
        mesh.Append(MT.MeshLoft(l22, l23));
        mesh.Append(MT.MeshLoft(l12, l22));
        return mesh;
    }
    Polyline offset(Polyline box, double u)
    {
        Polyline pl22 = new Polyline(box);
        Vector3d v;
        Point3d c1 = (pl22[0] + pl22[1] + pl22[2] + pl22[3]) / 4;
        v = pl22[0] - c1; v.Unitize(); v *= Math.Sqrt(2) * u; pl22[0] -= v;
        v = pl22[1] - c1; v.Unitize(); v *= Math.Sqrt(2) * u; pl22[1] -= v;
        v = pl22[2] - c1; v.Unitize(); v *= Math.Sqrt(2) * u; pl22[2] -= v;
        v = pl22[3] - c1; v.Unitize(); v *= Math.Sqrt(2) * u; pl22[3] -= v;
        v = pl22[4] - c1; v.Unitize(); v *= Math.Sqrt(2) * u; pl22[4] -= v;
        return pl22;
    }
    Mesh desk(Polyline l, double t, double u)
    {
        List<Polyline> pls3 = new List<Polyline>();
        Polyline pl11 = offset(l, -u / 2);
        pl11.Transform(Transform.Translation(new Vector3d(0, 0, -t / 2)));
        Polyline pl12 = offset(pl11, u);
        Polyline pl21 = new Polyline(pl11);
        pl21.Transform(Transform.Translation(new Vector3d(0, 0, t)));
        Polyline pl22 = offset(pl21, u);
        pls3.Add(pl11); pls3.Add(pl12); pls3.Add(pl22); pls3.Add(pl21);
        return MT.MeshLoft(pls3, false, true);

    }
    List<Line> Intersect(Polyline l1, Polyline l2)
    {
        double z1 = l1[0].Z;
        double z2 = l2[0].Z;
        Polyline l3 = new Polyline(l2);
        l3.Transform(Transform.Translation(0, 0, z1 - z2));
        Rhino.Geometry.Intersect.CurveIntersections cis =
          Rhino.Geometry.Intersect.Intersection.CurveCurve(l1.ToNurbsCurve(), l3.ToNurbsCurve(), 0.01, 0.01);
        List<Line> output = new List<Line>();
        Print(cis.Count.ToString());//////////////////////
        if (cis.Count > 0)
        {
            for (int k = 0; k < cis.Count; k++)
            {
                Point3d p = cis[k].PointA;
                output.Add(new Line(p, p + new Vector3d(0, 0, z2 - z1)));
            }
        }
        return output;
    }
    Polyline box(Line l)
    {
        Polyline pl = new Polyline();
        Line l2 = new Line(l.From, l.To);
        l2.Transform(Transform.Rotation(Math.PI / 2, (l.From + l.To) / 2));
        pl.Add(l.From);
        pl.Add(l2.From);
        pl.Add(l.To);
        pl.Add(l2.To);
        Plane p = new Plane(pl[0], pl[1], pl[2]);
        if (Vector3d.VectorAngle(p.Normal, Vector3d.ZAxis) > Math.PI / 2) pl.Reverse();
        pl.Add(pl[0]);
        return pl;
    }
    // </Custom additional code> 
}