using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : Movement
{
    Inputs inputs;
    Vector2 movementInput;
    Rigidbody2D rb;

    public List<Item> inventory = new();

    [SerializeField]
    int inventorySize = 10;

    Vector3 mousePos;

    GameObject bulletObject;

    // Start is called before the first frame update
    void Awake()
    {
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Fire.performed += ShootGun;

        rb = GetComponent<Rigidbody2D>();
        bulletObject = Resources.Load<GameObject>("Bullet");
    }

    private void FixedUpdate()
    {
        movementInput = inputs.Player.Move.ReadValue<Vector2>().normalized;
        rb.velocity = Vector2.ClampMagnitude(rb.velocity + movementInput, maxMoveSpeed);
        if (rb.velocity.magnitude > 0)
            rb.velocity -= rb.velocity.normalized * slowdownDrag;
        if (rb.velocity.magnitude < 0)
            rb.velocity = Vector2.zero;
    }

    // inventory management
    public void AddToInventory(Item newItem)
    {
        if (newItem.GetComponentType() != ComponentType.PLAYER)
            return;
        int size = 0;
        foreach (Item item in inventory)
            size += item.GetSize();
        if (size + newItem.GetSize() <= inventorySize)
            inventory.Add(newItem);
    }

    public void RemoveFromInventory(Item item)
    {
        if (inventory.Contains(item))
            inventory.Remove(item);
    }

    // usable items
    void ShootGun(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (inventory.Any(it => it.TryGetComponent<Gun>(out Gun NA)))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);
            Vector3 dir = (mousePos - transform.position).normalized;
            GameObject bullet = Instantiate(
                bulletObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.up, dir)
            );
            bullet.GetComponent<Bullet>().Launch(dir);
        }
    }

    private void OnDestroy()
    {
        inputs.Player.Fire.performed -= ShootGun;
    }
}
