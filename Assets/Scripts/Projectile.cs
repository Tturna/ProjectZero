using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed;
    float lifeTime;
    float timer;

    void Update()
    {
        // Move the projectile right and destroy it after it's lifetime runs out
        if (timer >= lifeTime) Destroy(gameObject);
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
        timer += Time.deltaTime;
    }

    public void Initialize(float speed, float lifeTime)
    {
        this.speed = speed;
        this.lifeTime = lifeTime;

    }
}
