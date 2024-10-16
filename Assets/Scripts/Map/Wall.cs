using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] bool checkBullet;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        GameObject go = other.gameObject; // what game object collided?

        // if bool is bool is true, wall will destroy Bullet    
        if (checkBullet&&(go.CompareTag("EvilBullet") || go.CompareTag("Bullet")))
            {
                // when in contact bullet dissapears
                Destroy(other.gameObject);
            }
    }

}
