using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    private Stat stat;

    //최종 데미지
    private int _totaldamage;
    //어태커의 데미지
    private int _attackerdamage;
    //최소 데미지
    private int _minattack;
    //최대 데미지
    private int _maxattack;
    //스킬 데미지
    private int _skillattack;
    //타격 횟수
    private int _hitnum;
    //상대방 방어력
    private int _defense;

    public int Calculate(object[] attackerStat, Stat meStat)
    {
        _minattack = (int)attackerStat[0];
        _maxattack = (int)attackerStat[1];
        _skillattack = (int)attackerStat[2];
        _hitnum = (int)attackerStat[3];
        _defense = meStat.Defense;

        _attackerdamage = Random.Range(_minattack,_maxattack) + _skillattack;
        _totaldamage = (_attackerdamage - _defense)/_hitnum;

        return _totaldamage;
    }


    
    
}
