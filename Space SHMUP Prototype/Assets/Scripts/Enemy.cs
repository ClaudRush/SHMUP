using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 1f;
    public float stopingValue = 0.5f;

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage;
    public float damageDoneTime;
    public bool notifiedOfDestruction;
    protected BoundsCheck boundsCheck;

    public Vector3 Pos
    {
        get { return (transform.position); }
        set { transform.position = value; }
    }

    private void Awake()
    {
        boundsCheck = GetComponent<BoundsCheck>();
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    private void Update()
    {
        Move();
        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }
        if (boundsCheck != null && boundsCheck.offDown)
        {
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = Pos;
        tempPos.y -= speed * Time.deltaTime;
        Pos = tempPos;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                if (!boundsCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                health -= Main.GetWeaponDefinition(p.Type).damageOnHit;

                if (health <= 0)
                {
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    Destroy(gameObject);
                }
                ShowDamage();
                Destroy(otherGO);
                break;

            default:
                print("Triggered by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }

    void ShowDamage()
    {
        foreach (Material material in materials)
        {
            material.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
        speed -= stopingValue;
    }

    void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
        Invoke("UnStoping", 0.2f);
    }

    void UnStoping()
    {
        speed += stopingValue;
    }
}
