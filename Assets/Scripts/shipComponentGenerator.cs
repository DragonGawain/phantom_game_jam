using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class shipComponentGenerator : MonoBehaviour
{
    GameObject shipComponent;
    public Ship theShip;
    private System.Random random;

    private void Start()
    {
        random = new System.Random(DateTime.Now.Millisecond);

        shipComponent = Resources.Load<GameObject>("Items/shipComponent");
        SpriteRenderer shipRender = shipComponent.GetComponent<SpriteRenderer>();

        foreach (ShipComponents shipComponentEntity in Enum.GetValues(typeof(ShipComponents)))
        {
            Debug.Log(shipComponentEntity.ToString());
            shipRender.sprite = Resources.Load<Sprite>("Items/" + shipComponentEntity.ToString());
            for (int i = 0; i < theShip.RequiredInventory[shipComponentEntity]; ++i)
            {
                Instantiate(
                    shipComponent,
                    new Vector3(random.Next(1, 20), random.Next(1, 20), 0),
                    Quaternion.identity
                );
                //load the resouce at run time
                // should use the overload with vector3 and quaternions
            }
        }
    }

    // Update is called once per frame
    void Update() { }
}
