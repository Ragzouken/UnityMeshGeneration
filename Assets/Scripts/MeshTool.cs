using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MeshTool
{
    public Mesh mesh;
    public List<Vector3> positions = new List<Vector3>();
    public List<Vector3> normals = new List<Vector3>();
    public List<Color32> colors = new List<Color32>();
    public List<Vector4> uv0s = new List<Vector4>();
    public List<Vector4> uv1s = new List<Vector4>();
    public int[] indices = new int[0];

    public MeshTopology Topology;
    public int VertexCount { get; private set; }

    public int ActiveVertices;
    public int ActiveIndices;

    public MeshTool(Mesh mesh = null,
                    MeshTopology topology = MeshTopology.Triangles)
    {
        this.mesh = mesh ?? new Mesh();

        Topology = topology;
    }

    public void SetVertexCount(int count)
    {
        VertexCount = count;

        positions.Resize(count);
        normals.Resize(count);
        colors.Resize(count);
        uv0s.Resize(count);
        uv1s.Resize(count);
    }

    public void SetIndexCount(int count, 
                              bool lazy=true,
                              bool preserve=false)
    {
        if (indices.Length < count || !lazy)
        {
            var prev = indices;
            indices = new int[count];

            if (preserve)
            {
                System.Array.Copy(prev, indices, Mathf.Min(prev.Length, indices.Length));
            }
        }
        else if (indices.Length > count)
        {
            for (int i = count; i < indices.Length; ++i)
            {
                indices[i] = 0;
            }
        }
    }

    public void Apply(bool positions = false,
                      bool normals = false,
                      bool colors = false,
                      bool uv0s = false,
                      bool uv1s = false,
                      bool indices = false,
                      bool autoNormals = false)
    {
        if (positions) mesh.SetVertices(this.positions);
        if (normals) mesh.SetNormals(this.normals);
        if (colors) mesh.SetColors(this.colors);
        if (uv0s) mesh.SetUVs(0, this.uv0s);
        if (uv1s) mesh.SetUVs(1, this.uv1s);
        if (indices) mesh.SetIndices(this.indices, Topology, 0, false);

        if (positions || indices)
        {
            mesh.RecalculateBounds();
        }

        if (autoNormals)
        {
            mesh.RecalculateNormals();
        }
    }

    public void SetTriangle(int index, IntVector3 triangle)
    {
        SetTriangle(index, triangle.x, triangle.y, triangle.z);
    }

    public void SetTriangle(int index, int v0, int v1, int v2)
    {
        Assert.IsTrue(Topology == MeshTopology.Triangles, "This mesh doesn't use Triangles topology!");
        Assert.IsTrue(indices.Length >= index * 3 + 2, "This mesh doesn't have enough indices!");

        indices.SetTriangle(index, v0, v1, v2);
    }

    public IntVector3 GetTriangle(int index)
    {
        Assert.IsTrue(Topology == MeshTopology.Triangles, "This mesh doesn't use Triangles topology!");
        Assert.IsTrue(indices.Length >= index * 3 + 2, "This mesh doesn't have enough indices!");

        return indices.GetTriangle(index);
    }

    public void SwapTriangle(int a, int b)
    {
        var ta = GetTriangle(a);
        var tb = GetTriangle(b);
        SetTriangle(a, tb);
        SetTriangle(b, ta);
    }

    public void SwapVertex(int a, int b)
    {
        Swap(positions, a, b);
        Swap(normals, a, b);
        Swap(colors, a, b);
        Swap(uv0s, a, b);
        Swap(uv1s, a, b);

        for (int i = 0; i < indices.Length; ++i)
        {
            if (indices[i] == a)
            {
                indices[i] = b;
            }
            else if (indices[i] == b)
            {
                indices[i] = a;
            }
        }
    }

    public static void Swap<T>(List<T> list, int a, int b)
    {
        var t = list[a];
        list[a] = list[b];
        list[b] = t;
    }
}

public static partial class Extensions
{
    public static void SetTriangle(this int[] indices, 
                                   int index, 
                                   int v0, int v1, int v2)
    {
        indices[index * 3 + 0] = v0;
        indices[index * 3 + 1] = v1;
        indices[index * 3 + 2] = v2;
    }

    public static IntVector3 GetTriangle(this int[] indices, int index)
    {
        return new IntVector3(indices[index * 3 + 0],
                              indices[index * 3 + 1],
                              indices[index * 3 + 2]);
    }
}
