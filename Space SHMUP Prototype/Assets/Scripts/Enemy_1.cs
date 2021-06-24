using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1")]
    public float waveFrequency = 2f;
    public float waveWidth = 4f;
    public float waveRotY = 45;

    private float x0;
    private float birthTime;

    private void Start()
    {
        x0 = Pos.x;
        birthTime = Time.time;
    }

    public override void Move()
    {
        Vector3 tempPos = Pos;

        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        Pos = tempPos;
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        transform.rotation = Quaternion.Euler(rot);

        base.Move();
    }
}
