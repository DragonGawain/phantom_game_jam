using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField, Range(1f, 5f)]
    protected float maxMoveSpeed = 2f;

    [SerializeField, Range(0f, 1f)]
    protected float slowdownDrag = 0.2f;
}
