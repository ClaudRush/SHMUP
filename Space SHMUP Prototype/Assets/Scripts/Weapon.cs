using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Это перечисление всех возможных типов оружия.
/// Также включает тип "shield", чтобы дать возможность совершенствовать защиту.
/// Аббревиатурой [НР] (Не реализованные. После реализции убрать приписку)
/// ниже отмечены элементы, не реализованные в этой книге.
/// </summary>

public enum WeaponType
{
    none,
    blaster,
    spread,
    phaser,
    missile_HP,
    laser_HP,
    shield
}

/// <summary>
///Класс WeaponDefinition позволяет настраивать свойства
/// конкретного вида оружия в инспекторе. Для этого класс Main
/// будет хранить массив элементов типа WeaponDefinition.
/// </summary>

[System.Serializable]
public class WeaponDefinition   
{
    public WeaponType type = WeaponType.none;
    public string letter;
    public Color color = Color.white;
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    //public float continuousDamage_HP = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20f;
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    public WeaponType type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime;
    private Renderer collarRend;

    public WeaponType Type
    {
        get { return type; }
        set { SetType(value); }
    }

    private void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        SetType(type);

        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectoleAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public void SetType(WeaponType weaponType)
    {
        type = weaponType;

        if (Type == WeaponType.none)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        def = Main.GetWeaponDefinition(type);
        collarRend.material.color = def.color;
        lastShotTime = 0;
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }

        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }

        switch (Type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;

            case WeaponType.spread:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10f, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10f, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;

            case WeaponType.phaser:
                p = MakeProjectile();
                p.gameObject.transform.localScale = new Vector3(0.75f, 2f, 0.5f);
                p.rigid.velocity = vel;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.CompareTag("Hero"))
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.Type = Type;
        lastShotTime = Time.time;
        return (p);
    }
}
