using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform followTransform;

    // Start is called before the first frame update
    void Start()
    {
        // find player
        followTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //follows player
        transform.position = new Vector3(
            followTransform.position.x,
            followTransform.position.y,
            transform.position.z
        );
    }
}
