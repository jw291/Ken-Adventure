using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    #region Pool
    //PoolManager는 여러개의 Pool을 갖는다.
    //"@Pool_Root"에 각기 다른 오브젝트들이 쌓이는 것을 방지하기 위해서
    //오브젝트마다 Root를 하나씩 같도록 한다.
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        //오브젝트마다 루트를 만들어준
        public void Init(GameObject original,bool following = false, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";
            if (following)
            {
                switch(original.name)
                {
                    case "Bullet" :
                        FixedUpdateFollow Bff = Root.gameObject.GetOrAddComponent<FixedUpdateFollow>();
                        Bff.toFollow = GameObject.Find("muzzle").transform;
                        break;
                    case "Grenade":
                        FixedUpdateFollow Gff = Root.gameObject.GetOrAddComponent<FixedUpdateFollow>();
                        Gff.toFollow = GameObject.Find("GrenadeLaunch").transform;
                        break;
                }
            }

            //count에 따라 오브젝트가 자신의 pool에 push된다. 
            for(int i = 0; i < count; i++)
            {
                Push(Create());
            }
        }

        //새로운 객체 생성를 생성하고 그 오브젝트가 Poolable인지를 반환한다.
        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>();
        }

        //만약 Create하는 오브젝트가 Poolable오브젝트라면
        //그 오브젝트는 자신의 Pool Root 산하로 들어간다.
        //그리고 stack에 push한다.
        public void Push(Poolable poolable)
        {
            if (poolable == null) //예외처 
                return;

            poolable.transform.parent = Root;
            poolable.transform.localPosition = Vector3.zero;
            poolable.transform.localRotation = Quaternion.identity;
            poolable.gameObject.SetActive(false); //동면 중...
            poolable.isUsing = false;

            _poolStack.Push(poolable);
        }

        //push되어 대기중인 오브젝트가 있다면 꺼내온다.
        //만약 대기중인 오브젝트가 없다면 새로 만들어준다.
        //이제 대기상태에서 벗어난다.
        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();

            if (parent == null)
                poolable.transform.parent = Camera.main.gameObject.transform;

            poolable.gameObject.SetActive(true); //동면에서 깨어나서 세계로 나온다.
            poolable.transform.parent = parent;
            poolable.isUsing = true;

            return poolable;
        }
    }
    #endregion

    //각각의 Object는 각자의 Pool을 가지고 있음.
    //Object name(key)를 통해 Pool을 Dictionary에 저장 
    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();

    Transform _root;

    public void Init()
    {
        //Pool Object Root GameObject 생성 
        if(_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    //Dictionary에 존재하지않은 새로운 오브젝트가 들어오면 호출된다.
    //따라서 새롭게 풀을 생성해준다.
    public void CreatePool(GameObject original,bool following, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original,following, count);
        pool.Root.parent = _root;

        _pool.Add(original.name, pool);
    }

    //사용이 끝난 오브젝트는 다시 동면 상태로 보낸다.
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if(_pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }
        _pool[name].Push(poolable);
    }

    //동면중인 오브젝트를 활성화 시킨다.
    //original 게임오브젝트의 이름으로 Dictionary에 이 오브젝트의 _pool이 이미 존재하는지 확인한다.
    //없다면 Pool을 만들어주고(처음에 호출하면 무조건 없을테니까 Pool을 만들어줘야함)
    //있다면 해당 오브젝트의 Pool에 접근하여 Pop시켜준다.(Pool에 잠들어있던 해당 오브젝트를 깨어낸다.)
    public Poolable Pop(GameObject original, Transform parent = null, bool following = false, int count = 5)
    {
        if (_pool.ContainsKey(original.name) == false) //처음에 Pop이 호출되면 무조건 없다.
            CreatePool(original,following,count);

        return _pool[original.name].Pop(parent);
    }

    //이미 풀에 들록된 오브젝트라면 바로 리턴해준다.
    //ResourceManager의 오브젝트 Load에서 사용한다.
    //이미 풀에 있는 오브젝트인데 굳이 Path찾아서 로드할 필요가 없으니까
    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;

        return _pool[name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }

}
