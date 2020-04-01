using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve
{
    private const float epsilon = 1e-01f;

    private List<Vector3> Positions;
    private List<Vector3> Tangents;
    private List<Vector3> PrincipalNormals;
    private List<Vector3> BiNormals;

    public Curve(List<Vector3> Positions)
    {
        this.InitializeCurve(Positions);
    }

    public void InitializeCurve(List<Vector3> Positions)
    {
        this.Positions = new List<Vector3>(Positions);
        this.RecalculateTangents();
        this.RecalculatePrincipalNormals();
        this.RecalculateBiNormals();
    }
    
    public int GetLength()
    {
        return this.Positions.Count;
    }

    public List<Vector3> GetPositions()
    {
        return this.Positions;
    }

    public List<Vector3> GetTangents()
    {
        return this.Tangents;
    }

    public List<Vector3> GetPrincipalNormals()
    {
        return this.PrincipalNormals;
    }

    public List<Vector3> GetBiNornals()
    {
        return this.BiNormals;
    }

    public void ConnectBothEnds()
    {
        Vector3 begin_of_curve = this.Positions[0];
        this.Positions.Add(begin_of_curve);
    }

    private void RecalculateTangents()
    {
        int N = this.GetLength();
        List<Vector3> tangents = new List<Vector3>();
        for (int i = 0; i < N; i++)
        {
            int k = (i != N - 1) ? i + 1 : 1;
            int l = (i != 0) ? i - 1 : N - 2;
            tangents.Add((this.Positions[k] - this.Positions[l]) / epsilon);
        }
        this.Tangents = tangents;
    }

    private void RecalculatePrincipalNormals()
    {
        int N = this.GetLength();
        List<Vector3> p_normals = new List<Vector3>();
        for (int i = 0; i < N; i++)
        {
            int k = (i < N - 1) ? i + 1 : 1;
            int l = (i != 0) ? i - 1 : N - 2;
            Vector3 p_normal = this.Tangents[k] - this.Tangents[l];
            p_normals.Add(p_normal.normalized);
        }
        this.PrincipalNormals = p_normals;
    }

    private void RecalculateBiNormals()
    {
        int N = this.GetLength();
        List<Vector3> b_normals = new List<Vector3>();
        for (int i = 0; i < N; i++)
        {
            Vector3 b_normal = Vector3.Cross(this.Tangents[i], this.PrincipalNormals[i]);
            b_normals.Add(b_normal.normalized);
        }
        this.BiNormals = b_normals;
    }

}
