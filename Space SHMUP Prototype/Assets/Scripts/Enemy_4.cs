using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Part
{
    public string name;
    public float health;
    public string[] protectedBy;

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material mat;
}

/// <summary>
/// Enemy_4 создается за верхней границей, выбирает случайную точку на экране и перемещается к ней.
/// Добравшись до места, выбирает другую точку и продолжает двигаться, пока игрок не уничтожит его.
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;

    private Vector3 p0, p1;
    private float timeStart;
    private float duration = 4f;

    private void Start()
    {
        p0 = p1 = Pos;
        InitMovement();

        Transform t;
        foreach (Part part in parts)
        {
            t = transform.Find(part.name);
            if (t != null)
            {
                part.go = t.gameObject;
                part.mat = part.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement()
    {
        p0 = p1;
        float widMinRad = boundsCheck.camWidth - boundsCheck.radius;
        float hgtMinRad = boundsCheck.camHeight - boundsCheck.radius;

        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);
        Pos = (1 - u) * p0 + u * p1;
    }

    Part FindPart(string n)
    {
        foreach (Part part in parts)
        {
            if (part.name == n) return (part);
        }
        return null;
    }
    Part FindPart(GameObject go)
    {
        foreach (Part part in parts)
        {
            if (part.go == go) return (part);
        }
        return null;
    }

    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part part)
    {
        if (part == null) return (true);
        return (part.health <= 0);
    }

    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                if (!boundsCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part partHit = FindPart(goHit);

                if (partHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    partHit = FindPart(goHit);
                }

                if (partHit.protectedBy != null)
                {
                    foreach (string s in partHit.protectedBy)
                    {
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }

                partHit.health -= Main.GetWeaponDefinition(p.Type).damageOnHit;

                ShowLocalizedDamage(partHit.mat);
                if (partHit.health <= 0)
                {
                    partHit.go.SetActive(false);
                }

                bool allDestroyed = true;

                foreach (Part part in parts)
                {
                    if (!Destroyed(part))
                    {
                        allDestroyed = false;
                        break;
                    }
                }

                if (allDestroyed)
                {
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }

                Destroy(other);

                break;
        }
    }
}
