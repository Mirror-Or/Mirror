using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolAble : MonoBehaviour
{
    public IObjectPool<GameObject> pool { get; set; }

    /// <summary>
    /// ������Ʈ Ǯ�� ������Ʈ�� ��ȯ�ϴ� �Լ�
    /// </summary>
    public void ReleaseObject()
    {
        pool.Release(gameObject);
    }
}
