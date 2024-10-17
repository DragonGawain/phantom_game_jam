using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // [SerializeField, Range(1f, 5f)]
    public float maxMoveSpeed = 2f; // HACK

    [SerializeField, Range(0f, 1f)]
    protected float slowdownDrag = 0.2f;

    private float originalMaxSpeed;

    private void Awake() // saving original value
    {
        originalMaxSpeed = maxMoveSpeed;
        OnAwake();
    }

    protected virtual void OnAwake() { }

    public void SetMaxMoveSpeed(float n)
    {
        maxMoveSpeed = n;
    }

    public float GetOriginalSpeed()
    {
        return originalMaxSpeed;
    }
}
