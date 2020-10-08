using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupObject : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rigidbody;

    private void Start()
    {
        startPosition = this.transform.position;
        startRotation = this.transform.rotation;

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }

    public void SetParent(Transform target)
    {
        this.transform.SetParent(target);
        rigidbody.isKinematic = true;
    }

    public void RemoveParent()
    {
        this.transform.SetParent(null);
        rigidbody.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name != "Ground")
            return;

        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(2f);
        transform.position = startPosition;
        transform.rotation = startRotation;
        rigidbody.isKinematic = true;
    }
}
