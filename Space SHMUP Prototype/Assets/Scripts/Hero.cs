using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;

    [Header("Set in Inspector")]
    public float speed = 30f;
    public float rollMult = -45f;
    public float pitchMult = 30f;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40f;
    public Weapon[] weapons;

    [Header("Set Dynamically")]
    [SerializeField] private float shieldLevel = 1;
    public int ndxWeapon = 0;

    private GameObject lastTriggerGo = null;
    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    public float ShieldLevel
    {
        get { return (shieldLevel); }
        set
        {
            shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
            if (value > 4)
            {
                value = 4;
            }
        }
    }

    private void Start()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            //Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }

        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
        weapons[1].SetType(WeaponType.blaster);
        weapons[2].SetType(WeaponType.blaster);
        weapons[3].SetType(WeaponType.blaster);
        weapons[4].SetType(WeaponType.blaster);
    }
    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        //Vector3 pos = transform.position;
        //pos.x += xAxis * speed * Time.deltaTime;
        //pos.y += yAxis * speed * Time.deltaTime;
        //transform.position = pos;

        Vector3 mousePos2D = Input.mousePosition;

        mousePos2D.z = -Camera.main.transform.position.z;

        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 pos = transform.position;
        pos.x = mousePos3D.x;
        pos.y = mousePos3D.y;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        if ((Input.GetAxis("Jump") == 1 || Main.S.buttonFire.IsPressed) && fireDelegate != null)
        {
            fireDelegate();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo) return;

        lastTriggerGo = go;

        if (go.CompareTag("Enemy"))
        {
            ShieldLevel--;
            Destroy(go);
        }
        else if (go.CompareTag("PowerUp"))
        {
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                ShieldLevel++;
                break;
            default:

                if (GetEmptyWeaponSlot() != null)
                {
                    Weapon weapon = GetEmptyWeaponSlot();
                    if (weapon != null)
                    {
                        weapon.SetType(WeaponType.blaster);
                    }
                }
                else if (pu.type == WeaponType.spread)
                {
                    if (weapons[ndxWeapon].type == WeaponType.blaster)
                    {
                        weapons[ndxWeapon].SetType(WeaponType.spread);
                        ndxWeapon++;
                    }
                }
                else if (pu.type == WeaponType.phaser)
                {
                    if (weapons[ndxWeapon].type == WeaponType.blaster)
                    {
                        weapons[ndxWeapon].SetType(WeaponType.spread);
                        ndxWeapon++;
                    }
                    else if (weapons[ndxWeapon].type == WeaponType.spread)
                    {
                        weapons[ndxWeapon].SetType(WeaponType.phaser);
                        ndxWeapon++;
                    }
                }
                if (ndxWeapon == 5)
                {
                    ndxWeapon = 0;
                }
                break;
        }

        pu.AbsorbedBy(gameObject);
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach (Weapon weapon in weapons)
        {
            weapon.SetType(WeaponType.none);
        }
    }

}
