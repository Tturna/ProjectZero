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

    public delegate void OnExitNearObjectHandler(GameObject target);
    public delegate void OnExitNearEnemyHandler(GameObject enemy);
    public delegate void OnExitNearDoorHandler(GameObject door);

    public event OnEnterNearObjectHandler OnEnterNearObject;
    public event OnEnterNearEnemyHandler OnEnterNearEnemy;
    public event OnEnterNearDoorHandler OnEnterNearDoor;

    public event OnExitNearObjectHandler OnExitNearObject;
    public event OnExitNearEnemyHandler OnExitNearEnemy;
    public event OnExitNearDoorHandler OnExitNearDoor;

    // Event triggers
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

    void Start()
    {
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
    }
}
