using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    protected Dictionary<string, GameObject> _childObjects = new();
    protected Dictionary<string, Component> _childComponents = new();

    /// <summary>
    /// 모든 자식 오브젝트를 바인딩
    /// </summary>
    protected void BindAllChildrenObject(){
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            _childObjects.Add(child.name, child.gameObject);
        }
    }

    /// <summary>
    /// 특정 타입의 컴포넌트를 바인딩
    /// </summary>
    /// <param name="objectName">저장할 이름</param>
    protected T Bind<T>(string objectName) where T : Component
    {
        // 이미 캐싱된 컴포넌트가 있으면 바로 반환
        if (_childComponents.ContainsKey(objectName))
        {
            return _childComponents[objectName] as T;
        }

        // 자식 중에 해당 이름을 가진 오브젝트를 찾아 컴포넌트를 바인딩
        T childComponent = GetComponentInChildren<T>(true);
        if (childComponent != null)
        {
            _childComponents.Add(objectName, childComponent);
            return childComponent;
        }

        return null;
    }
}
