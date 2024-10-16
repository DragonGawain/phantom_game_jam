using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class shipComponentGenerator : MonoBehaviour
{
    GameObject shipComponent;
    [SerializeField, Range(0, 10)] int shipComponentNum = 10;
    private void Start()
    {

        shipComponent = Resources.Load<GameObject>("Items/shipComponent");

        for (int i = 0; i < shipComponentNum; ++i) {    

            Vector3 newPosition = new Vector3(Random.Range(1.0f, 10.0f), Random.Range(1.0f, 10.0f), 0);
            Instantiate(shipComponent, newPosition, Quaternion.identity); 
            //load the resouce at run time
            // should use the overload with vector3 and quaternions

        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
