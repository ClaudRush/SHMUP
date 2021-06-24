using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;
    public float fadeTime = 4f;

    [Header("Set Dynamically")]
    public WeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public float birthTime;
    public Vector3 rotPerSecond;

    private Rigidbody rigid;
    private BoundsCheck boundsCheck;
    private Renderer cubeRend;

    private void Awake()
    {
        cube = transform.Find("Bonus").gameObject;
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        boundsCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();

        Vector3 vel = Random.onUnitSphere;
        vel.z = 0f;
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        transform.rotation = Quaternion.identity;

        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }

    private void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;

        if (u >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        if (u > 0)
        {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;
            c = letter.color;
            c.a = 1f - (u * 0.75f);
            letter.color = c;
        }

        if (!boundsCheck.isOnScreen && u > 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetType(WeaponType wt)
    {
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        cubeRend.material.color = def.color;
        letter.text = def.letter;
        type = wt;
    }

    public void AbsorbedBy(GameObject target)
    {
        Destroy(gameObject);
    }
}
