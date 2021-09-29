using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    //Resource Folder에 존재하는 File에 접근하여 로드한다.
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            //이미 풀에 존재하는 애라면 바로 리턴해준다.
            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        return Resources.Load<T>(path);
    }

    //path를 통해 오브젝트를 생성한다.
    public GameObject Instantiate(string path, Transform parent = null,bool following = false, int count = 5)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        //풀링하는 오브젝트는 Poolable컴포넌트가 붙어있다.
        //만약 풀링된 오브젝트라면 Pool에서 꺼낸다.
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent, following, count).gameObject;

        //풀링하는 오브젝트가 아니라면 바로 복사본 생성
        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        //go가 풀링된 오브젝트라면
        //다시 Pool에 넣겠다.
        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }
        Object.Destroy(go);
    }
}
