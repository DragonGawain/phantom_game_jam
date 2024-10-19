using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : Movement
{
    Inputs inputs;
    Vector2 movementInput;
    Rigidbody2D rb;

    public List<PlayerComponents> inventory = new();
    public List<ShipComponents> shipInventory = new();

    readonly int inventorySize = 999;
    readonly int shipInventorySize = 10;

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

    Transform flashlight;
    Transform advFlashlight;

    Transform questPointer1;
    Transform questPointer2;
    Transform shipPointer;

    Transform questTarget1;
    Transform questTarget2;

    float rot;

    // Start is called before the first frame update
    protected override void OnAwake()
    {
        playerAudio = GetComponent<PlayerAudio>();
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Fire.performed += ShootGun;
        inputs.Player.Close.performed += CloseMenu;

        rb = GetComponent<Rigidbody2D>();
        bulletObject = Resources.Load<GameObject>("Bullet");
        advBulletObject = Resources.Load<GameObject>("AdvBullet");
        hp = maxHp;
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        ship.SetPlayer(this);
        questPointer1 = transform.Find("QuestPointerRoot1");
        questPointer2 = transform.Find("QuestPointerRoot2");
        shipPointer = transform.Find("ShipPointer");

        flashlight = transform.Find("Flashlight");
        advFlashlight = transform.Find("AdvancedFlashlight");

        // numberOfMissingComponents.enabled = false;
    }

    private void Start()
    {
        // UIManager.ActivateMenu("hud");
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

            rot = Vector2.SignedAngle(new Vector2(0, 1).normalized, rb.velocity.normalized);
            flashlight.localEulerAngles = new Vector3(0, 0, rot);
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

        // ship pointer
        shipPointer.localEulerAngles = new Vector3(
            0,
            0,
            Vector2.SignedAngle(
                Vector2.up,
                new Vector2(
                    ship.transform.position.x - transform.position.x,
                    ship.transform.position.y - transform.position.y
                )
            )
        );
        if (questTarget1 != null)
            questPointer1.localEulerAngles = new Vector3(
                0,
                0,
                Vector2.SignedAngle(
                    Vector2.up,
                    new Vector2(
                        questTarget1.transform.position.x - transform.position.x,
                        questTarget1.transform.position.y - transform.position.y
                    )
                )
            );
        if (questTarget2 != null)
            questPointer2.localEulerAngles = new Vector3(
                0,
                0,
                Vector2.SignedAngle(
                    Vector2.up,
                    new Vector2(
                        questTarget2.transform.position.x - transform.position.x,
                        questTarget2.transform.position.y - transform.position.y
                    )
                )
            );
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
            switch (newItem)
            {
                case PlayerComponents.GUN:
                    UIManager.ActivateHudItem("gun");
                    break;
                case PlayerComponents.ADV_GUN:
                    UIManager.ActivateHudItem("advGun");
                    break;
                case PlayerComponents.BOOTS:
                    swampSpeedModifier = -0.25f;
                    StartCoroutine(FlashTriggerBox());
                    UIManager.ActivateHudItem("boots");
                    break;
                case PlayerComponents.FLASHLIGHT:
                    flashlight.gameObject.SetActive(false);
                    advFlashlight.gameObject.SetActive(true);
                    UIManager.ActivateHudItem("flashlight");
                    break;
                case PlayerComponents.COIN:
                    UIManager.ActivateHudItem("coin");
                    break;
            }
            
            return true;
        }
        return false;
    }

    public IEnumerator FlashTriggerBox()
    {
        CapsuleCollider2D triggerBox = GetComponent<CapsuleCollider2D>();
        triggerBox.enabled = false;
        yield return new WaitForFixedUpdate();
        triggerBox.enabled = true;
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
            CheckQuestArrows(size + Item.shipComponentSizes[newItem]);
            return true;
        }
        CheckQuestArrows(size);
        return false;
    }

    void CheckQuestArrows(int size)
    {
        Debug.Log("size: " + size);
        if (questTarget1 != null)
            if (size + Item.shipComponentSizes[questTarget1.GetComponent<ShipPiece>().GetShipComponentType()] > shipInventorySize)
                SetQuest1Target(null);
        
        if (questTarget2 != null)
            if (size + Item.shipComponentSizes[questTarget2.GetComponent<ShipPiece>().GetShipComponentType()] > shipInventorySize)
                SetQuest2Target(null);

        if (questTarget1 == null && questTarget2 == null)
            UIManager.SetOperationText("OpGoToShipText");
    }

    public int GetAvailableShipInventory()
    {
        int size = 0;
        foreach (ShipComponents item in shipInventory)
            size += Item.shipComponentSizes[item];
        return 10 - size;
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

    void CloseMenu(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (UIManager.isInShipInventory)
            UIManager.CloseShipInventory();
        else if (!UIManager.isInPauseMenu)
            UIManager.Pause();
        else if (UIManager.isInPauseMenu)
            UIManager.UnPause();
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

    public Transform GetQuest1Target() => questTarget1;

    public Transform GetQuest2Target() => questTarget2;
    
    public ShipComponents GetQuest1ShipComponentType() => questTarget1.GetComponent<ShipPiece>().GetShipComponentType();
    public ShipComponents GetQuest2ShipComponentType() => questTarget2.GetComponent<ShipPiece>().GetShipComponentType();

    public void SetQuest1Target(Transform target)
    {
        if (target == transform)
            target = null;
        questTarget1 = target;
        if (target == null)
            questPointer1.gameObject.SetActive(false);
        else
            questPointer1.gameObject.SetActive(true);
    }

    public void SetQuest2Target(Transform target)
    {
        if (target == transform)
            target = null;
        questTarget2 = target;
        if (target == null)
            questPointer2.gameObject.SetActive(false);
        else
            questPointer2.gameObject.SetActive(true);
    }

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
        GetComponent<SpriteRenderer>().enabled = false;
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

        maxMoveSpeed = GetOriginalSpeed() + ship.Inventory[ShipComponents.ENGINES] * 0.7f;

        questPointer1.gameObject.SetActive(false);
        questPointer2.gameObject.SetActive(false);
        shipPointer.gameObject.SetActive(false);

        uiManager.SetMaxHealth(ship.GetMaxHp());
        UIManager.ActivateEndTimer();
        StartCoroutine(EndSequenceWinTimer());
    }

    IEnumerator EndSequenceWinTimer()
    {
        yield return new WaitForSeconds(300);
        Debug.Log("<color=blue>YOU WIN!!!!! YAY!!</color>");
    }

    public void EndSequenceDamage(int val)
    {
        uiManager.SetHealth(val);
        if (Random.Range(0f, 1f) < 0.2f)
            playerAudio.EndSequenceTookDamageSound();
    }

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
        inputs.Player.Close.performed -= CloseMenu;
    }

    //shows to player the number of shipComponent needed to fix the ship
}
