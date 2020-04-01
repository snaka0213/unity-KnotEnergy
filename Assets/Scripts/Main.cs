using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] Material _material;

    private List<Vector3> _positions;
    private Curve CurrentCurve;
    private Curve NewCurve;
    private List<Vector3> Momentum;
    private List<Vector3> NewMomentum;
    private DrawCurve CurveObject;
    private SGD SGD;

    private int n_longitude = 50; // number of vertices excluding the end point along longitude
    private float scale = 2.0f; // scale of the curve

    private float lr = 1e-05f; // the learning rate
    private float alpha = 0.9f; // momentum

    private List<Vector3> OriginalKnot()
    {
        List<Vector3> _knot = new List<Vector3>();
        for (int i = 0; i <= this.n_longitude; i++)
        {
            _knot.Add(ExampleCurve5((float)i / this.n_longitude) * this.scale);
        }
        return _knot;
    }

    // trefoil knot on a torus
    private Vector3 ExampleCurve1(float t) // t in [0, 1]
    {
        float theta = 2 * Mathf.PI * t;
        float x = (2 + Mathf.Cos(3 * theta)) * Mathf.Cos(2 * theta);
        float y = (2 + Mathf.Cos(3 * theta)) * Mathf.Sin(2 * theta);
        float z = Mathf.Sin(3 * theta);
        return new Vector3(x, y, z);
    }

    // ellipse
    private Vector3 ExampleCurve2(float t) // t in [0, 1]
    {
        float theta = 2 * Mathf.PI * t;
        float x = 3 * Mathf.Cos(theta);
        float y = 2 * Mathf.Sin(theta);
        float z = 0;
        return new Vector3(x, y, z);
    }

    // unknot + perturbation
    private Vector3 ExampleCurve3(float t) // t in [0, 1]
    {
        float theta = 2 * Mathf.PI * t;
        float x = 2 * Mathf.Cos(theta) + 2 * Mathf.Exp((float) -1 / (1 - Mathf.Pow((2 * t - 1), 2)));
        float y = 2 * Mathf.Sin(theta);
        float z = 0;
        return new Vector3(x, y, z);
    }
    
    // "figure-8" unknot
    private Vector3 ExampleCurve4(float t) // t in [0, 1]
    {
        float theta = 2 * Mathf.PI * t;
        float x = 2 * Mathf.Cos(theta) / (1 + Mathf.Pow(Mathf.Sin(theta), 2));
        float y = 2 * Mathf.Cos(theta) * Mathf.Sin(theta) / (1 + Mathf.Pow(Mathf.Sin(theta), 2));
        float z;

        if (t <= 0.5f)
        {
            z = Mathf.Exp((float)-1 / (1 - Mathf.Pow((4 * t - 1), 2)));
        }
        else
        {
            z = -Mathf.Exp((float)-1 / (1 - Mathf.Pow((-4 * t + 3), 2)));
        }

        return new Vector3(x, y, z);
    }

    // "doubled" unknot
    private Vector3 ExampleCurve5(float t) // t in [0, 1]
    {
        float theta = 2 * Mathf.PI * t;
        float x;
        float y;
        float z;

        if (t <= 0.25f || 0.75f <= t)
        {
            x = 2 * Mathf.Cos(2 * theta);
            y = 2 * Mathf.Sin(2 * theta);
        }
        else
        {
            x = Mathf.Cos(2 * theta) - 1;
            y = Mathf.Sin(2 * theta);
        }

        if (t <= 0.5f)
        {
            z = Mathf.Exp((float)-1 / (1 - Mathf.Pow((4 * t - 1), 2)));
        }
        else
        {
            z = -Mathf.Exp((float)-1 / (1 - Mathf.Pow((-4 * t + 3), 2)));
        }

        return new Vector3(x, y, z);
    }

    void Start()
    {
        Debug.Log("Started");

        _positions = OriginalKnot();
        CurrentCurve = new Curve(_positions);
        CurveObject = new DrawCurve(_material, CurrentCurve);
        Debug.Log("Builded Knot");

        this.SGD = new SGD(this.lr, this.alpha);
        Debug.Log("Optimizer Ready");

        Momentum = new List<Vector3>();

        // initialize
        for (int i = 0; i <= n_longitude; i++)
        {
            Momentum.Add(new Vector3(0, 0, 0));
        }

    }

    void Update()
    {
        CurveObject.DrawMesh();

        if (Input.GetKey(KeyCode.Space))
        {
            (NewCurve, NewMomentum) = SGD.Step(CurrentCurve, Momentum);
            CurrentCurve = NewCurve;
            Momentum = NewMomentum;

            CurveObject.UpdateMesh(CurrentCurve);
            Debug.Log("Updated Knot");

            List<Vector3> tangents = CurrentCurve.GetTangents();
        }
    }

}
