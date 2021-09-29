using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStat : MonoBehaviour
{
    public PlayerStat _stat;
    private Dictionary<string, Data.Item> _dict;
    private Data.Item item;
    public string _weaponname;
    public string _parts;
    public int _minattack;
    public int _maxattack;
    public int _price;

    void Start()
    {
        _stat = Managers.Game.GetPlayer().GetComponent<PlayerStat>();
        _dict = Managers.Data.ItemDict;
        item = _dict[gameObject.name];

        _weaponname = item.name;
        _parts = item.parts;
        _minattack = item.minattack;
        _maxattack = item.maxattack;
        _price = item.price;
    }

    void OnEnable()
    {
        StartCoroutine("Delay");
    }

    void OnDisable()
    {
        _stat.MinusAttack(_minattack, _maxattack);
        Debug.Log("해제" + _minattack + " " + _maxattack);
    }

    IEnumerator Delay()
    {
        yield return new WaitUntil(() => _stat != null);
        _stat.AddAttack(_minattack, _maxattack);
        Debug.Log("장착" + _minattack + " " + _maxattack);
    }
}
