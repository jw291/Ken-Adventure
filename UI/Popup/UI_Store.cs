using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_Store : UI_Popup
{
    private Dictionary<string, Data.Item> dict;
    private List<Data.Item> itemlist;
    private GameObject _player;
    private PlayerStat _stat;

    private GameObject ItemFrame;
    private Text goldtext;
    private GameObject CloseButton;

    public override void Init()
    {
        base.Init();

        _player = Managers.Game.GetPlayer();
        _stat = _player.GetComponent<PlayerStat>();
        dict = Managers.Data.ItemDict;
        itemlist = new List<Data.Item>(dict.Values);

        goldtext = GetText((int)Texts.Gold).gameObject.GetComponent<Text>();
        CloseButton = GetButton((int)Buttons.CloseButton).gameObject;
        CloseButton.BindEvent(Clear);

        ItemFrame = GetObject((int)GameObjects.ItemFrame).gameObject;

        foreach (Data.Item item in itemlist)
        {
            GameObject item_UI = Managers.Resource.Instantiate("UI/Popup/StoreItem_UI");
            item_UI.transform.SetParent(ItemFrame.transform);

            UI_StoreItem storeItem = item_UI.GetOrAddComponent<UI_StoreItem>();
            storeItem.SetIteminfo(item);
        }
    }

    private void Update()
    {
        StartCoroutine("Optimizer");
    }

    IEnumerator Optimizer()
    {
        yield return new WaitForSeconds(0.05f);
        goldtext.text = _stat.Gold.ToString();
    }

    private void Clear(PointerEventData eventData)
    {
        ClosePopupUI();
        Managers.Sound.Play("Select");
    }
}
