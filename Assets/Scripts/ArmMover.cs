using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmMover : MonoBehaviour
{
    public Vector3 StartPosition;
    public Vector3 TargetPosition;
    public float TimeToReachTarget;
    public bool TargetReached;

    private float t;

    private void Start()
    {
        StartPosition = TargetPosition = transform.position;
    }

    private void Update()
    {
        t += Time.deltaTime / TimeToReachTarget;
        transform.position = Vector3.Lerp(StartPosition, TargetPosition, t);

        if (transform.position == TargetPosition)
            TargetReached = true;
    }

    public void SetDestination(Vector3 destination, float time)
    {
        TargetReached = false;

        t = 0;
        TimeToReachTarget = time;

        StartPosition = transform.position;
        TargetPosition = destination;
    }
}
