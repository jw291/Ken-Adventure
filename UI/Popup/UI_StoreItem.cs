using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_StoreItem : UI_Popup
{
    private GameObject _player;
    private PlayerStat _stat;
    private PlayerInventory _inventory;

    private GameObject Itemobj;
    private Text ItemPrice;

    private Data.Item item;

    private string Recieveimgname;
    private int Recieveprice;

    // Start is called before the first frame update
    public override void Init()
    {
        base.Init();

        Recieveimgname = item.name;
        Recieveprice = item.price;

        _player = Managers.Game.GetPlayer();
        _stat = _player.GetComponent<PlayerStat>();
        _inventory = _player.GetComponent<PlayerInventory>();

        Itemobj = GetObject((int)GameObjects.ItemImg).gameObject;
        Image ItemImg = Itemobj.GetComponent<Image>();
        Itemobj.BindEvent(ItemBuy);

        ItemPrice = GetText((int)Texts.Price).gameObject.GetComponent<Text>();
        gameObject.BindEvent(ItemBuy);

        Texture2D texture = Resources.Load<Texture2D>($"Prefabs/UI/Texture/{Recieveimgname}");
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        ItemImg.sprite = sprite;
        ItemPrice.text = Recieveprice.ToString();
    }

    private void ItemBuy(PointerEventData eventData)
    {
        if(_stat.Gold >= Recieveprice)
        {
            _stat.Gold -= Recieveprice;
            Debug.Log(_stat.Gold);
            _inventory.SetItem(item);
            Managers.Sound.Play("Select");
        }
        else
        {
            Managers.Sound.Play("Negative");
        }
    }

    public void SetIteminfo(Data.Item item)
    {
        this.item = item;
    }
}
