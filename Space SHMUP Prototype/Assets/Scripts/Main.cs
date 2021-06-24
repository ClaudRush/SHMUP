using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;

    public ButtonController buttonFire;

    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[]
    {
        WeaponType.blaster, WeaponType.blaster,
        WeaponType.spread, WeaponType.shield,
        WeaponType.shield, WeaponType.phaser
    };

    private BoundsCheck boundsCheck;

    private void Awake()
    {
        S = this;
        boundsCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
            //print(def.type);
        }
    }

    public void SpawnEnemy()
    {
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -boundsCheck.camWidth + enemyPadding;
        float xMax = boundsCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = boundsCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    public void ShipDestroyed(Enemy e)
    {
        if (Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            GameObject go = Instantiate(prefabPowerUp);
            PowerUp pu = go.GetComponent<PowerUp>();

            pu.SetType(puType);
            pu.transform.position = e.transform.position;
        }
    }

    public void DelayedRestart(float delay)
    {
        Invoke(nameof(Restart), delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    ///<summary>
    ///Статическая функция, возвращающая WeaponDefinition из статического 
    /// защищенного поля WEAP_DICT класса Main.
    ///</summary>
    ///<returns>Экземпляр WeaponDefinition или, если нет такого определения
    /// для указанного WeaponType, возвращает новый экземпляр WeaponDefinition
    /// с типом none. </returns>
    /// <param name="wt">Тип оружия WeaponType, для которого требуется получить
    /// WeaponDefinition.</param>

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }

        return (new WeaponDefinition());
    }
}
