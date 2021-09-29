using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRespawn : MonoBehaviour
{
    [SerializeField]
    public int _monsterCount = 0;
    public int _reserveCount = 0;

    [SerializeField]
    int _keepMonsterCount = 0;

    [SerializeField]
    Vector3 _spawnPos;
    [SerializeField]
    float _spawnRadius = 15.0f;
    [SerializeField]
    float _spawnTime = 5.0f;
    [SerializeField]
    string _enemyName = null;
    int count = 0;

    public void AddMonsterCount(int value) { _monsterCount += value; }
    public void SetKeepMonsterCount(int count) { _keepMonsterCount = count; }

    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        while (_reserveCount + _monsterCount < _keepMonsterCount)
        {
            if (count >= 1)
                break;

            StartCoroutine("ReserveSpawn");
        }
    }

    IEnumerator ReserveSpawn()
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));
        GameObject obj = Managers.Game.Spawn(Define.Character.Monster, $"Object/{_enemyName}");

        Vector3 randPos;
        
        Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
        randDir.y = 0;
        randPos = _spawnPos + randDir;
        obj.transform.position = randPos;
        _reserveCount--;
    }
}
