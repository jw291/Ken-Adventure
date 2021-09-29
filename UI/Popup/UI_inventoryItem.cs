using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_inventoryItem : UI_Popup
{
    public UI_inventory UIparent;
    private GameObject _player;
    private PlayerStat _stat;
    private PlayerInventory _inventory;
    private Data.Item item;

    private GameObject Itemobj;

    private string Recieveimgname;

    public override void Init()
    {
        base.Init();

        if (item == null)
            return;

        Recieveimgname = item.name;

        _player = Managers.Game.GetPlayer();
        _stat = _player.GetComponent<PlayerStat>();
        _inventory = _player.GetComponent<PlayerInventory>();

        Itemobj = GetObject((int)GameObjects.ItemImg).gameObject;
        Image Itemimg = Itemobj.GetComponent<Image>();
        Itemobj.BindEvent(Equip);

        Texture2D texture = Resources.Load<Texture2D>($"Prefabs/UI/Texture/{Recieveimgname}");
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        Itemimg.sprite = sprite;

    }

    private void Equip(PointerEventData eventData)
    {
        _inventory.EquipItem(item);
        UIparent.SetEquipWindow();
        Managers.Sound.Play("Equip");
    }

    public void SetIteminfo(Data.Item item)
    {
        this.item = item;
    }
}
