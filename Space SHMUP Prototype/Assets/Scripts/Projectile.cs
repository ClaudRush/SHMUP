using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck boundsCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;

    [SerializeField] private WeaponType type;
    public WeaponType Type
    {
        get { return type; }
        set { SetType(value); }
    }

    private void Awake()
    {
        boundsCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (boundsCheck.offUp)
        {
            Destroy(gameObject);
        }
    }
    ///<summary>
    ///Изменяет скрытое поле type и устанавливает цвет этого снаряда,
    /// как определено в WeaponDefinition.
    ///</summary>
    ///<param name="eType">Тип WeaponType используемого оружия.</param>
    public void SetType(WeaponType eType)
    {
        type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(type);
        rend.material.color = def.projectileColor;
    }
}
