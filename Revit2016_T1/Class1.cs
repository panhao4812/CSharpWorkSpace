using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using System.Runtime.InteropServices;
using Autodesk.Revit.DB.Structure;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
[Journaling(JournalingMode.NoCommandData)]

public class Lab1PlaceGroup : IExternalCommand
{
    public string output = "Wall : ";
    public Result Execute(
        ExternalCommandData commandData, 
        ref string message,
        ElementSet elements)
    {
        initConsole();
         Print("StartCommond"); 
        UIApplication uiApp = commandData.Application;
        Document doc = uiApp.ActiveUIDocument.Document;
        Selection sel = uiApp.ActiveUIDocument.Selection;     
        List<ElementId> idList= new List<ElementId>(sel.GetElementIds());
        Element element;
        if (idList.Count > 0)
        {
            element = doc.GetElement(idList[0]);
        }
        else
        {
            Reference reference = sel.PickObject(ObjectType.Element);
            element = doc.GetElement(reference);
        }

        try { 
        Wall wall = element as Wall;
            GetInfo_Wall(wall);
        }

        catch(Exception ex) {
            Print(output);
            Print(ex.ToString());           
            return Result.Failed;
        }
        Print(output);
        return Result.Succeeded;
    }
    public void GetInfo_Wall(Wall wall)
    {
        
        AnalyticalModel model = wall.GetAnalyticalModel();
        if (null != model)
        {
            output += "\nWall AnalyticalModel type is : " + model;
        }
        //   wall.Flip();
        output += "\nIf wall Flipped : " + wall.Flipped;
        output += "\nWall orientation point is :(" + wall.Orientation.X + ", "
            + wall.Orientation.Y + ", " + wall.Orientation.Z + ")";
        output += "\nWall StructuralUsage is : " + wall.StructuralUsage;
        output += "\nWall type name is : " + wall.WallType.Name;
        output += "\nWall width is : " + wall.Width;       
    }


    public void initConsole()
    {
        AllocConsole();
        Console.WindowWidth = 136;
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Clear();
        
    }
    public void Print(string str)
    {
        Console.WriteLine(str);
    }
    [DllImport("kernel32.dll")]
    public static extern bool AllocConsole();
  
    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();
}
