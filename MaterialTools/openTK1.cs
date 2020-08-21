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


using OpenTK;
using OpenTK.Input;
using OpenTK.Platform;
using OpenTK.Graphics;
using OpenTK.Platform.MacOS;
using OpenTK.Platform.Windows;
using OpenTK.Platform.X11;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.ES10;
using OpenTK.Graphics.ES11;
using OpenTK.Graphics.ES20;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.ES30;
using OpenTK.Graphics.OpenGL4;



/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public class Script_Instance1 : GH_ScriptInstance
{
#region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { __out.Add(text); }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { __out.Add(string.Format(format, args)); }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj)); }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj, method_name)); }
#endregion

#region Members
  /// <summary>Gets the current Rhino document.</summary>
  private RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private IGH_Component Component; 
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private int Iteration;
#endregion

  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments, 
  /// Output parameters as ref arguments. You don't have to assign output parameters, 
  /// they will have a default value.
  /// </summary>
  private void RunScript(Mesh Profile, Mesh x, bool y, ref object A)
  {
    
    if(y){
      try{
        using (var game = new GameWindow(Width, Height, GraphicsMode.Default))
        {
          game.Load += (sender, e) =>
            {
            OpenTK.Graphics.OpenGL.GL.ClearColor(Color.Gray);// 背景
            OpenTK.Graphics.OpenGL.GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);
            // OpenTK.Graphics.OpenGL.GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.CullFace);//反面不可见
            OpenTK.Graphics.OpenGL.GL.Light(OpenTK.Graphics.OpenGL.LightName.Light0,
              OpenTK.Graphics.OpenGL.LightParameter.Ambient, LightAmbient);//设置系统灯光
            OpenTK.Graphics.OpenGL.GL.Light(OpenTK.Graphics.OpenGL.LightName.Light0,
              OpenTK.Graphics.OpenGL.LightParameter.Diffuse, LightDiffuse);		// 设置漫射光
            OpenTK.Graphics.OpenGL.GL.Light(OpenTK.Graphics.OpenGL.LightName.Light0,
              OpenTK.Graphics.OpenGL.LightParameter.Position, LightPosition);	// 设置光源位置
            OpenTK.Graphics.OpenGL.GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Light0);//启用灯光

            // OpenTK.Graphics.OpenGL.GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);							// 启用纹理映射
            // OpenTK.Graphics.OpenGL.GL.ShadeModel(OpenTK.Graphics.OpenGL.ShadingModel.Smooth);							// 启用阴影平滑

            // OpenTK.Graphics.OpenGL.GL.ClearDepth(1.0f);									// 设置深度缓存
            //OpenTK.Graphics.OpenGL.GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);								// 启用深度测试
            // OpenTK.Graphics.OpenGL.GL.DepthFunc(OpenTK.Graphics.OpenGL.DepthFunction.Lequal);							// 所作深度测试的类型
            // OpenTK.Graphics.OpenGL.GL.Hint(OpenTK.Graphics.OpenGL.HintTarget.PerspectiveCorrectionHint,
            //   OpenTK.Graphics.OpenGL.HintMode.Nicest);// 告诉系统对透视进行修正
            //  OpenTK.Graphics.OpenGL.GL.Color4(1.0f, 1.0f, 1.0f, 0.5f);						// 全亮度， 50% Alpha 混合
            //  OpenTK.Graphics.OpenGL.GL.BlendFunc(OpenTK.Graphics.OpenGL.BlendingFactorSrc.SrcAlpha, OpenTK.Graphics.OpenGL.BlendingFactorDest.One);


            v1 = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.CameraLocation;
            v2 = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.CameraUp;
            v3 = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.CameraTarget;
            };

          game.Resize += (sender, e) =>
            {
            OpenTK.Graphics.OpenGL.GL.Viewport(0, 0, Width, Height);
            double aspect_ratio = Width / (double) Height;
            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float) aspect_ratio, 1, 12000);//设置视野最大最小距离
            OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            OpenTK.Graphics.OpenGL.GL.LoadMatrix(ref perspective);

            };


          game.UpdateFrame += (sender, e) =>
            {
            if (game.Keyboard[Key.Escape]){ game.Exit();}
            };

          game.RenderFrame += (sender, e) =>
            {
            OpenTK.Graphics.OpenGL.GL.Clear
              (OpenTK.Graphics.OpenGL.ClearBufferMask.ColorBufferBit | OpenTK.Graphics.OpenGL.ClearBufferMask.DepthBufferBit);



            OpenTK.Matrix4 LookAt = OpenTK.Matrix4.LookAt(
              (float) v1.X, (float) v1.Z, -(float) v1.Y,
              (float) v3.X, (float) v3.Z, -(float) v3.Y,
              0, 1, 0
              );

            // Matrix4 modelview = Matrix4.LookAt(Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
            OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview0Ext);
            OpenTK.Graphics.OpenGL.GL.LoadMatrix(ref LookAt);

            angle = (float) ( (Width / 2 - game.Mouse.X) / (float) Width * 360f);


            OpenTK.Graphics.OpenGL.GL.Rotate(angle, OpenTK.Vector3d.UnitY);
            DrawMatrix();

            //OpenTK.Graphics.OpenGL.GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Lighting);
            Drawface(x);
            //OpenTK.Graphics.OpenGL.GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Lighting);
           // OpenTK.Graphics.OpenGL.GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.LineSmooth);
            // Drawface(Profile);
            Drawline(x);
            game.SwapBuffers();
            };
          // Run the game at 60 updates per second
          game.Run(60);
        }

      }catch(Exception ex){Print(ex.ToString());}
    }
  }

  // <Custom additional code> 
    int Width = 800,Height = 600;
  float angle = 0;
  float[]  LightAmbient = { 0.5f, 0.5f, 0.5f, 1f }; 	// 环境光参数
  float[] LightDiffuse = { 1.0f, 1.0f, 1.0f, 1.0f };		// 漫射光参数
  float[] LightPosition = { 0.0f, 0.0f, 20.0f, 10.0f };
  Point3d v1,v3;
  Rhino.Geometry.Vector3d v2;
  private void DrawMatrix(){
    OpenTK.Graphics.OpenGL.GL.PushMatrix();
    OpenTK.Graphics.OpenGL.GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.Lines);
    OpenTK.Graphics.OpenGL.GL.Color3(Color.Red);
    OpenTK.Graphics.OpenGL.GL.Vertex3(0, 0, 0);
    OpenTK.Graphics.OpenGL.GL.Color3(Color.Red);
    OpenTK.Graphics.OpenGL.GL.Vertex3(1000, 0, 0);
    OpenTK.Graphics.OpenGL.GL.Color3(Color.Green);
    OpenTK.Graphics.OpenGL.GL.Vertex3(0, 0, 0);
    OpenTK.Graphics.OpenGL.GL.Color3(Color.Green);
    OpenTK.Graphics.OpenGL.GL.Vertex3(0, 0, -1000);
    OpenTK.Graphics.OpenGL.GL.End();
    OpenTK.Graphics.OpenGL.GL.PopMatrix();
  }
  private void Drawline(Mesh x)
  {  OpenTK.Graphics.OpenGL.GL.PushMatrix();
    Rhino.Geometry.Collections.MeshTopologyEdgeList el = x.TopologyEdges;
    OpenTK.Graphics.OpenGL.GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.Lines);
    OpenTK.Graphics.OpenGL.GL.Color3(Color.Black);
    for(int i = 0;i < el.Count;i++){
      Line l1 = el.EdgeLine(i);
      Point3d p1=l1.From,p2 = l1.To;
      OpenTK.Graphics.OpenGL.GL.Vertex3((float) p1.X, (float) p1.Z, (float) - p1.Y);
      OpenTK.Graphics.OpenGL.GL.Vertex3((float) p2.X, (float) p2.Z, (float) - p2.Y);
    }
    OpenTK.Graphics.OpenGL.GL.End();
    OpenTK.Graphics.OpenGL.GL.PopMatrix();
  }
  private void Drawface(Mesh x)
  {  OpenTK.Graphics.OpenGL.GL.PushMatrix();
    if(x.VertexColors.Count != x.Vertices.Count){
      Color color1 = Color.Black;
      OpenTK.Graphics.OpenGL.GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
      for(int i = 0;i < x.Faces.Count;i++){
        MeshFace f = x.Faces[i];
        if(f.IsTriangle){
          Point3f p1 = x.Vertices[f.A];  Point3f p2 = x.Vertices[f.B];  Point3f p3 = x.Vertices[f.C];
          OpenTK.Graphics.OpenGL.GL.Color3(color1);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p1.X, p1.Z, -p1.Y);
          OpenTK.Graphics.OpenGL.GL.Color3(color1);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p2.X, p2.Z, -p2.Y);
          OpenTK.Graphics.OpenGL.GL.Color3(color1);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p3.X, p3.Z, -p3.Y);
        }
      }
      OpenTK.Graphics.OpenGL.GL.End();
      OpenTK.Graphics.OpenGL.GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.Quads);
      for(int i = 0;i < x.Faces.Count;i++){
        MeshFace f = x.Faces[i];
        if(f.IsQuad){
          Point3f p1 = x.Vertices[f.A];  Point3f p2 = x.Vertices[f.B];
          Point3f p3 = x.Vertices[f.C];   Point3f p4 = x.Vertices[f.D];
          OpenTK.Graphics.OpenGL.GL.Color3(color1);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p1.X, p1.Z, -p1.Y);
          OpenTK.Graphics.OpenGL.GL.Color3(color1);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p2.X, p2.Z, -p2.Y);
          OpenTK.Graphics.OpenGL.GL.Color3(color1);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p3.X, p3.Z, -p3.Y);
          OpenTK.Graphics.OpenGL.GL.Color3(color1);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p4.X, p4.Z, -p4.Y);
        }
      }
      OpenTK.Graphics.OpenGL.GL.End();
    }else{
      OpenTK.Graphics.OpenGL.GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
      for(int i = 0;i < x.Faces.Count;i++){
        MeshFace f = x.Faces[i];
        if(f.IsTriangle){
          Point3f p1 = x.Vertices[f.A];  Point3f p2 = x.Vertices[f.B];  Point3f p3 = x.Vertices[f.C];
          OpenTK.Graphics.OpenGL.GL.Color3(x.VertexColors[f.A]);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p1.X, p1.Z, -p1.Y);
          OpenTK.Graphics.OpenGL.GL.Color3(x.VertexColors[f.B]);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p2.X, p2.Z, -p2.Y);
          OpenTK.Graphics.OpenGL.GL.Color3(x.VertexColors[f.C]);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p3.X, p3.Z, -p3.Y);
        }
      }
      OpenTK.Graphics.OpenGL.GL.End();
      OpenTK.Graphics.OpenGL.GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.Quads);
      for(int i = 0;i < x.Faces.Count;i++){
        MeshFace f = x.Faces[i];
        if(f.IsQuad){
          Point3f p1 = x.Vertices[f.A];  Point3f p2 = x.Vertices[f.B];
          Point3f p3 = x.Vertices[f.C];   Point3f p4 = x.Vertices[f.D];
          OpenTK.Graphics.OpenGL.GL.Color3(x.VertexColors[f.A]);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p1.X, p1.Z, -p1.Y);
          OpenTK.Graphics.OpenGL.GL.Color3(x.VertexColors[f.B]);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p2.X, p2.Z, -p2.Y);
          OpenTK.Graphics.OpenGL.GL.Color3(x.VertexColors[f.C]);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p3.X, p3.Z, -p3.Y);
          OpenTK.Graphics.OpenGL.GL.Color3(x.VertexColors[f.D]);
          OpenTK.Graphics.OpenGL.GL.Vertex3(p4.X, p4.Z, -p4.Y);
        }
      }
      OpenTK.Graphics.OpenGL.GL.End();
      } OpenTK.Graphics.OpenGL.GL.PopMatrix();
  }


  // </Custom additional code> 

  private List<string> __err = new List<string>(); //Do not modify this list directly.
  private List<string> __out = new List<string>(); //Do not modify this list directly.
  private RhinoDoc doc = RhinoDoc.ActiveDoc;       //Legacy field.
  private IGH_ActiveObject owner;                  //Legacy field.
  private int runCount;                            //Legacy field.
  
  public override void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA)
  {
    //Prepare for a new run...
    //1. Reset lists
    this.__out.Clear();
    this.__err.Clear();

    this.Component = owner;
    this.Iteration = iteration;
    this.GrasshopperDocument = owner.OnPingDocument();
    this.RhinoDocument = rhinoDocument as Rhino.RhinoDoc;

    this.owner = this.Component;
    this.runCount = this.Iteration;
    this. doc = this.RhinoDocument;

    //2. Assign input parameters
        Mesh Profile = default(Mesh);
    if (inputs[0] != null)
    {
      Profile = (Mesh)(inputs[0]);
    }

    Mesh x = default(Mesh);
    if (inputs[1] != null)
    {
      x = (Mesh)(inputs[1]);
    }

    bool y = default(bool);
    if (inputs[2] != null)
    {
      y = (bool)(inputs[2]);
    }



    //3. Declare output parameters
      object A = null;


    //4. Invoke RunScript
    RunScript(Profile, x, y, ref A);
      
    try
    {
      //5. Assign output parameters to component...
            if (A != null)
      {
        if (GH_Format.TreatAsCollection(A))
        {
          IEnumerable __enum_A = (IEnumerable)(A);
          DA.SetDataList(1, __enum_A);
        }
        else
        {
          if (A is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(1, (Grasshopper.Kernel.Data.IGH_DataTree)(A));
          }
          else
          {
            //assign direct
            DA.SetData(1, A);
          }
        }
      }
      else
      {
        DA.SetData(1, null);
      }

    }
    catch (Exception ex)
    {
      this.__err.Add(string.Format("Script exception: {0}", ex.Message));
    }
    finally
    {
      //Add errors and messages... 
      if (owner.Params.Output.Count > 0)
      {
        if (owner.Params.Output[0] is Grasshopper.Kernel.Parameters.Param_String)
        {
          List<string> __errors_plus_messages = new List<string>();
          if (this.__err != null) { __errors_plus_messages.AddRange(this.__err); }
          if (this.__out != null) { __errors_plus_messages.AddRange(this.__out); }
          if (__errors_plus_messages.Count > 0) 
            DA.SetDataList(0, __errors_plus_messages);
        }
      }
    }
  }
}