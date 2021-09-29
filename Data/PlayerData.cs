using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] position;

    public int _questStep;

    public int _level;
    public int _hp;
    public int _maxHP;
    public int _minattack;
    public int _maxattack;
    public int _defense;
    public float _moveSpeed;
    public int _totalExp;
    public int _exp;
    public int _gold;

    public List<Data.Item> _itemList;
    public Data.Item _equipSword;
    public Data.Item _equipGun;

    public PlayerData(GameObject player)
    {
        //------player position
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        //------player quest step
        Quest _quest = GameObject.Find("SimpleQuest").GetComponent<Quest>();
        _questStep = _quest._step;

        PlayerInventory _inventory = player.GetComponent<PlayerInventory>();
        _itemList = _inventory.itemlist;
        _equipSword = _inventory.equipSword;
        _equipGun = _inventory.equipGun;

        //player step min max는 현재 장착한 무기를 빼줘야함
        PlayerStat _stat = player.GetComponent<PlayerStat>();
        _level = _stat.Level;
        _hp = _stat.Hp;
        _maxHP = _stat.MaxHp;
        _minattack = _stat.MinAttack - _equipSword.minattack;
        _maxattack = _stat.MaxAttack - _equipSword.maxattack;
        _defense = _stat.Defense;
        _moveSpeed = _stat.MoveSpeed;
        _totalExp = _stat.TotalEXP;
        _exp = _stat.Exp;
        _gold = _stat.Gold;

    }
}
