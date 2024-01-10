using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MakeSingleton<TowerController>
{
    public Transform gunTransform; // Reference to the transform controlling the tower's gun/aim.
    public float firingRange; // The maximum firing range of the tower.
    public float fireRate = 2.0f; // The rate at which the tower fires (shots per second).

    private float nextFireTime = 0.0f; // The time when the tower can fire again.

    // Additional variables and methods as needed

    private void Start()
    {
        firingRange = TowerStats.Instance.range;
        fireRate = TowerStats.Instance.fireRate;
    }

    void Update()
    {
        // Check if it's time for the tower to fire again.
        if (Time.time >= nextFireTime)
        {
            // Find the closest enemy within firing range.
            GameObject closestEnemy = FindClosestEnemy();

            // If an enemy is within range, aim at it and fire.
            if (closestEnemy != null)
            {
                Fire(closestEnemy.transform);
                //AimAtEnemy(closestEnemy.transform);
            }
        }
    }

    // Method to find the closest enemy within firing range.
    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = firingRange;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy <= firingRange && distanceToEnemy < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distanceToEnemy;
            }
        }
        return closestEnemy;
    }

    // Method to aim the tower's gun at a target.
    void AimAtEnemy(Transform target)
    {
        //Vector2 direction = (target.position - gunTransform.position).normalized;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //gunTransform.rotation = Quaternion.Euler(0, 0, angle);
        Fire(target);
    }

    // Method to fire at the aimed target.
    void Fire(Transform target)
    {
        // Implement your firing logic here, e.g., instantiating bullets or triggering an attack animation.
        GameObject bulletGO = BulletPooling.SharedInstance.GetPooledBullets();

        if (bulletGO != null)
        {
            bulletGO.transform.position = transform.position;
            bulletGO.transform.rotation = transform.rotation;
            bulletGO.gameObject.SetActive(true);
            bulletGO.GetComponent<BulletObject>().Seek(target);
        }

        // Set the next allowed firing time based on the fire rate.
        nextFireTime = Time.time + 1.0f / fireRate;
    }

    public void CheckHealth()
    {
        var hp = TowerStats.Instance.health;

        if(hp > 0)
        {
            Debug.Log("tower hp is " + hp);

        }
        else
        {
            //game over
            //Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            TowerStats.Instance.health -= GameController.Instance._enemyDamage;
            CheckHealth();
        }
    }

}