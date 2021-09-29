using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingManager
{
    GameObject _player;
    //Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();
    HashSet<GameObject> _monsters = new HashSet<GameObject>();

    public Action<int> OnSpawnEvent;

    public GameObject GetPlayer() { return _player; }

    //Contents Folder에 EnemyRespawn Class에서 호출한다.
    //옵저버 패턴을 통해 OnSpawnEvent에 구독해놓고
    //몬스터가 스폰되면 Invoke(1)로 값(몬스터 수)을 하나 증가시킨다.
    public GameObject Spawn(Define.Character type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.Character.Monster:
                _monsters.Add(go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);
                break;
            case Define.Character.Player:
                _player = go;
                break;
        }

        return go;
    }

    public Define.Character GetCharacterType(GameObject go)
    {
        CharacterController bc = go.GetComponent<CharacterController>();
        if (bc == null)
            return Define.Character.Unknown;

        return bc.CharacterType;
    }

    //옵저버 패턴을 통해 OnSpawnEvent에 구독해놓은 함수에게
    //Despawn을 통해 값(몬스터 수)을 1감소시켜 몬스터가 죽었다는 사실을 알린다.
    public void Despawn(GameObject go)
    {
        Define.Character type = GetCharacterType(go);

        switch (type)
        {
            case Define.Character.Monster:
                {
                    if (_monsters.Contains(go))
                    {
                        _monsters.Remove(go);
                        if (OnSpawnEvent != null)
                            OnSpawnEvent.Invoke(-1);
                    }
                }
                break;
            case Define.Character.Player:
                {
                    if (_player == go)
                        _player = null;
                }
                break;
        }

        Managers.Resource.Destroy(go);
    }
}
