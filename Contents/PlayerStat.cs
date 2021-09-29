using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
	[SerializeField]
	public int _exp;
	[SerializeField]
	public int _gold;
    [SerializeField]
    private ParticleSystem _levelVFX;


    public int Exp
    {
        get { return _exp; }
        set
        {
            _exp = value;

            int level = _level;
            while (true)
            {
                Data.Stat stat;
                if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)
                    break;
                if (_exp < _totalExp)
                    break;
                else
                {
                    level++;
                    break;
                }
            }

            if (level != Level)
            {
                _levelVFX.Play();
                Managers.Sound.Play("Levelup");
                Level = level;
                Levelup(Level);
            }
        }
    }

    public int Gold { get { return _gold; } set { _gold = value; } }

    private void Awake()
	{
		_level = 1;
		_exp = 0;
		_defense = 5;
		_moveSpeed = 5.0f;
		_gold = 1500;
		_minattack = 10;
        _maxattack = 20;
		SetStat(_level);
	}

    public void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Data.Stat stat = dict[level];
        _maxHp = stat.maxHp;
        _hp = _maxHp;
        _minattack = stat.minattack;
        _maxattack = stat.maxattack;
        _totalExp = stat.totalExp;
        _exp = 0;
    }

    public void Levelup(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Data.Stat stat = dict[level];
        Data.Stat prev_stat = dict[level - 1];

        _maxHp += (stat.maxHp - prev_stat.maxHp);
        _hp = _maxHp;
        _minattack += (stat.minattack - prev_stat.minattack);
        _maxattack += (stat.maxattack - prev_stat.maxattack);
        _totalExp += (stat.totalExp - prev_stat.totalExp);
        _exp = 0;
    }

    public override void AddAttack(int min, int max)
    {
        _minattack += min;
        _maxattack += max;
    }

    public override void MinusAttack(int min, int max)
    {
        _minattack -= min;
        _maxattack -= max;
    }
}
