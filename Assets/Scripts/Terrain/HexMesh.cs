using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes care of mesh-related functionalities (vertices, triangles, colors, UVs)
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    public bool useCollider, useColors, useUVCoordinates;
    
    Mesh hexMesh;
    [System.NonSerialized] List<Vector3> vertices;
    [System.NonSerialized] List<int> triangles;
    [System.NonSerialized] List<Color> colors;
    [System.NonSerialized] List<Vector2> uvs;

    MeshCollider meshCollider;
    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        if (useCollider)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        hexMesh.name = "Hex Mesh";
    }

    public void Clear()
    {
        hexMesh.Clear();
        vertices = ListPool<Vector3>.Get();
        if (useColors)
        {
            colors = ListPool<Color>.Get();
        }
        if (useUVCoordinates)
        {
            uvs = ListPool<Vector2>.Get();
        }
        triangles = ListPool<int>.Get();
    }

    public void Apply()
    {
        hexMesh.SetVertices(vertices);
        ListPool<Vector3>.Add(vertices);
        if (useColors)
        {
            hexMesh.SetColors(colors);
            ListPool<Color>.Add(colors);
        }
        hexMesh.SetTriangles(triangles, 0);
        ListPool<int>.Add(triangles);
        if (useUVCoordinates)
        {
            hexMesh.SetUVs(0, uvs);
            ListPool<Vector2>.Add(uvs);
        }
        hexMesh.RecalculateNormals();
        if (useCollider)
        {
            meshCollider.sharedMesh = hexMesh;
        }

    }

    public void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    public void AddTriangleColor(Color color)
    {
        if (useColors)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }    
    }

    public void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        if (useColors)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
        }
    }

    public void AddTriangleUV (Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        if (useUVCoordinates)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
        }
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        if (useColors)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
            colors.Add(c4);
        }
    }

    public void AddQuadColor(Color c1, Color c2)
    {
        if (useColors)
        {
            colors.Add(c1);
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c2);
        }
    }

    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
    {
        if (useUVCoordinates)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
            uvs.Add(uv4);
        }
    }
    /// <summary>
    /// Adds rectangular UV area. Typical usecase: When quad and its texture is aligned.
    /// </summary>
    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        if (useUVCoordinates)
        {
            uvs.Add(new Vector2(uMin, vMin));
            uvs.Add(new Vector2(uMax, vMin));
            uvs.Add(new Vector2(uMin, vMax));
            uvs.Add(new Vector2(uMax, vMax));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
