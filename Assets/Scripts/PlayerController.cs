using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : Movement
{
    Inputs inputs;
    Vector2 movementInput;
    Rigidbody2D rb;

    public List<PlayerComponents> inventory = new();
    public List<ShipComponents> shipInventory = new();

    // HACK:: SF to expose in inspector
    // I would make this into a dictionary to suit the new inventory style, but this is faster and it works
    int inventorySize = 999;

    [SerializeField]
    int shipInventorySize = 10;

    Vector3 mousePos;

    GameObject bulletObject;
    GameObject advBulletObject;

    int shootCooldown = 0;
    int advShootCooldown = 0;

    int shootCooldownReset = 75;
    int advShootCooldownReset = 95;

    // HACK:: public for inspector exposure
    public int hp;
    readonly int maxHp = 15;

    UIManager uiManager;

    int damageCooldown = 0;

    [SerializeField]
    Ship ship;

    [SerializeField]
    TextMeshProUGUI numberOfMissingComponents;

    PlayerAudio playerAudio;
    public static bool isEndingSequence = false;

    // Start is called before the first frame update
    protected override void OnAwake()
    {
        playerAudio = GetComponent<PlayerAudio>();
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Fire.performed += ShootGun;

        rb = GetComponent<Rigidbody2D>();
        bulletObject = Resources.Load<GameObject>("Bullet");
        advBulletObject = Resources.Load<GameObject>("AdvBullet");
        hp = maxHp;
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        ship.SetPlayer(this);

        // numberOfMissingComponents.enabled = false;
    }

    private void Start()
    {
        UIManager.ActivateMenu("hud");
        uiManager.SetMaxHealth(maxHp);
    }

    private void FixedUpdate()
    {
        movementInput = inputs.Player.Move.ReadValue<Vector2>().normalized;
        rb.velocity = Vector2.ClampMagnitude(rb.velocity + movementInput, maxMoveSpeed);
        if (rb.velocity.magnitude > 0.05f)
        {
            rb.velocity -= rb.velocity.normalized * slowdownDrag;
            // if (rb.velocity.magnitude < 0)
            //     rb.velocity = Vector2.zero;

            transform.localEulerAngles = new Vector3(
                0,
                0,
                Vector2.SignedAngle(new Vector2(0, 1).normalized, rb.velocity.normalized)
            );
        }
        // if (movementInput.magnitude == 0)
        if (rb.velocity.magnitude < 0.15f)
            rb.velocity = Vector2.zero;

        if (damageCooldown > 0)
            damageCooldown--;

        if (shootCooldown > 0)
            shootCooldown--;
        if (advShootCooldown > 0)
            advShootCooldown--;

        // missingComponentsIndicator();
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
            if (newItem == PlayerComponents.FLASHLIGHT)
            {
                transform.Find("Flashlight").gameObject.SetActive(false);
                transform.Find("AdvancedFlashlight").gameObject.SetActive(true);
            }
            else if (newItem == PlayerComponents.BOOTS)
                swampSpeedModifier = -0.25f;
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
        if (inventory.Contains(PlayerComponents.GUN) && shootCooldown <= 0)
        {
            playerAudio.ShootSoundBasic();
            shootCooldown = shootCooldownReset;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);
            Vector3 dir = (mousePos - transform.position).normalized;
            GameObject bulletO = Instantiate(
                bulletObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.up, dir)
            );
            Bullet bullet = bulletO.GetComponent<Bullet>();
            bullet.Launch(dir);
            bullet.SetShooterId(-2);
            bullet.SetShooter(transform);
        }
        if (inventory.Contains(PlayerComponents.ADV_GUN) && advShootCooldown <= 0)
        {
            playerAudio.ShootSoundAdvanced();
            advShootCooldown = advShootCooldownReset;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);
            Vector3 dir = (mousePos - transform.position).normalized;
            GameObject bulletOA = Instantiate(
                advBulletObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.up, dir)
            );
            Bullet bulletA = bulletOA.GetComponent<Bullet>();
            bulletA.SetSpecs(5, 8.5f, 75);
            bulletA.Launch(dir);
            bulletA.SetShooterId(-2);
            bulletA.SetShooter(transform);
        }
    }

    public void TakeDamage(int amt = 1)
    {
        if (damageCooldown > 0)
            return;
        hp -= amt;
        playerAudio.TookDamageSound();
        damageCooldown = 150; // 3 seconds of I-frames (that sounds like a lot...)
        if (hp <= 0)
        {
            Debug.Log("<color=red>THE PLAYER HAS BEEN SLAIN</color>");
        }
        uiManager.SetHealth(hp);
    }

    public void RestoreHealth(int amt = 1)
    {
        hp = Mathf.Clamp(hp + amt, 0, maxHp);
        uiManager.SetHealth(hp);
    }

    public void MissingComponentsIndicator()
    {
        if (Vector3.Distance(ship.gameObject.transform.position, transform.position) < 10)
        {
            string missingComponentsList = "";
            foreach (ShipComponents sc in Enum.GetValues(typeof(ShipComponents)))
            {
                missingComponentsList +=
                    sc.ToString()
                    + ":"
                    + ship.Inventory[sc].ToString()
                    + "/"
                    + ship.RequiredInventory[sc].ToString()
                    + "\n"; //still adding it to the ship after find all the parts
            }
            numberOfMissingComponents.text = missingComponentsList;
            numberOfMissingComponents.enabled = true;
        }
    }

    public GameObject FindNearestShipComponent()
    {
        GameObject[] shipComponents = GameObject.FindGameObjectsWithTag("ShipComponent");
        GameObject nearestShipComponent = null;
        float nearestDistance = Mathf.Infinity;

        foreach(GameObject sc in shipComponents)
        {
            float distance = Vector3.Distance(this.transform.position, ship.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestShipComponent = sc;
            }
        }

        return nearestShipComponent;
    }
    // public void ActivateAdvancedFlashlight()
    // {
    //     if (inventory.Contains(PlayerComponents.FLASHLIGHT))
    //     {
    //         transform.Find("AdvancedFlashlight").gameObject.SetActive(true);
    //     }
    // }

    // public void DeactivateAdvancedFlashlight()
    // {
    //     transform.Find("AdvancedFlashlight").gameObject.SetActive(false);
    // }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (isEndingSequence)
            return;
        if (damageCooldown > 0)
            return;
        // layer 7 => enemy, layer 8 => alien
        if (other.gameObject.layer == 7)
            TakeDamage(other.gameObject.GetComponent<Enemy>().GetDamage());
        else if (other.gameObject.layer == 8)
        {
            TakeDamage(other.gameObject.GetComponent<Enemy>().GetDamage());
            other.gameObject.GetComponent<Alien>().PlayAttackSound();
        }
    }

    public void InitializeEndingSequence()
    {
        Debug.Log("HIT");
        GameObject[] alienBases = GameObject.FindGameObjectsWithTag("AlienBase");
        foreach (GameObject ab in alienBases)
        {
            ab.GetComponent<AlienBase>().TriggerEndSequence();
            ab.GetComponent<AlienBase>().IncreaseAggro(transform);
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Human");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponentInChildren<Human>().SetCombatState(Enemy.CombatState.ATTACK);
            enemy.GetComponentInChildren<Human>().SetTarget(transform);
        }

        isEndingSequence = true;
        shootCooldownReset = 15;
        advShootCooldownReset = 25;

        maxMoveSpeed = GetOriginalSpeed() + ship.Inventory[ShipComponents.ENGINES] * 0.5f;

        uiManager.SetMaxHealth(ship.GetMaxHp());
    }

    public void EndSequenceDamage(int val) => uiManager.SetHealth(val);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEndingSequence)
            return;
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

    //shows to player the number of shipComponent needed to fix the ship
}
