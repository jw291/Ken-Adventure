using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Pause : UI_Popup
{
    GameObject _player;
    Quest _quest;
    PlayerStat _stat;
    PlayerInventory _inventory;

    GameObject Saveobj;
    GameObject Loadobj;
    GameObject Resumeobj;
    GameObject Exitobj;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();//binding

        //------Load Data Init
        _player = Managers.Game.GetPlayer();
        _quest = GameObject.Find("SimpleQuest").GetComponent<Quest>();
        _stat = _player.GetComponent<PlayerStat>();
        _inventory = _player.GetComponent<PlayerInventory>();
        //------UI Init
        Saveobj = GetButton((int)Buttons.Save).gameObject;
        Loadobj = GetButton((int)Buttons.Load).gameObject;
        Resumeobj = GetButton((int)Buttons.Resume).gameObject;
        Exitobj = GetButton((int)Buttons.Exit).gameObject;

        Saveobj.BindEvent(Save);
        Loadobj.BindEvent(Load);
        Resumeobj.BindEvent(Resume);
        Exitobj.BindEvent(Exit);
        Pause();
    }

    void Pause()
    {
        Time.timeScale = 0f;
    }

    void Save(PointerEventData eventData)
    {
        SaveSystem.SavePlayer(_player);
        ClosePopupUI();
        Time.timeScale = 1f;
    }

    void Load(PointerEventData eventData)
    {
        PlayerData data = SaveSystem.LoadPlayer();

        _player.transform.position = new Vector3(data.position[0],data.position[1],data.position[2]);
        _quest._step = data._questStep;

        _stat.Level = data._level;
        _stat.Hp = data._hp;
        _stat.MaxHp = data._maxHP;
        _stat.MinAttack = data._minattack;
        _stat.MaxAttack = data._maxattack;
        _stat.Defense = data._defense;
        _stat.MoveSpeed = data._moveSpeed;
        _stat.TotalEXP = data._totalExp;
        _stat.Exp = data._exp;
        _stat.Gold = data._gold;

        _inventory.itemlist = data._itemList;
        _inventory.EquipItem(data._equipSword);
        _inventory.EquipItem(data._equipGun);

        ClosePopupUI();
        Time.timeScale = 1f;
        //_player.transform.position.x = data.position[0];
        //_player.transform.position.y = data.position[1];
        //_player.transform.position.z = data.position[2];
    }

    void Resume(PointerEventData eventData)
    {
        Time.timeScale = 1f;
        ClosePopupUI();
        Managers.Sound.Play("Select");
    }

    void Exit(PointerEventData eventData)
    {
        ClosePopupUI();
        //저장 관련 넣기
        Application.Quit();
    }
}
