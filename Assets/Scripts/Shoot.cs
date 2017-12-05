using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shoot : MonoBehaviour {

    public Action<BulletInfos> OnShoot;

    public BulletInfos[] Bullets;
    public BulletInfos currentBullet;

    public bool FireOnMouseClick = true;

    [Header("InspectorOnly")]
    public int showBulletId = 0;
    public int EditorGUIPrecision = 5;
    public bool showGUI = true;

    public bool hasVariations(BulletInfos bullet) { return bullet.Variations != null && bullet.Variations.Length > 0; }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && FireOnMouseClick)
        {
            Fire(FireDirection());
        }
    }

    public BulletInfos getRandomBullet()
    {
        var index = UnityEngine.Random.Range(0, Bullets.Length);
        return Bullets[index];
    }

    public void SelectNewWeapon()
    {
        currentBullet = getRandomBullet();
    }

    public Vector3 FireDirection()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouse2DWorldPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);

        Vector3 transform2DPosition = new Vector3(transform.position.x, transform.position.y, 0);

        Vector3 direction = mouse2DWorldPos - transform2DPosition;
        direction.Normalize();

        return direction;
    }

    public void Fire(Vector3 direction)
    {
        if (currentBullet == null)
            SelectNewWeapon();
        Fire(direction, currentBullet);
    }

    public void Fire(Vector3 direction, BulletInfos bullet)
    {
        direction.Scale(bullet.SpawnScale);
        Instantiate(bullet.Projectile, transform.position + direction + bullet.SpawnOffset, Quaternion.LookRotation(direction));
        if (OnShoot != null)
            OnShoot(bullet);
    }

    [Serializable]
    public class BulletInfos
    {
        [SerializeField]
        public GameObject Projectile;

        [SerializeField]
        public Material[] Variations;

        [SerializeField]
        public Vector3 SpawnOffset;
        public Vector3 SpawnScale;
    }
}
