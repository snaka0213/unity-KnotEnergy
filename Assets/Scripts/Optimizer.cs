using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGD
{
    private float lr;
    private float alpha;
    private Curve curve;

    public SGD(float lr = 1e-03f, float alpha = 0.9f)
    {
        this.lr = lr;
        this.alpha = alpha;
    }

    public Curve Step(Curve curve)
    {
        this.curve = curve;
        int N = curve.GetLength();

        List<Vector3> CurrentPositions = this.curve.GetPositions();
        List<Vector3> NewPositions = new List<Vector3>();
        
        Loss loss = new Loss(this.curve);
        List<Vector3> grad = loss.Gradient();
        List<Vector3> Momentum = new List<Vector3>();

        // initialize Momentum
        for (int i = 0; i < N; i++)
        {
            Momentum.Add(new Vector3(0, 0, 0));
        }

        // gradient descent
        for (int i = 0; i < N; i++)
        {
            Vector3 P = CurrentPositions[i];
            Vector3 DP = -this.alpha * Momentum[i] + (1 - this.alpha) * this.lr * grad[i];
            Momentum[i] = DP;
            NewPositions.Add(P - DP);
        }

        Curve _curve = new Curve(NewPositions);
        return _curve;
    }
    
}
