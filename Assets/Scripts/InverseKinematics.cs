using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematics : MonoBehaviour
{
    public int ChainLength = 2;
    public Transform Target;
    public Transform Pole;

    public int Iterations = 10;
    public float Delta = 0.001f;

    [Range(0, 1)]
    public float SnapBackStrength = 1f;

    private Transform[] bones;
    private Vector3[] positions;
    private float[] bonesLength;
    private float fullLength;
    private Vector3[] startDirectionSucc;
    private Quaternion[] startRotationBone;
    private Quaternion startRotationTarget;
    private Quaternion startRotationRoot;

    private void Awake()
    {
        Initialize();
    }

    private void LateUpdate()
    {
        Resolve();
    }

    private void Initialize()
    {
        bones = new Transform[ChainLength + 1];
        positions = new Vector3[ChainLength + 1];
        bonesLength = new float[ChainLength];
        startDirectionSucc = new Vector3[ChainLength + 1];
        startRotationBone = new Quaternion[ChainLength + 1];
        startRotationTarget = Target.rotation;
        fullLength = 0;

        var current = this.transform;
        for (int i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            startRotationBone[i] = current.rotation;

            if (i == bones.Length -1)
            {
                startDirectionSucc[i] = Target.position - current.position;
            }
            else
            {
                startDirectionSucc[i] = bones[i + 1].position - current.position;
                bonesLength[i] = (bones[i + 1].position - current.position).magnitude;
                fullLength += bonesLength[i];
            }

            current = current.parent;
        }
    }

    private void Resolve()
    {
        if (Target == null)
            return;

        if (bonesLength.Length != ChainLength)
            Initialize();

        for (int i = 0; i < bones.Length; i++)
            positions[i] = bones[i].position;

        var rootRot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        var rootRotDiff = rootRot * Quaternion.Inverse(startRotationRoot);

        if ((Target.position - bones[0].position).sqrMagnitude >= fullLength * fullLength)
        {
            var direction = (Target.position - positions[0]).normalized;
            for (int i = 1; i < positions.Length; i++)
                positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
        }
        else
        {
            for (int i = 0; i < positions.Length - 1; i++)
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + rootRotDiff * startDirectionSucc[i], SnapBackStrength);

            for (int i = 0; i < Iterations; i++)
            {
                for (int j = positions.Length - 1; j > 0; j--)
                {
                    if (j == positions.Length - 1)
                        positions[j] = Target.position;
                    else
                        positions[j] = positions[j + 1] + (positions[j] - positions[j + 1]).normalized * bonesLength[j];
                }

                for (int j = 1; j < positions.Length; j++)
                    positions[j] = positions[j - 1] + (positions[j] - positions[j - 1]).normalized * bonesLength[j - 1];

                if ((positions[positions.Length - 1] - Target.position).sqrMagnitude < Delta * Delta)
                    break;
            }
        }

        if (Pole != null)
        {
            for (int i = 1; i < positions.Length - 1; i++)
            {
                var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(Pole.position);
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }

        for (int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
                bones[i].rotation = Target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
            else
                bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], positions[i + 1] - positions[i]) * startRotationBone[i];

            bones[i].position = positions[i];
        }
    }
}
