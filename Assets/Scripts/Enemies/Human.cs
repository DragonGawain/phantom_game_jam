using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Human : Enemy
{
    // idea: make the ship an enemy be under a common parent.
    // This might even make it possible for us to introduce allies - multiple enemies working together attached to the same ship?
    public Ship ship;

    // HACK:: public just to expose them in the inspector
    public List<PlayerComponents> inventory = new();
    public List<ShipComponents> shipInventory = new();

    [SerializeField]
    int inventorySize = 10;

    [SerializeField]
    int shipInventorySize = 10;

    [SerializeField, Range(8, 18)]
    float distFromAttackTarget = 10f;

    [SerializeField, Range(1, 5)]
    float distFromAttackTargetBuffer = 2f;

    [SerializeField, Range(0.5f, 5f)]
    float attackRevolutionSpeed = 1.25f;

    GameObject bulletObject;
    int shootTimer = 0;

    bool chase = true;

    static int idTracker = 0;

    // Awake is used in the parent 'Enemy', so we'll just use Start instead
    void Start()
    {
        FindNewShipPiece();
        ship.SetHuman(this);
        fleePoint = ship.transform.position;

        bulletObject = Resources.Load<GameObject>("Bullet");
        id = idTracker;
        idTracker++;
    }

    public int GetId() => id;

    public CombatState GetCombatState() => combatState;

    public Transform GetTarget() => target;

    // Attack algo
    protected override void Attack()
    {
        if (shootTimer > 0)
            shootTimer--;
        // we start with the core of the base ARRIVE, but we need to edit the movement logic at the bottom, so we can't just call base.Arrive();
        if (
            moveState != MoveState.ROT_OBSTACLE_L
            && moveState != MoveState.ROT_OBSTACLE_R
            && holdTimer <= 0
        )
        {
            float angle = Vector3.Angle(
                (fDot.position - transform.position).normalized,
                (target.position - transform.position).normalized
            );
            float righterAngle = Vector3.Angle(
                (fDot.position - transform.position).Rotate(0.4f).normalized,
                (target.position - transform.position).normalized
            );
            // move towards target
            if (angle <= targetArc)
            {
                moveState = MoveState.MOV_TARGET;
            }

            // rotate towards target
            if (angle > targetArc)
            {
                moveState = MoveState.ROT_TARGET;
                if (righterAngle < angle)
                    transform.Rotate(0, 0, 1.25f * turnSpeed);
                else
                    transform.Rotate(0, 0, -1.25f * turnSpeed);
            }
        }

        if (
            chase
            && Vector3.Distance(transform.position, target.position)
                < (distFromAttackTarget - distFromAttackTargetBuffer)
        )
            chase = false;
        else if (
            !chase
            && Vector3.Distance(transform.position, target.position)
                > (distFromAttackTarget + distFromAttackTargetBuffer)
        )
            chase = true;

        if (chase)
        {
            if (
                Vector3.Distance(transform.position, target.position)
                > (distFromAttackTarget + distFromAttackTargetBuffer)
            )
            {
                if (moveState == MoveState.MOV_TARGET)
                    rb.velocity = maxMoveSpeed * (target.position - transform.position).normalized;
                else
                    rb.velocity =
                        0.8f * maxMoveSpeed * (fDot.position - transform.position).normalized;
            }
            else
            {
                if (moveState == MoveState.MOV_TARGET)
                    rb.velocity =
                        0.55f * maxMoveSpeed * (target.position - transform.position).normalized;
                else
                    rb.velocity =
                        0.5f * maxMoveSpeed * (fDot.position - transform.position).normalized;
                Shoot();
                if (preferedTurnDir < 0)
                    rb.velocity +=
                        new Vector2(
                            rDot.position.x - transform.position.x,
                            rDot.position.y - transform.position.y
                        ).normalized * attackRevolutionSpeed;
                else
                    rb.velocity +=
                        new Vector2(
                            lDot.position.x - transform.position.x,
                            lDot.position.y - transform.position.y
                        ).normalized * attackRevolutionSpeed;
            }
        }
        else
        {
            if (moveState == MoveState.MOV_TARGET)
                rb.velocity =
                    0.55f * maxMoveSpeed * (transform.position - target.position).normalized;
            else
                rb.velocity = 0.5f * maxMoveSpeed * (transform.position - fDot.position).normalized;
            Shoot();
            if (preferedTurnDir < 0)
                rb.velocity +=
                    new Vector2(
                        rDot.position.x - transform.position.x,
                        rDot.position.y - transform.position.y
                    ).normalized * attackRevolutionSpeed;
            else
                rb.velocity +=
                    new Vector2(
                        lDot.position.x - transform.position.x,
                        lDot.position.y - transform.position.y
                    ).normalized * attackRevolutionSpeed;
        }
        // if(Vector3.Distance(transform.position, target.position) <= distFromAttackTarget)
    }

    public void SetAttackTarget(Transform target)
    {
        this.target = target;
        combatState = CombatState.ATTACK;
    }

    public void StopAttack()
    {
        combatState = CombatState.ARRIVE;
        CollectedShipPiece();
    }

    void Shoot()
    {
        if (shootTimer <= 0)
        {
            shootTimer = 100;
            Vector3 dir = (target.position - transform.position).normalized;
            GameObject bulletO = Instantiate(
                bulletObject,
                transform.position,
                Quaternion.FromToRotation(Vector3.up, dir)
            );
            Bullet bullet = bulletO.GetComponent<Bullet>();
            bullet.Launch(dir);
            bullet.SetShooterId(id);
            bulletO.tag = "EvilBullet";
        }
    }

    // inventory management
    // player
    public void AddToInventory(PlayerComponents newItem)
    {
        int size = 0;
        foreach (PlayerComponents item in inventory)
            size += Item.playerComponentSizes[item];
        if (size + Item.playerComponentSizes[newItem] <= inventorySize)
            inventory.Add(newItem);
    }

    public void RemoveFromInventory(PlayerComponents item)
    {
        if (inventory.Contains(item))
            inventory.Remove(item);
    }

    // ship
    public bool AddToShipInventory(ShipComponents newItem)
    {
        int size = GetSpaceLeftInShipInv();

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

    public void CollectedShipPiece()
    {
        if (combatState != CombatState.ARRIVE)
            return;
        int size = GetSpaceLeftInShipInv();

        if (size > 7)
            target = ship.transform;
        else
            FindNewShipPiece();
    }

    void FindNewShipPiece()
    {
        target = ship.GetTransformOfNearestNeededShipPiece(
            transform,
            shipInventory,
            shipInventorySize - GetSpaceLeftInShipInv()
        );
        if (target.TryGetComponent<ShipPiece>(out ShipPiece sp))
            sp.AddToSeekers(this);
    }

    int GetSpaceLeftInShipInv()
    {
        int size = 0;
        foreach (ShipComponents item in shipInventory)
            size += Item.shipComponentSizes[item];
        return size;
    }
}
