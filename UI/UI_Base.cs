using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
	//Types : Buttons, GameObjects, Texts, Images 사용 중
	//UI_Base를 상속받는 UI_Popup에서 정의
	protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
	public abstract void Init();

	private void Start()
	{
		Init();
	}

	//UI_Popup에서 Enum Type에 정의한 모든 이름들을 GetNames로 가져옴
	//이를 _objects 딕셔너리에 타입에 맞게 저장함
	//전달 받은 Type에 맞게 FindChild로 종속 객체들을 전부 찾아냄.
	protected void Bind<T>(Type type) where T : UnityEngine.Object
	{
		string[] names = Enum.GetNames(type);
		UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
		_objects.Add(typeof(T), objects);

		for (int i = 0; i < names.Length; i++)
		{
			if (typeof(T) == typeof(GameObject))
				objects[i] = Util.FindChild(gameObject, names[i], true);
			else
				objects[i] = Util.FindChild<T>(gameObject, names[i], true);

			//if (objects[i] == null)
			//	//Debug.Log($"Failed to bind({names[i]})");
		}
	}

	//UI_Popup에서 모든 Button,Text,Image,GameObject를 Bind하였기 때문에
	//_objects에는 key값인 Type에 맞게 UI Object가 저장되어있음
	//따라서 바로 해당 오브젝트의 Component(Text,Button 등..)를 Get할 수 있음
	protected T Get<T>(int idx) where T : UnityEngine.Object
	{
		UnityEngine.Object[] objects = null;
		if (_objects.TryGetValue(typeof(T), out objects) == false)
			return null;

		return objects[idx] as T;
	}

	protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
	protected Text GetText(int idx) { return Get<Text>(idx); }
	protected Button GetButton(int idx) { return Get<Button>(idx); }
	protected Image GetImage(int idx) { return Get<Image>(idx); }

	//Extension Method를 통해 오브젝트와 Action이 날라옴
	//해당 오브젝트에 UI_EventHandler 컴포넌트를 부착하고
	//옵저버 패턴을 통해, 핸들러에 정의되어있는 액션에 구독권을 신청
	public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
	{
		UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

		switch (type)
		{
			case Define.UIEvent.Click:
				evt.OnClickHandler -= action;
				evt.OnClickHandler += action;
				break;
			case Define.UIEvent.Drag:
				evt.OnDragHandler -= action;
				evt.OnDragHandler += action;
				break;
			case Define.UIEvent.PointerDown:
				evt.OnPointerDownHandler -= action;
				evt.OnPointerDownHandler += action;
				break;
			case Define.UIEvent.PointerUp:
				evt.OnPointerUpHandler -= action;
				evt.OnPointerUpHandler += action;
				break;
		}
	}
}
