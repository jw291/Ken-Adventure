using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int _level;
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;
    [SerializeField]
    protected int _minattack;
    [SerializeField]
    protected int _maxattack;
    [SerializeField]
    protected int _defense;
    [SerializeField]
    protected float _moveSpeed;
    [SerializeField]
    protected int _totalExp;

    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int MinAttack { get { return _minattack; } set { _minattack = value; } }
    public int MaxAttack { get { return _maxattack; } set { _maxattack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public int TotalEXP { get { return _totalExp; } set { _totalExp = value; } }

    private void Awake()
    {
        //_level = 1;
        //_maxHp = 100;
        //_hp = _maxHp;
        //_minattack = 10;
        //_maxattack = 20;
        //_defense = 5;
        //_moveSpeed = 2.0f;
    }


    public virtual void AddAttack(int minattack, int maxattack) { }
    public virtual void MinusAttack(int minattack, int maxattack) { }
}
