using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_inventory : UI_Popup
{
    private GameObject _player;
    private PlayerStat _stat;
    private PlayerInventory _playerInventory;
    private List<Data.Item> _itemList;
    private int inventoryLimit = 9;

    private GameObject ItemFrame;
    private Image EquipSwordimg;
    private Image EquipGunimg;
    private Text goldtext;
    private Text leveltext;
    private Text maxAttack;
    private Text minAttack;
    private Text defense;
    private Text maxhp;
    private Text movespeed;
    private Text totalexp;

    private GameObject CloseButton;

    public override void Init()
    {
        base.Init();
        Debug.Log("init");
        _player = Managers.Game.GetPlayer();
        _stat = _player.GetComponent<PlayerStat>();
        _playerInventory = _player.GetComponent<PlayerInventory>();
        _itemList = _playerInventory.itemlist;

        GameObject EquipSword = GetObject((int)GameObjects.EquipSword).gameObject;
        GameObject EquipGun = GetObject((int)GameObjects.EquipGun).gameObject;
        EquipSwordimg = EquipSword.GetComponent<Image>();
        EquipGunimg = EquipGun.GetComponent<Image>();

        ItemFrame = GetObject((int)GameObjects.ItemFrame).gameObject;
        goldtext = GetText((int)Texts.Gold).gameObject.GetComponent<Text>();
        leveltext = GetText((int)Texts.LevelText).gameObject.GetComponent<Text>();
        maxAttack = GetText((int)Texts.maxAttackText).gameObject.GetComponent<Text>();
        minAttack = GetText((int)Texts.minAttackText).gameObject.GetComponent<Text>();
        defense = GetText((int)Texts.defenseText).gameObject.GetComponent<Text>();
        maxhp = GetText((int)Texts.maxHpText).gameObject.GetComponent<Text>();
        movespeed = GetText((int)Texts.moveSpeedText).gameObject.GetComponent<Text>();
        totalexp = GetText((int)Texts.totalExpText).gameObject.GetComponent<Text>();
        CloseButton = GetButton((int)Buttons.CloseButton).gameObject;

        CloseButton.BindEvent(Clear);

        for(int i = 0; i <= inventoryLimit; i++)
        {
            GameObject item_UI = Managers.Resource.Instantiate("UI/Popup/InvenItem_UI");
            item_UI.transform.SetParent(ItemFrame.transform);

            try
            {
                UI_inventoryItem invenitem = item_UI.GetOrAddComponent<UI_inventoryItem>();
                invenitem.UIparent = this;
                invenitem.SetIteminfo(_itemList[i]);
            }
            catch (ArgumentOutOfRangeException outOfRange)
            {

                Debug.Log("Error: {0}"+outOfRange.Message);
            }
           
        }

        SetEquipWindow();
    }

    private void Update()
    {
        StartCoroutine("Optimizer");
    }

    IEnumerator Optimizer()
    {
        yield return new WaitForSeconds(0.05f);
        goldtext.text = _stat.Gold.ToString();
        leveltext.text = _stat.Level.ToString();
        maxAttack.text = _stat.MaxAttack.ToString();
        minAttack.text = _stat.MinAttack.ToString();
        defense.text = _stat.Defense.ToString();
        maxhp.text = _stat.MaxHp.ToString();
        movespeed.text = _stat.MoveSpeed.ToString();
        totalexp.text = _stat.TotalEXP.ToString();
    }

    public void SetEquipWindow()
    {
        string swordname = _playerInventory.equipSword.name;
        Texture2D texture1 = Resources.Load<Texture2D>($"Prefabs/UI/Texture/{swordname}");
        Sprite sprite1 = Sprite.Create(texture1, new Rect(0, 0, texture1.width, texture1.height), Vector2.zero);
        EquipSwordimg.sprite = sprite1;

        string gunname = _playerInventory.equipGun.name;
        Texture2D texture2 = Resources.Load<Texture2D>($"Prefabs/UI/Texture/{gunname}");
        Sprite sprite2 = Sprite.Create(texture2, new Rect(0, 0, texture2.width, texture2.height), Vector2.zero);
        EquipGunimg.sprite = sprite2;

    }

    private void Clear(PointerEventData eventData)
    {
        ClosePopupUI();
        Managers.Sound.Play("Select");
    }
}
