using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : Movement
{
    Inputs inputs;
    Vector2 movementInput;
    Rigidbody2D rb;

    public List<PlayerPiece> inventory = new();

    [SerializeField]
    int inventorySize = 10;

    Vector3 mousePos;

    GameObject bulletObject;

    // HACK:: public for inspector exposure
    public int hp;

    int damageCooldown = 0;

    // Start is called before the first frame update
    void Awake()
    {
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Fire.performed += ShootGun;

        rb = GetComponent<Rigidbody2D>();
        bulletObject = Resources.Load<GameObject>("Bullet");
        hp = 15;
    }

    private void FixedUpdate()
    {
        movementInput = inputs.Player.Move.ReadValue<Vector2>().normalized;
        rb.velocity = Vector2.ClampMagnitude(rb.velocity + movementInput, maxMoveSpeed);
        if (rb.velocity.magnitude > 0)
            rb.velocity -= rb.velocity.normalized * slowdownDrag;
        if (rb.velocity.magnitude < 0)
            rb.velocity = Vector2.zero;

        if (damageCooldown > 0)
            damageCooldown--;
    }

    // inventory management
    public bool AddToInventory(PlayerPiece newItem)
    {
        if (newItem.GetComponentType() != ComponentType.PLAYER)
            return false;
        int size = 0;
        foreach (PlayerPiece item in inventory)
            size += item.GetSize();
        if (size + newItem.GetSize() <= inventorySize)
        {
            inventory.Add(newItem);
            return true;
        }
        return false;
    }

    public void RemoveFromInventory(PlayerPiece item)
    {
        if (inventory.Contains(item))
            inventory.Remove(item);
    }

    // usable items
    void ShootGun(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (inventory.Any(it => it.GetPlayerComponentType() == PlayerComponents.GUN))
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

    public void TakeDamage(int amt = 1)
    {
        if (damageCooldown > 0)
            return;
        hp -= amt;
        damageCooldown = 150; // 3 seconds of I-frames (that sounds like a lot...)
        if (hp <= 0)
        {
            Debug.Log("<color=red>THE PLAYER HAS BEEN SLAIN</color>");
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        // layer 7 => enemy, layer 8 => alien
        if (other.gameObject.layer == 7 || other.gameObject.layer == 8)
        {
            TakeDamage();
        }
    }

    private void OnDestroy()
    {
        inputs.Player.Fire.performed -= ShootGun;
    }
}
