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
        GameObject nearestShipComponent = FindNearestShipComponent();

        Vector3 directionToShip = nearestShipComponent.transform.position - thePlayer.transform.position;

        Vector3 arrowPosition = thePlayer.transform.position + directionToShip.normalized * distanceFromPlayer;
        transform.position = arrowPosition;

        // Calculate the angle to rotate the arrow to face the ship
        //float angle = Mathf.Atan2(directionToShip.y, directionToShip.x) * Mathf.Rad2Deg;

        // Set the rotation of the arrow to face the nearest ship
        Vector3 defaultDirection = new Vector3(0, 1, 0); //postive y axis points downward
        Vector3 crossProduct = Vector3.Cross(defaultDirection, directionToShip);

        float angle;
        // Check the sign of the angle
        if (crossProduct.z > 0)
        {
            angle = Vector3.Angle(defaultDirection, directionToShip);
        }
        else
        {
            angle = -Vector3.Angle(defaultDirection, directionToShip);
        }

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }


    List<GameObject> FindObjectsWithTags(string[] tags)
    {
        List<GameObject> objectsWithTags = new List<GameObject>();

        // Find objects for each tag and add them to the list
        foreach (string tag in tags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            objectsWithTags.AddRange(objects);
        }

        // Remove duplicates
        HashSet<GameObject> uniqueObjects = new HashSet<GameObject>(objectsWithTags);
        return new List<GameObject>(uniqueObjects);
    }


    public GameObject FindNearestShipComponent()
    {
        string[] tags = { "nose_gear", "landing_gear", "oxygen_tank", "fuel_tank", "solid_boosters", "engines", "rcs", "wings" };
        List<GameObject> shipComponents = FindObjectsWithTags(tags);

        GameObject nearestShipComponent = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject sc in shipComponents)
        {
            float distance = Vector3.Distance(thePlayer.transform.position, sc.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestShipComponent = sc;
            }
        }

        return nearestShipComponent;
    }
}
