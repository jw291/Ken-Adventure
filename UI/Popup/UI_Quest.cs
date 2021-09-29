using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Quest : UI_Popup
{
    private Text QuestInfo;
    private Quest SimpleQuest;
    public override void Init()
    {
        base.Init();
        QuestInfo = GetText((int)Texts.QuestInfo).gameObject.GetComponent<Text>();
    }

    public void SetText(List<string> list)
    {
        QuestInfo.text =$"{list[0]} ({list[1]} / {list[2]})";
    }
}
