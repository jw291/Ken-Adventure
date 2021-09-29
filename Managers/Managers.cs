using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

    #region Contents
    GamingManager _game = new GamingManager();

    public static GamingManager Game { get { return Instance._game; } }
    #endregion

    //Managers 클래스에서 객체화 하고
    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SoundManager _sound = new SoundManager();
    ScenesManager _scene = new ScenesManager();
    UIManager _ui = new UIManager();

    //객체화한 각종 Managers 클래스에 접근할 수 있도록 리턴한다.
    //다른 클래스에서는 Managers.[]로 접근 가능하다.
    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static ScenesManager Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Manager@");
            if (go == null)
            {
                go = new GameObject { name = "@Manager@" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
            s_instance._sound.Init();
            s_instance._data.Init();
            s_instance._pool.Init();
            
        }
    }

    public static void Clear()
    {
        UI.Clear();
        Pool.Clear();
        Scene.Clear();
        Sound.Clear();
    }
}
