﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flummery
{
    
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 UV;
        public Color4 Colour;

        public static readonly int Stride = Marshal.SizeOf(default(Vertex));

        public Vertex Clone()
        {
            Vertex v = new Vertex();

            v.Position = this.Position;
            v.Normal = this.Normal;
            v.UV = this.UV;
            v.Colour = this.Colour;

            return v;
        }
    }

    public sealed class VertexBuffer
    {
        List<Vertex> data = new List<Vertex>();
        int vbo;
        bool Initialised = false;

        public int Length { get { return (data != null ? data.Count : 0); } }
        public List<Vertex> Data { get { return data; } }

        public int AddVertex(Vertex v)
        {
            data.Add(v);
            return data.Count - 1;
        }

        public void ModifyVertexPosition(int index, Vector3 position)
        {
            var v = data[index];
            v.Position = position;
            data[index] = v;
        }

        public void ModifyVertexNormal(int index, Vector3 normal)
        {
            var v = data[index];
            v.Normal = normal;
            data[index] = v;
        }

        public void ModifyVertexUVs(int index, Vector4 uv)
        {
            var v = data[index];
            v.UV = uv;
            data[index] = v;
        }

        public void ModifyVertexColour(int index, Color4 colour)
        {
            var v = data[index];
            v.Colour = colour;
            data[index] = v;
        }

        public void Initialise(List<Vertex> data = null)
        {
            if (data != null) { this.data = data; }

            GL.GenBuffers(1, out vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(this.data.Count * Vertex.Stride), this.data.ToArray(), BufferUsageHint.DynamicDraw);
        }

        public void Draw(IndexBuffer ibo, PrimitiveType primitiveType)
        {

            if (vbo == 0 && !Initialised)
            {
                Initialise();
                Initialised = true;
            }
            ibo.Draw();

            bool bWireframe = (SceneManager.Current.RenderMode == SceneManager.RenderMeshMode.Wireframe);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            if (!bWireframe) { GL.EnableClientState(ArrayCap.ColorArray); }

            GL.VertexPointer(3, VertexPointerType.Float, Vertex.Stride, new IntPtr(0));
            GL.NormalPointer(NormalPointerType.Float, Vertex.Stride, new IntPtr(Vector3.SizeInBytes));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Stride, new IntPtr(2 * Vector3.SizeInBytes));
            if (!bWireframe) { GL.ColorPointer(4, ColorPointerType.Float, Vertex.Stride, new IntPtr((2 * Vector3.SizeInBytes) + Vector4.SizeInBytes)); }

            GL.DrawElements(primitiveType, ibo.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            if (!bWireframe) { GL.DisableClientState(ArrayCap.ColorArray);}

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
