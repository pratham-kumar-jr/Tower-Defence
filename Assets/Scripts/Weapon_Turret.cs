﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Turret : MonoBehaviour
{
    [Header("Attribute Field")]
    public float Range=10f;
    public float FireRate = 2f;
    public Transform FirePoint;
    public Bullet BulletPrefab;
    private float fireCountDown = 0f;

    public bool useLaser = false;
    public LineRenderer lineRenderer;


    [Header("Unity Setup Field")]
    public string EnemyTag = "Enemy";
    public Transform Rotator;
    public float turnSpeed = 10f;

    private Transform _Target;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("FindTarget", 1f, .5f);
        lineRenderer.enabled = false;
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(EnemyTag);

        float shortestDistance = Mathf.Infinity;
        float distBtwEnmeyAndWeapon;
        GameObject nearesrEnemy = null;
        foreach(GameObject enemy in enemies)
        {
            distBtwEnmeyAndWeapon = Vector3.Distance(enemy.transform.position, transform.position);
            if( distBtwEnmeyAndWeapon <shortestDistance)
            {
                shortestDistance = distBtwEnmeyAndWeapon;
                nearesrEnemy = enemy;
            }
        }

        if (nearesrEnemy != null && shortestDistance <= Range)
        {
            _Target = nearesrEnemy.transform;
        }
        else
            _Target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (_Target == null)
        {
            if (useLaser)
            {
                if (lineRenderer.enabled)
                    lineRenderer.enabled = false;
            }

            return;
        }
        LockOnTarget();

        if(useLaser)
        {
            Laser();
        }
        else
        {
            if (fireCountDown <= 0)
            {
                Shoot();
                fireCountDown = 1 / FireRate;
            }

            fireCountDown -= Time.deltaTime;
        }
    }

    void LockOnTarget()
    {
        Vector3 direction = _Target.position - transform.position;

        Quaternion lookrotation = Quaternion.LookRotation(direction);

        Vector3 rotation = Quaternion.Lerp(Rotator.rotation, lookrotation, turnSpeed * Time.deltaTime).eulerAngles;

        Rotator.rotation = Quaternion.Euler(0, rotation.y, 0);
    }

    void Laser()
    {
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, FirePoint.position);
        lineRenderer.SetPosition(1, _Target.position);
    }

    private void Shoot()
    {
        Bullet bullet=Instantiate<Bullet>(BulletPrefab, FirePoint.position, Quaternion.identity);
        bullet.Seek(_Target);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
