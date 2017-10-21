using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

using System;

public class SphereGenerator
{
    private static ushort GetMiddlePoint(ushort a, ushort b,
                                         MeshTool geometry,
                                         float radius, 
                                         Dictionary<int, ushort> edges)
    {
        ushort i;

        int min = a < b ? a : b;
        int max = a < b ? b : a;
        int key = (min << 16) | max; 
        
        if (!edges.TryGetValue(key, out i))
        {
            Vector3 middle = FasterMath.Mul(FasterMath.Add(geometry.positions[a], 
                                                           geometry.positions[b]), 
                                            0.5f);
            middle.Normalize();

            i = (ushort) geometry.ActiveVertices;
            geometry.ActiveVertices += 1;

            geometry.positions[i] = FasterMath.Mul(middle, radius);
            geometry.normals[i] = middle;

            edges[key] = i;
        }

        return i;
    }

    private enum Solid
    {
        Tetrahedron,
        Octahedron,
        Icosohedron,
    }

    private class Geodesic
    {
        public Solid Base;
        public int Subdivisions;
        public int VertexCount;
        public int FaceCount;
        public float Correction;

        public Geodesic(Solid @base, 
                        int subdivisions, 
                        int vertexCount, 
                        int faceCount,
                        float correction = 1f)
        {
            Base = @base;
            Subdivisions = subdivisions;
            VertexCount = vertexCount;
            FaceCount = faceCount;
            Correction = correction;
        }

        public override string ToString()
        {
            return string.Concat("Geodesic(", Base, ", ", Subdivisions, ", ", VertexCount, ", ", FaceCount, ")");
        }
    }

    private static List<Geodesic> geodesics = new List<Geodesic>
    {
        new Geodesic(Solid.Tetrahedron, 0,   4,   4, 1.62f), // 6
        new Geodesic(Solid.Octahedron,  0,   6,   8, 1.402f), // 12
        new Geodesic(Solid.Tetrahedron, 1,  10,  16, 1.145f), // 24
        new Geodesic(Solid.Icosohedron, 0,  12,  20, 1.125f), // 30
        new Geodesic(Solid.Octahedron,  1,  18,  32, 1.085f), // 48
        new Geodesic(Solid.Tetrahedron, 2,  34,  64, 1.045f), // 96
        new Geodesic(Solid.Icosohedron, 1,  42,  80, 1.025f), // 120
        new Geodesic(Solid.Octahedron,  2,  66, 128, 1.015f), // 192
        new Geodesic(Solid.Tetrahedron, 3, 130, 256, 1.015f), // 384
        new Geodesic(Solid.Icosohedron, 2, 162, 320, 1.015f), // 480
        new Geodesic(Solid.Octahedron,  3, 258, 512, 1.005f), // 768
        new Geodesic(Solid.Tetrahedron, 4, 514,1024, 1.00f), // 1536
        new Geodesic(Solid.Icosohedron, 3, 642,1280, 1.00f), // 1920
    };

    #region Primitive Data
    private static float tetrahedronFactor = 1f / Mathf.Sqrt(2);
    private static Vector3[] tetrahedronVertices = new Vector3[4];
    private static int[] tetrahedronIndices = new[]
    {
        0, 1, 2,
        0, 2, 3,
        1, 3, 2,
        0, 3, 1,
    };
    
    private static Vector3[] octahedronVertices = new Vector3[6];
    private static int[] octahedronIndices = new[]
    {
        0, 1, 2,
        3, 2, 1,
        2, 3, 4,
        0, 5, 1,
        3, 1, 5,
        0, 4, 5,
        4, 3, 5,
        0, 2, 4,
    };

    private static float icosahedronFactor = (1f + Mathf.Sqrt(5f)) / 2f;
    private static Vector3[] icosahedronVertices = new Vector3[12];
    private static int[] icosahedronIndices = new[]
    {
         0, 11,  5,
         0,  5,  1,
         0,  1,  7,
         0,  7, 10,
         0, 10, 11,

         1,  5,  9,
         5, 11,  4,
        11, 10,  2,
        10,  7,  6,
         7,  1,  8,

         3,  9,  4,
         3,  4,  2,
         3,  2,  6,
         3,  6,  8,
         3,  8,  9,

         4,  9,  5,
         2,  4, 11,
         6,  2, 10,
         8,  6,  7,
         9,  8,  1,
    };

    static SphereGenerator()
    {
        float t;

        t = tetrahedronFactor;
        tetrahedronVertices[0] = new Vector3( 1,  0, -t).normalized;
        tetrahedronVertices[1] = new Vector3(-1,  0, -t).normalized;
        tetrahedronVertices[2] = new Vector3( 0,  1,  t).normalized;
        tetrahedronVertices[3] = new Vector3( 0, -1,  t).normalized;

        octahedronVertices[0] = new Vector3( 0,  0,  1);
        octahedronVertices[1] = new Vector3(-1,  0,  0);
        octahedronVertices[2] = new Vector3( 0, -1,  0);
        octahedronVertices[3] = new Vector3( 0,  0, -1);
        octahedronVertices[4] = new Vector3( 1,  0,  0);
        octahedronVertices[5] = new Vector3( 0,  1,  0);

        t = icosahedronFactor;
        icosahedronVertices[ 0] = new Vector3(-1,  t,  0).normalized;
        icosahedronVertices[ 1] = new Vector3( 1,  t,  0).normalized;
        icosahedronVertices[ 2] = new Vector3(-1, -t,  0).normalized;
        icosahedronVertices[ 3] = new Vector3( 1, -t,  0).normalized;

        icosahedronVertices[ 4] = new Vector3( 0, -1,  t).normalized;
        icosahedronVertices[ 5] = new Vector3( 0,  1,  t).normalized;
        icosahedronVertices[ 6] = new Vector3( 0, -1, -t).normalized;
        icosahedronVertices[ 7] = new Vector3( 0,  1, -t).normalized;

        icosahedronVertices[ 8] = new Vector3( t,  0, -1).normalized;
        icosahedronVertices[ 9] = new Vector3( t,  0,  1).normalized;
        icosahedronVertices[10] = new Vector3(-t,  0, -1).normalized;
        icosahedronVertices[11] = new Vector3(-t,  0,  1).normalized;
    }
    #endregion

    public static void Tetrahedron(MeshTool geometry, float radius)
    {
        Array.Copy(tetrahedronIndices, geometry.indices, tetrahedronIndices.Length);

        for (int i = 0; i < tetrahedronVertices.Length; ++i)
        {
            geometry.positions[i] = tetrahedronVertices[i] * radius;
            geometry.normals[i] = tetrahedronVertices[i];
        }

        geometry.ActiveVertices = tetrahedronVertices.Length;
        geometry.ActiveIndices = tetrahedronIndices.Length;
    }

    public static void Octahedron(MeshTool geometry, float radius)
    {
        Array.Copy(octahedronIndices, geometry.indices, octahedronIndices.Length);

        for (int i = 0; i < octahedronVertices.Length; ++i)
        {
            geometry.positions[i] = octahedronVertices[i] * radius;
            geometry.normals[i] = octahedronVertices[i];
        }

        geometry.ActiveVertices = octahedronVertices.Length;
        geometry.ActiveIndices = octahedronIndices.Length;
    }

    public static void Icosohedron(MeshTool geometry, float radius)
    {
        Array.Copy(icosahedronIndices, geometry.indices, icosahedronIndices.Length);

        for (int i = 0; i < icosahedronVertices.Length; ++i)
        {
            geometry.positions[i] = icosahedronVertices[i] * radius;
            geometry.normals[i] = icosahedronVertices[i];
        }

        geometry.ActiveVertices = icosahedronVertices.Length;
        geometry.ActiveIndices = icosahedronIndices.Length;
    }

    public static void Sphere(MeshTool geometry, 
                              float radius,
                              int level,
                              bool correction)
    {
        var geodesic = geodesics[Mathf.Min(level, geodesics.Count - 1)];
        geometry.Topology = MeshTopology.Triangles;
        geometry.SetVertexCount(geodesic.VertexCount);
        geometry.SetIndexCount(geodesic.FaceCount * 3, lazy: true);

        if (correction)
        {
            radius *= geodesic.Correction;
        }

        if (geodesic.Base == Solid.Tetrahedron) Tetrahedron(geometry, radius);
        if (geodesic.Base == Solid.Octahedron)  Octahedron(geometry, radius);
        if (geodesic.Base == Solid.Icosohedron) Icosohedron(geometry, radius);

        int prevTriCount = geometry.ActiveIndices / 3;
        int nextTriCount = 0;

        // only need as much edge memory as the last iteration, which adds an six
        // edges per triangle of the previous iteration
        int penulTriCount = geodesic.FaceCount / 4;
        var edges = new Dictionary<int, ushort>(penulTriCount * 6);

        for (int i = 0; i < geodesic.Subdivisions; i++)
        {
            nextTriCount = prevTriCount * 4;

            for (int j = 0; j < prevTriCount; ++j)
            {
                var tri = geometry.GetTriangle(j);
                
                int a = GetMiddlePoint((ushort) tri.x, (ushort) tri.y, geometry, radius, edges);
                int b = GetMiddlePoint((ushort) tri.y, (ushort) tri.z, geometry, radius, edges);
                int c = GetMiddlePoint((ushort) tri.z, (ushort) tri.x, geometry, radius, edges);

                geometry.SetTriangle(prevTriCount + j * 3 + 0, tri.x, a, c);
                geometry.SetTriangle(prevTriCount + j * 3 + 1, tri.y, b, a);
                geometry.SetTriangle(prevTriCount + j * 3 + 2, tri.z, c, b);
                geometry.SetTriangle(j, a, b, c);
            }

            prevTriCount = nextTriCount;
            edges.Clear();
        }

        geometry.mesh.Clear();
        geometry.Apply(positions: true, normals: true, indices: true);
    }

    public static void CullSphereToHemisphere(MeshTool geometry)
    {
        int lastTri = geometry.indices.Length / 3 - 1;
        var vertUse = new byte[geometry.VertexCount]; 

        for (int i = 0; i <= lastTri; ++i)
        {
            var triangle = geometry.GetTriangle(i);

            Vector3 a = geometry.positions[triangle.x];
            Vector3 b = geometry.positions[triangle.y];
            Vector3 c = geometry.positions[triangle.z];

            if (Vector3.Cross(b - a, c - a).y < 0)
            {
                geometry.SwapTriangle(i, lastTri);
                lastTri -= 1;
                i -= 1;
            }
            else
            {
                vertUse[triangle.x] += 1;
                vertUse[triangle.y] += 1;
                vertUse[triangle.z] += 1;
            }
        }

        int lastVert = geometry.VertexCount - 1;

        for (int i = 0; i <= lastVert; ++i)
        {
            if (vertUse[i] == 0)
            {
                geometry.SwapVertex(i, lastVert, indices: true);
                Swap(ref vertUse[i], ref vertUse[lastVert]);
                lastVert -= 1;
                i -= 1;
            }
        }

        geometry.SetVertexCount(lastVert + 1);
        geometry.SetIndexCount((lastTri + 1) * 3, preserve: true, lazy: false);

        geometry.mesh.Clear();
        geometry.Apply(positions: true, normals: true, indices: true);
    }

    public static void Swap<T>(ref T a, ref T b)
    {
        var t = a;
        a = b;
        b = t;
    }

    public static void Hemisphere(MeshTool geometry,
                                  float radius,
                                  int complexity,
                                  bool correction)
    {
        Sphere(geometry, radius, complexity, correction);
        CullSphereToHemisphere(geometry);
    }

    public static void HemisphereOld(MeshTool geometry,
                                     float radius,
                                     int complexity,
                                     bool correction)
    {
        int subdivisions = complexity / 6;
        int sides = (complexity % 6) + 3; 

        if (correction)
        {
            // try to make the average of apoth/radius = desired radius
            // * desired = (radius + apoth) / 2
            // apoth = radius * cos(pi / sides)
            // * desired = (radius + radius * cos(pi / sides)) / 2
            // * desired = radius * (1 + cos(pi / sides)) / 2
            // rearrange to determine radius from desired radius
            // * radius = desired / ((1 + cos(pi / sides)) / 2)
            
            radius = radius * 2 / (1 + Mathf.Cos(Mathf.PI / (sides * (subdivisions + 1))));
        }

        // pyramid
        int vertCount = sides + 1;
        int faceCount = sides;
        int edgeCount = sides * 2;

        for (int i = 0; i < subdivisions; ++i)
        {
            vertCount += edgeCount;
            edgeCount += faceCount * 6;
            faceCount *= 4;
        }

        geometry.SetVertexCount(vertCount);
        geometry.SetIndexCount(faceCount * 3, lazy: true);

        geometry.ActiveVertices = sides + 1;
        geometry.ActiveIndices = sides * 3;

        geometry.positions[0] = new Vector3(0, radius, 0);
        geometry.normals[0] = new Vector3(0, 1, 0);

        float da = Mathf.PI * 2 / sides;

        for (int i = 1; i < vertCount; ++i)
        {
            var normal = new Vector3(Mathf.Cos(da * i), 0, Mathf.Sin(da * i));

            geometry.positions[i] = FasterMath.Mul(normal, radius);
            geometry.normals[i] = normal;
        }

        for (int i = 1; i < sides; ++i)
        {
            geometry.SetTriangle(i, 0, i + 1, i);
        }

        geometry.SetTriangle(0, 0, 1, sides);

        int prevTriCount = geometry.ActiveIndices / 3;
        int nextTriCount = 0;

        var edges = new Dictionary<int, ushort>(prevTriCount * 6);

        for (int i = 0; i < subdivisions; i++)
        {
            nextTriCount = prevTriCount * 4;

            for (int j = 0; j < prevTriCount; ++j)
            {
                var tri = geometry.GetTriangle(j);

                int a = GetMiddlePoint((ushort) tri.x, (ushort) tri.y, geometry, radius, edges);
                int b = GetMiddlePoint((ushort) tri.y, (ushort) tri.z, geometry, radius, edges);
                int c = GetMiddlePoint((ushort) tri.z, (ushort) tri.x, geometry, radius, edges);

                geometry.SetTriangle(prevTriCount + j * 3 + 0, tri.x, a, c);
                geometry.SetTriangle(prevTriCount + j * 3 + 1, tri.y, b, a);
                geometry.SetTriangle(prevTriCount + j * 3 + 2, tri.z, c, b);
                geometry.SetTriangle(j, a, b, c);
            }

            prevTriCount = nextTriCount;
            edges.Clear();
        }

        geometry.mesh.Clear();
        geometry.Apply(positions: true, normals: true, indices: true);
    }
}
