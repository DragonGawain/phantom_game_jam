using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject go = other.gameObject; // what game object collided?
        //Bullet bullet= go.GetComponent<Bullet>();
        
        if(go.CompareTag("EvilBullet")||go.CompareTag("Bullet"))
        {
            // when in contact bullet dissapears
            Destroy(other.gameObject);
        }

    }

}
