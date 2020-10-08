using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ArmMover ArmMover;
    public PickupObject PickupObject;
    public Transform PickupPoint;
    public Transform DropPoint;
    public float TimeToReachTarget = 5f;

    private bool prevTargetDrop;

    private void Start()
    {
        ArmMover.SetDestination(PickupPoint.position, TimeToReachTarget);
    }

    private void Update()
    {
        if (!ArmMover.TargetReached)
            return;

        if (!prevTargetDrop)
        {
            // Pickup Point Reached
            ArmMover.SetDestination(DropPoint.position, TimeToReachTarget);
            prevTargetDrop = true;

            PickupObject.SetParent(ArmMover.transform);
        }
        else
        {
            // Drop Point Reached
            ArmMover.SetDestination(PickupPoint.position, TimeToReachTarget);
            prevTargetDrop = false;

            PickupObject.RemoveParent();
        }

    }
}
