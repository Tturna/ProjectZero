using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VicinityManager : MonoBehaviour
{
    private CircleCollider2D vicinityTrigger;
    [SerializeField] private float interactRange;

    // Events
    public delegate void OnEnterNearObjectHandler(GameObject target);
    public delegate void OnEnterNearEnemyHandler(GameObject enemy);
    public delegate void OnEnterNearDoorHandler(GameObject door);
    public delegate void OnEnterNearLockerHandler(GameObject locker);

    public delegate void OnExitNearObjectHandler(GameObject target);
    public delegate void OnExitNearEnemyHandler(GameObject enemy);
    public delegate void OnExitNearDoorHandler(GameObject door);
    public delegate void OnExitNearLockerHandler(GameObject locker);

    public event OnEnterNearObjectHandler OnEnterNearObject;
    public event OnEnterNearEnemyHandler OnEnterNearEnemy;
    public event OnEnterNearDoorHandler OnEnterNearDoor;
    public event OnEnterNearLockerHandler OnEnterNearLocker;

    public event OnExitNearObjectHandler OnExitNearObject;
    public event OnExitNearEnemyHandler OnExitNearEnemy;
    public event OnExitNearDoorHandler OnExitNearDoor;
    public event OnExitNearLockerHandler OnExitNearLocker;

    #region Event Triggers

    // Enter
    protected virtual void OnEnterNearObjectTrigger(GameObject target)
    {
        OnEnterNearObject?.Invoke(target);
    }

    protected virtual void OnEnterNearEnemyTrigger(GameObject enemy)
    {
        OnEnterNearEnemy?.Invoke(enemy);
    }

    protected virtual void OnEnterNearDoorTrigger(GameObject door)
    {
        OnEnterNearDoor?.Invoke(door);
    }

    protected virtual void OnEnterNearLockerTrigger(GameObject locker)
    {
        OnEnterNearLocker?.Invoke(locker);
    }

    // Exit

    protected virtual void OnExitNearObjectTrigger(GameObject target)
    {
        OnExitNearObject?.Invoke(target);
    }

    protected virtual void OnExitNearEnemyTrigger(GameObject enemy)
    {
        OnExitNearEnemy?.Invoke(enemy);
    }

    protected virtual void OnExitNearDoorTrigger(GameObject door)
    {
        OnExitNearDoor?.Invoke(door);
    }

    protected virtual void OnExitNearLockerTrigger(GameObject locker)
    {
        OnExitNearLocker?.Invoke(locker);
    }

    #endregion

    void Start()
    {
        // Create a cirlce collider as a trigger to check for close objects
        vicinityTrigger = gameObject.AddComponent<CircleCollider2D>();
        vicinityTrigger.isTrigger = true;
        vicinityTrigger.radius = interactRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Entered " + collision.name);
        OnEnterNearObjectTrigger(collision.gameObject);

        if (collision.gameObject.tag == "Enemy")
        {
            OnEnterNearEnemyTrigger(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Door")
        {
            OnEnterNearDoorTrigger(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Locker")
        {
            OnEnterNearLockerTrigger(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Exited " + collision.name);
        OnExitNearObjectTrigger(collision.gameObject);

        if (collision.gameObject.tag == "Enemy")
        {
            OnExitNearEnemyTrigger(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Door")
        {
            OnExitNearDoorTrigger(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Locker")
        {
            OnExitNearLockerTrigger(collision.gameObject);
        }
    }
}
