using HybridShapeTypeLib;
using INFITF;
using MECMOD;
using System.Collections.Generic;

namespace ToCatia1
{
    public class CATIA_VBA
    {
        public INFITF.Application catia;
        public string out_Err = "";
        public static string PartBody = "PartBody";
        public CATIA_VBA()
        {
            try
            {
                //System.Type catiaType = System.Type.GetTypeFromProgID("CATIA.Application");
                //INFITF.Application catia = (INFITF.Application)System.Activator.CreateInstance(catiaType);
                // catia.Visible = true;
                catia = (INFITF.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Catia.Application");
                catia.Visible = true;
            }
            catch (Exception ex)
            {
                out_Err = ex.ToString();
            }
        }
        public void AddSpline(List<double> px, List<double> py, string BodyName, string SketchName)
        {
            if ((px.Count < 0) || (py.Count < 0) || (px.Count != py.Count)) return;
            PartDocument partDocument1 = (PartDocument)catia.ActiveDocument;
            out_Err += partDocument1.get_Name();
            Sketches sketches1 = partDocument1.Part.Bodies.Item(BodyName).Sketches;
            Sketch sketch1 = sketches1.Item(SketchName);
            Line2D LX = sketch1.Factory2D.CreateLine(0, 0, 0, 50);
            Line2D LY = sketch1.Factory2D.CreateLine(0, 0, 50, 0);
            Object[] arrayOfObject1 = new Object[px.Count];
            for (int i = 0; i < px.Count; i++)
            {
                ControlPoint2D cp = sketch1.Factory2D.CreateControlPoint(px[i], py[i]);

                arrayOfObject1[i] = cp;
                Reference reference1 = partDocument1.Part.CreateReferenceFromObject(LX);
                Reference reference2 = partDocument1.Part.CreateReferenceFromObject(cp);
                Constraint constraint1 = sketch1.Constraints.AddBiEltCst(CatConstraintType.catCstTypeDistance, reference1, reference2);
                constraint1.Dimension.Value = Math.Abs(px[i]);
                reference1 = partDocument1.Part.CreateReferenceFromObject(LY);
                Constraint constraint2 = sketch1.Constraints.AddBiEltCst(CatConstraintType.catCstTypeDistance, reference1, reference2);
                constraint2.Dimension.Value = Math.Abs(py[i]);
            }
            Spline2D sp = sketch1.Factory2D.CreateSpline(arrayOfObject1);
        }
        public void AddPlaneEquation(double a, double b, double c, double d, string BodyName)
        {
            PartDocument partDocument1 = (PartDocument)catia.ActiveDocument;
            out_Err += partDocument1.get_Name();
            HybridShapeFactory shapefactory1 = (HybridShapeFactory)partDocument1.Part.HybridShapeFactory;
            HybridShapePlaneEquation plane1 = shapefactory1.AddNewPlaneEquation(a, b, c, d);
            Body body1 = partDocument1.Part.Bodies.Item(BodyName);
            body1.InsertHybridShape(plane1);
        }
        public void AddPointCoord(double x, double y, double z, string BodyName)
        {
            PartDocument partDocument1 = (PartDocument)catia.ActiveDocument;
            out_Err += partDocument1.get_Name();
            HybridShapeFactory shapefactory1 = (HybridShapeFactory)partDocument1.Part.HybridShapeFactory;
            HybridShapePointCoord point1 = shapefactory1.AddNewPointCoord(x, y, z);
            Body body1 = partDocument1.Part.Bodies.Item(BodyName);
            body1.InsertHybridShape(point1);
        }
        public void AddSpline(List<double> px, List<double> py, List<double> pz, string BodyName)
        {
            PartDocument partDocument1 = (PartDocument)catia.ActiveDocument;
            out_Err += partDocument1.get_Name();
            HybridShapeFactory shapefactory1 = (HybridShapeFactory)partDocument1.Part.HybridShapeFactory;
            HybridShapeSpline sp1 = shapefactory1.AddNewSpline();
            sp1.SetSplineType(0);
            sp1.SetClosing(0);
            Body body1 = partDocument1.Part.Bodies.Item(BodyName);
            for (int i = 0; i < px.Count; i++)
            {
                HybridShapePointCoord point1 = shapefactory1.AddNewPointCoord(px[i], py[i], pz[i]);
                body1.InsertHybridShape(point1);
                Reference reference1 = partDocument1.Part.CreateReferenceFromObject(point1);
                sp1.AddPointWithConstraintExplicit(reference1, null, -1, 1, null, 0);
            }
            body1.InsertHybridShape(sp1);
        }
        public void AddPoint(List<Rhino.Geometry.Point3d> points, string bodyname)
        {

            MECMOD.PartDocument doc = (PartDocument)catia.ActiveDocument;
            Part part = doc.Part;
            HybridShapeFactory factory = (HybridShapeFactory)part.HybridShapeFactory;
            Body partbody = (Body)part.Bodies.GetItem(bodyname);
            for (int i = 0; i < points.Count; i++)
            {
                HybridShapePointCoord Point1 = factory.AddNewPointCoord(points[i].X, points[i].Y, points[i].Z);
                partbody.InsertHybridShape(Point1);
            }
        }
        public void AddLine(List<Rhino.Geometry.Line> lines, string bodyname)
        {
            MECMOD.PartDocument doc = (PartDocument)catia.ActiveDocument;
            Part part = doc.Part;
            HybridShapeFactory factory = (HybridShapeFactory)part.HybridShapeFactory;
            Body partbody = (Body)part.Bodies.GetItem(bodyname);

            for (int i = 0; i < lines.Count; i++)
            {
                HybridShapePointCoord Point1 = factory.AddNewPointCoord(lines[i].From.X, lines[i].From.Y, lines[i].From.Z);
                HybridShapePointCoord Point2 = factory.AddNewPointCoord(lines[i].To.X, lines[i].To.Y, lines[i].To.Z);
                HybridShapeLinePtPt line1 = factory.AddNewLinePtPt((INFITF.Reference)Point1, (INFITF.Reference)Point2);
                partbody.InsertHybridShape(line1);
            }
        }
        public void AddLine(List<Rhino.Geometry.Line> lines)
        {
            AddLine(lines, PartBody);
        }
        public void AddPoint(List<Rhino.Geometry.Point3d> points)
        {
            AddPoint(points, PartBody);
        }
        public void AddPowerCopyNPoint(string refname, string refpath, string[] inputpt, string[] refpoint)
        {
            MECMOD.PartDocument doc = (PartDocument)catia.ActiveDocument;
            Part part = doc.Part;
            InstanceFactory factory = (InstanceFactory)part.GetCustomerFactory("InstanceFactory");
            factory.BeginInstanceFactory(refname, refpath);
            factory.BeginInstantiate();
            for (int i = 0; i < inputpt.Length; i++)
            {
                CATBaseDispatch p1 = part.FindObjectByName(refpoint[i]);
                factory.PutInputData(inputpt[i], p1);
            }
            /*/-----------------------------------------------------------------
            Dim param1 As Parameter
            Set param1 = factory.GetParameter("Radius1")
            param1.ValuateFromString("25mm")
            catia.SystemService.Print("Modify Parameters");
                */
            factory.Instantiate();
            factory.EndInstantiate();
            factory.EndInstanceFactory();//release file
            part.Update();
        }
    }
}
