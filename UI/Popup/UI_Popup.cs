using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_Popup : UI_Base
{
    protected enum Buttons
    {
        Roll,
        SwordAttack,
        GunAttack,
        WeaponSwitch,
        SwordSkill1,
        SwordSkill2,
        GunSkill,
        Inventory,
        Store,
        Exit,
        CloseButton,
        Resume,
        Save,
        Load
    }

    protected enum GameObjects
    {
        JoyStick,
        JoyStick_handle,
        ItemFrame,
        ItemImg,
        EquipSword,
        EquipGun,
    }

    protected enum Texts
    {
        Level,
        Price,
        Gold,
        LevelText,
        maxAttackText,
        minAttackText,
        defenseText,
        maxHpText,
        moveSpeedText,
        totalExpText,
        QuestInfo
    }

    protected enum Images
    {
        Healthbar,
        Expbar,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);

        //종속 객체들을 Dictionary에 모두 binding
        //Enum을 전달하고 string으로 찾는 방식
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
