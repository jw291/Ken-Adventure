using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager
{
    public Canvas canvas = null;

    int _order = 10; //UI SortOrder Manage

    //stack이 비어있는지 들어있는지에 따라
    //UI가 생성되었는지 삭제되었는지를 판단할 수 있음
    //UI 생성 삭제와 stack의 생성 삭제는 LIFO로 유사하기 때문에 가장 적절한 자료구조
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

    UI_Scene _sceneUI = null;

    //UI Object(Canvas Prefab)들의 부모 오브젝트 정의
    public GameObject Root
    {
        get
        {
			GameObject root = GameObject.Find("@UI_Root");
			if (root == null)
				root = new GameObject { name = "@UI_Root" };
            return root;
		}
    }

    //Canvas를 Set할 때 Sorting오더를 하나씩 증가시킨다.
    public void SetCanvas(GameObject go, bool sort = true)
    {
        canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    //PopupUI 생성
    //ResourceManager를 통해 path를 넘겨서 생성함.
    //생성과 동시에 제네릭Type으로 받은 컴포넌트를 부착함
    //그리고 Stack에 Push
	public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);
        
        go.transform.SetParent(Root.transform);

		return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        //stack이 비어있다면 close할게 없다.
		if (_popupStack.Count == 0)
			return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    //제거할 때에는 stack에서 pop
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }
}
