using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArrowIndicator : MonoBehaviour
{
    public PlayerController thePlayer;
    public float distanceFromPlayer = 2f; // Distance to place the arrow from the player
    // Update is called once per frame
    void Update()
    {
        GameObject nearestShipComponent = thePlayer.FindNearestShipComponent();

        Vector3 directionToShip = nearestShipComponent.transform.position - thePlayer.transform.position;

        Vector3 arrowPosition = thePlayer.transform.position + directionToShip.normalized * distanceFromPlayer;
        transform.position = arrowPosition;

        // Calculate the angle to rotate the arrow to face the ship
        float angle = Mathf.Atan2(directionToShip.z, directionToShip.x) * Mathf.Rad2Deg;

        // Set the rotation of the arrow to face the nearest ship
        transform.rotation = Quaternion.Euler(new Vector3(0, -angle, 0));
    }
}
