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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Vector3d = Rhino.Geometry.Vector3d;

namespace MaterialTools
{
    class OpenTKWindow : GameWindow
    {
      public   float[] LightAmbient = { 0.5f, 0.5f, 0.5f, 1.0f }; // 环境光参数
      public   float[] LightDiffuse = { 1.0f, 1.0f, 1.0f, 1.0f }; // 漫射光参数
      public   float[] LightPosition = { 0.0f, 0.0f, 2.0f, 1.0f };
      public   Mesh mesh = new Mesh();
        public OpenTKWindow(): base(){ }
        public OpenTKWindow(int width, int length): base(width,length) { }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color.Gray);// 背景
            GL.Enable(EnableCap.DepthTest);

            GL.Light(LightName.Light0, LightParameter.Ambient, LightAmbient);//设置系统灯光
            GL.Light(LightName.Light0, LightParameter.Diffuse, LightDiffuse); // 设置漫射光
            GL.Light(LightName.Light0, LightParameter.Position, LightPosition); // 设置光源位置
            GL.Enable(EnableCap.Light0);//启用灯光

            /*
            GL.Enable(EnableCap.CullFace);//反面不可见         
            GL.Enable(EnableCap.Texture2D); // 启用纹理映射
            GL.ShadeModel(ShadingModel.Smooth); // 启用阴影平滑
            GL.ClearDepth(1.0f); // 设置深度缓存
            GL.Enable(EnableCap.DepthTest); // 启用深度测试
            GL.DepthFunc(DepthFunction.Lequal); // 所作深度测试的类型
            GL.Hint(HintTarget.PerspectiveCorrectionHint,HintMode.Nicest);// 告诉系统对透视进行修正
            GL.Color4(1.0f, 1.0f, 1.0f, 0.5f); // 全亮度， 50% Alpha 混合
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
            */
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Keyboard[OpenTK.Input.Key.Escape])
            {
                this.Exit();
                return;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            double aspect_ratio = Width / (double)Height;
            Matrix4 perspective =Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 12000);//设置视野最大最小距离
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 LookAt = Matrix4.LookAt(Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref LookAt);
            DrawMatrix();
            Drawface(mesh);Drawline(mesh);
            this.SwapBuffers();
        }
        public void DrawMatrix()
        {
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
        public void Drawline(Mesh x)
        {
            GL.PushMatrix();
            Rhino.Geometry.Collections.MeshTopologyEdgeList el = x.TopologyEdges;
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Black);
            for (int i = 0; i < el.Count; i++)
            {
                Line l1 = el.EdgeLine(i);
                Point3d p1 = l1.From, p2 = l1.To;
                GL.Vertex3((float)p1.X, (float)p1.Z, (float)-p1.Y);
                GL.Vertex3((float)p2.X, (float)p2.Z, (float)-p2.Y);
            }
            GL.End();
            GL.PopMatrix();
        }
        public void Drawface(Mesh x)
        {
            GL.PushMatrix();
            if (x.VertexColors.Count != x.Vertices.Count)
            {
                Color color1 = Color.Green;
                GL.Begin(PrimitiveType.Triangles);
                for (int i = 0; i < x.Faces.Count; i++)
                {
                    MeshFace f = x.Faces[i];
                    if (f.IsTriangle)
                    {
                        Point3f p1 = x.Vertices[f.A]; Point3f p2 = x.Vertices[f.B]; Point3f p3 = x.Vertices[f.C];
                        GL.Color3(color1);
                        GL.Vertex3(p1.X, p1.Z, -p1.Y);
                        GL.Color3(color1);
                        GL.Vertex3(p2.X, p2.Z, -p2.Y);
                        GL.Color3(color1);
                        GL.Vertex3(p3.X, p3.Z, -p3.Y);
                    }
                }
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                for (int i = 0; i < x.Faces.Count; i++)
                {
                    MeshFace f = x.Faces[i];
                    if (f.IsQuad)
                    {
                        Point3f p1 = x.Vertices[f.A]; Point3f p2 = x.Vertices[f.B];
                        Point3f p3 = x.Vertices[f.C]; Point3f p4 = x.Vertices[f.D];
                        GL.Color3(color1);
                        GL.Vertex3(p1.X, p1.Z, -p1.Y);
                        GL.Color3(color1);
                        GL.Vertex3(p2.X, p2.Z, -p2.Y);
                        GL.Color3(color1);
                        GL.Vertex3(p3.X, p3.Z, -p3.Y);
                        GL.Color3(color1);
                        GL.Vertex3(p4.X, p4.Z, -p4.Y);
                    }
                }
                GL.End();
            }
            else
            {
                GL.Begin(PrimitiveType.Triangles);
                for (int i = 0; i < x.Faces.Count; i++)
                {
                    MeshFace f = x.Faces[i];
                    if (f.IsTriangle)
                    {
                        Point3f p1 = x.Vertices[f.A]; Point3f p2 = x.Vertices[f.B]; Point3f p3 = x.Vertices[f.C];
                        GL.Color3(x.VertexColors[f.A]);
                        GL.Vertex3(p1.X, p1.Z, -p1.Y);
                        GL.Color3(x.VertexColors[f.B]);
                        GL.Vertex3(p2.X, p2.Z, -p2.Y);
                        GL.Color3(x.VertexColors[f.C]);
                        GL.Vertex3(p3.X, p3.Z, -p3.Y);
                    }
                }
                GL.End();
                GL.Begin(PrimitiveType.Quads);
                for (int i = 0; i < x.Faces.Count; i++)
                {
                    MeshFace f = x.Faces[i];
                    if (f.IsQuad)
                    {
                        Point3f p1 = x.Vertices[f.A]; Point3f p2 = x.Vertices[f.B];
                        Point3f p3 = x.Vertices[f.C]; Point3f p4 = x.Vertices[f.D];
                        GL.Color3(x.VertexColors[f.A]);
                        GL.Vertex3(p1.X, p1.Z, -p1.Y);
                        GL.Color3(x.VertexColors[f.B]);
                        GL.Vertex3(p2.X, p2.Z, -p2.Y);
                        GL.Color3(x.VertexColors[f.C]);
                        GL.Vertex3(p3.X, p3.Z, -p3.Y);
                        GL.Color3(x.VertexColors[f.D]);
                        GL.Vertex3(p4.X, p4.Z, -p4.Y);
                    }
                }
                GL.End();
            }
            GL.PopMatrix();
        }
    }
}
