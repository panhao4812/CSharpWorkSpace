using HybridShapeTypeLib;
using INFITF;
using MECMOD;
using System.Collections.Generic;

namespace ToCatia1
{
    public class ToCatiaAutomation
    {
        public static string PartBody = "PartBody";
        public ToCatiaAutomation()
        {
        }
        public static void AddPoint(List<Rhino.Geometry.Point3d> points, string bodyname)
        {
            System.Type catiaType = System.Type.GetTypeFromProgID("CATIA.Application");
            INFITF.Application catia = (INFITF.Application)System.Activator.CreateInstance(catiaType);
            catia.Visible = true;
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
        public static void AddLine(List<Rhino.Geometry.Line> lines, string bodyname)
        {
            System.Type catiaType = System.Type.GetTypeFromProgID("CATIA.Application");
            INFITF.Application catia = (INFITF.Application)System.Activator.CreateInstance(catiaType);
            catia.Visible = true;
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
        public static void AddLine(List<Rhino.Geometry.Line> lines)
        {
            AddLine(lines, PartBody);
        }
        public static void AddPoint(List<Rhino.Geometry.Point3d> points)
        {
            AddPoint(points, PartBody);
        }
        public static void AddPowerCopyNPoint(string refname,string refpath,string[] inputpt, string[] refpoint)
        {
            System.Type catiaType = System.Type.GetTypeFromProgID("CATIA.Application");
            INFITF.Application catia = (INFITF.Application)System.Activator.CreateInstance(catiaType);
            catia.Visible = true;
            MECMOD.PartDocument doc = (PartDocument)catia.ActiveDocument;
            Part part = doc.Part;
            InstanceFactory factory = (InstanceFactory)part.GetCustomerFactory("InstanceFactory");       
            factory.BeginInstanceFactory(refname, refpath);
            factory.BeginInstantiate();
            for(int i = 0; i < inputpt.Length; i++) { 
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
