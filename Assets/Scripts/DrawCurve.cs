using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCurve
{
    private Material _material;
    private Mesh _mesh;

    private int n_meridian = 20; // number of vertices along meridian
    private float radius = 0.05f; // radius of the tubular neighborhood

    private Vector3 origin = new Vector3(0, 0, 0);
    private Quaternion rotation = Quaternion.identity;

    public DrawCurve(Material material, Curve curve)
    {
        this._material = material;
        InitializeMesh(curve);
    }

    private void InitializeMesh(Curve curve)
    {
        _mesh = new Mesh();
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var normals = new List<Vector3>();

        int N = curve.GetLength();
        List<Vector3> positions = curve.GetPositions();
        List<Vector3> p_normals = curve.GetPrincipalNormals();
        List<Vector3> b_normals = curve.GetBiNornals();

        for (int i = 0; i < N; i++)
        {
            Vector3 ps = positions[i];
            Vector3 pn = p_normals[i];
            Vector3 bn = b_normals[i];
            for (int j = 0; j <= n_meridian; j++)
            {
                float theta = 2 * Mathf.PI * j / n_meridian; 
                Vector3 normal = Normal(pn, bn, theta);
                vertices.Add(ps + radius * normal);
                normals.Add(normal);
            }
        }

        for (int i = 0; i < N - 1; i++)
        {
            for (int j = 0; j < n_meridian; j++)
            {
                triangles.Add(GetIndex(i, j));
                triangles.Add(GetIndex(i, j + 1));
                triangles.Add(GetIndex(i + 1, j + 1));

                triangles.Add(GetIndex(i, j));
                triangles.Add(GetIndex(i + 1, j + 1));
                triangles.Add(GetIndex(i + 1, j));
            }
        }

        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        _mesh.normals = normals.ToArray();
        _mesh.RecalculateBounds();
    }

    public void UpdateMesh(Curve curve)
    {
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();

        int N = curve.GetLength();
        List<Vector3> positions = curve.GetPositions();
        List<Vector3> p_normals = curve.GetPrincipalNormals();
        List<Vector3> b_normals = curve.GetBiNornals();

        for (int i = 0; i < N; i++)
        {
            Vector3 ps = positions[i];
            Vector3 pn = p_normals[i];
            Vector3 bn = b_normals[i];
            for (int j = 0; j <= n_meridian; j++)
            {
                float theta = 2 * Mathf.PI * j / n_meridian;
                Vector3 normal = Normal(pn, bn, theta);
                vertices.Add(ps + radius * normal);
                normals.Add(normal);
            }
        }

        _mesh.SetVertices(vertices);
        _mesh.SetNormals(normals);
        _mesh.RecalculateBounds();
    }

    private Vector3 Normal(Vector3 pn, Vector3 bn, float theta)
    {
        float x = Mathf.Cos(theta);
        float y = Mathf.Sin(theta);
        Vector3 normal = x * pn + y * bn;
        return normal;
    }

    int GetIndex(int i, int j)
    {
        return (this.n_meridian + 1) * i + j;
    }

    public void DrawMesh()
    {
        Graphics.DrawMesh(_mesh, this.origin, this.rotation, this._material, 0);
    }

}
