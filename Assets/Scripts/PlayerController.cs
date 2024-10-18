using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : Movement
{
    Inputs inputs;
    Vector2 movementInput;
    Rigidbody2D rb;

    public List<PlayerComponents> inventory = new();
    public List<ShipComponents> shipInventory = new();

    // HACK:: SF to expose in inspector
    [SerializeField]
    int inventorySize = 10;

    [SerializeField]
    int shipInventorySize = 10;

    Vector3 mousePos;

    GameObject bulletObject;

    // HACK:: public for inspector exposure
    public int hp;
    readonly int maxHp = 15;

    public UIManager healthBar;

    int damageCooldown = 0;

    [SerializeField]
    Ship ship;

    PlayerAudio playerAudio;

    // Start is called before the first frame update
    protected override void OnAwake()
    {
        playerAudio = GetComponent<PlayerAudio>();
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Fire.performed += ShootGun;
        inputs.Player.Fire.performed += playerAudio.ShootSound;

        rb = GetComponent<Rigidbody2D>();
        bulletObject = Resources.Load<GameObject>("Bullet");
        hp = maxHp;
        // healthBar.SetMaxHealth(maxHp);
        ship.SetPlayer(this);
    }

    private void FixedUpdate()
    {
        movementInput = inputs.Player.Move.ReadValue<Vector2>().normalized;
        rb.velocity = Vector2.ClampMagnitude(rb.velocity + movementInput, maxMoveSpeed);
        if (rb.velocity.magnitude > 0)
            rb.velocity -= rb.velocity.normalized * slowdownDrag;
        // if (rb.velocity.magnitude < 0)
        //     rb.velocity = Vector2.zero;

        if (damageCooldown > 0)
            damageCooldown--;
    }

    // inventory management
    // personal
    public bool AddToInventory(PlayerComponents newItem)
    {
        int size = 0;
        foreach (PlayerComponents item in inventory)
            size += Item.playerComponentSizes[item];
        if (size + Item.playerComponentSizes[newItem] <= inventorySize)
        {
            inventory.Add(newItem);
            return true;
        }
        return false;
    }

    public void RemoveFromInventory(PlayerComponents item)
    {
        if (inventory.Contains(item))
            inventory.Remove(item);
    }

    // ship
    public bool AddToShipInventory(ShipComponents newItem)
    {
        int size = 0;
        foreach (ShipComponents item in shipInventory)
            size += Item.shipComponentSizes[item];
        // size += item.GetSize();

        if (size + Item.shipComponentSizes[newItem] <= shipInventorySize)
        {
            shipInventory.Add(newItem);
            return true;
        }
        return false;
    }

    public void RemoveFromShipInventory(ShipComponents item)
    {
        if (shipInventory.Contains(item))
            shipInventory.Remove(item);
    }

    public List<ShipComponents> GetShipInventory() => shipInventory;

    // usable items
    void ShootGun(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (inventory.Contains(PlayerComponents.GUN))
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
            bullet.GetComponent<Bullet>().SetShooterId(-2);
            bullet.GetComponent<Bullet>().SetShooter(transform);
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
        // healthBar.SetHealth(hp);
    }

    public void RestoreHealth(int amt = 1)
    {
        hp = Mathf.Clamp(hp + amt, 0, maxHp);
        // healthBar.SetHealth(hp);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        // layer 7 => enemy, layer 8 => alien
        if (other.gameObject.layer == 7 || other.gameObject.layer == 8)
            TakeDamage(other.gameObject.GetComponent<Enemy>().GetDamage());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("EvilBullet"))
        {
            TakeDamage(other.gameObject.GetComponent<Bullet>().GetBulletDamage());
            Destroy(other.gameObject);
        }
    }

    private void OnDestroy()
    {
        inputs.Player.Fire.performed -= ShootGun;
    }
}
