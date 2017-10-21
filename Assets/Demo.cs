using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class Demo : MonoBehaviour 
{
    public MeshFilter filter;
    private Mesh mesh;
    private MeshTool tool;

    [Header("Config")]
    [Range(0, 12)]
    public int level;
    public bool correction;

    private void Awake()
    {
        mesh = new Mesh();
        tool = new MeshTool(mesh, MeshTopology.Triangles);
        filter.sharedMesh = mesh;
    }

    public void Refresh()
    {
        //SphereGenerator.Sphere(tool, 0.5f, level, correction);
        SphereGenerator.Hemisphere(tool, 0.5f, level, correction);
        //transform.localRotation = Random.rotationUniform;
    }

    public void Update()
    {
        Refresh();
    }
}
