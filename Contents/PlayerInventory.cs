using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Data.Item> itemlist = new List<Data.Item>(); //player inventory list
    private Dictionary<string, Data.Item> dict;
    public Data.Item equipSword; // the player is equipped with
    public Data.Item equipGun; //  the player is equipped with

    [SerializeField]
    private Transform SwordHandTr;
    [SerializeField]
    private Transform GunHandTr;

    void Awake()
    {
        if (SwordHandTr != null)
        {
            GameObject SwordHand = Util.AllFindChild(gameObject, "Sword");
            SwordHandTr = SwordHand.transform;
        }

        if (GunHandTr != null)
        {
            GameObject GunHand = Util.AllFindChild(gameObject, "Gun");
            GunHandTr = GunHand.transform;
        }
        dict = Managers.Data.ItemDict;

        itemlist.Add(dict["NormalSword"]);
        itemlist.Add(dict["NormalGun"]);
    }


    public void SetItem(Data.Item item)
    {
        itemlist.Add(item);
    }

    public void DefaultItemEquip()
    {

        GameObject sword = Managers.Resource.Instantiate("Object/NormalSword", SwordHandTr);
        WeaponStat ws =  sword.GetOrAddComponent<WeaponStat>();
        equipSword = dict["NormalSword"];
        ws._stat = GetComponent<PlayerStat>();

        GameObject gun = Managers.Resource.Instantiate("Object/NormalGun", GunHandTr);
        WeaponStat ws2 =  gun.GetOrAddComponent<WeaponStat>();
        equipGun = dict["NormalGun"];
        ws2._stat = GetComponent<PlayerStat>();

        GunHandTr.gameObject.SetActive(false);
    }

    public void EquipItem(Data.Item item)
    {
        //기존거 삭제
        switch(item.parts)
        {
            case "Sword":
                if(SwordHandTr.childCount > 0)
                    Destroy(SwordHandTr.GetChild(0).gameObject);

                GameObject sword = Managers.Resource.Instantiate($"Object/{item.name}", SwordHandTr);
                WeaponStat ws = sword.GetOrAddComponent<WeaponStat>();
                ws._stat = GetComponent<PlayerStat>();
                equipSword = item;
                break;
            case "Gun":
                if (GunHandTr.childCount > 0)
                    Destroy(GunHandTr.GetChild(0).gameObject);

                GameObject gun = Managers.Resource.Instantiate($"Object/{item.name}", GunHandTr);
                WeaponStat ws2 = gun.GetOrAddComponent<WeaponStat>();
                ws2._stat = GetComponent<PlayerStat>();
                equipGun = item;
                
                break;
        }
        //item으로 받은거 추가
    }
}
