using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;     // ������Ʈ Ǯ�� ����� ���� ���� �����̽� �߰�

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [System.Serializable]
    private class ObjectInfo    // ������Ʈ Ǯ�� ������ ������Ʈ ���� Ŭ����
    {
        public string objectName;    // ������Ʈ �̸�
        public GameObject prefab;    // ������Ʈ Ǯ���� ������ ������Ʈ(������)
        public int count;            // �̸� ������ ������Ʈ ����
    }

    public bool IsReady { get; private set; }    // ������ƮǮ �Ŵ��� �غ�Ϸ� üũ�� ����

    [SerializeField]
    private ObjectInfo[] objectInfos = null;    // ������Ʈ Ǯ�� ������ ������Ʈ ���� �迭

    private string objectName;      // ������ ������Ʈ�� key�� ������ ���� ����

    private Dictionary<string, IObjectPool<GameObject>> objectPoolDic = new Dictionary<string, IObjectPool<GameObject>>();  // ������Ʈ Ǯ���� ������ ��ųʸ�

    private Dictionary<string, GameObject> objectDic = new Dictionary<string, GameObject>();    // ������Ʈ Ǯ���� ������Ʈ�� ���� �����Ҷ� ����� ��ųʸ�

    void Awake()
    {
        Init();     // ������Ʈ Ǯ �ʱ� ����
    }

    /// <summary>
    /// ������Ʈ Ǯ �ʱ� ���� �Լ�
    /// </summary>
    private void Init()
    {
        IsReady = false;    // ������Ʈ Ǯ �غ� ���·� ����

        for (int idx = 0; idx < objectInfos.Length; idx++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,   // ������Ʈ Ǯ�� ���� ����
            OnDestroyPoolObject, true, objectInfos[idx].count, objectInfos[idx].count);

            if (objectDic.ContainsKey(objectInfos[idx].objectName))     // �̹� ������Ʈ Ǯ�� ������ ������Ʈ���� üũ
            {
                Debug.LogFormat("{0} �̹� ��ϵ� ������Ʈ�Դϴ�.", objectInfos[idx].objectName);
                return;     // �̹� �����Ǿ��ٸ� �Լ� ����
            }

            objectDic.Add(objectInfos[idx].objectName, objectInfos[idx].prefab);    // ������Ʈ�� ���� �����ϱ� ���� ��ųʸ��� �߰�
            objectPoolDic.Add(objectInfos[idx].objectName, pool);   // ������Ʈ Ǯ ������ ��ųʸ��� ������Ʈ ������ ������ Ǯ�� �߰�

            // ������ count�� ���� �̸� ������Ʈ ����
            for (int i = 0; i < objectInfos[idx].count; i++)
            {
                objectName = objectInfos[idx].objectName;
                PoolAble poolAbleGo = CreatePooledItem().GetComponent<PoolAble>();
                poolAbleGo.pool.Release(poolAbleGo.gameObject);
            }
        }

        Debug.Log("������ƮǮ�� �غ� �Ϸ�");
        IsReady = true;     // ������Ʈ Ǯ �غ�Ϸ� ���·� ����
    }

    /// <summary>
    ///  ������Ʈ Ǯ�� ������ ������Ʈ�� �����ϴ� �Լ�
    /// </summary>
    /// <returns>������ ������Ʈ</returns>
    private GameObject CreatePooledItem()
    {
        GameObject poolObject = Instantiate(objectDic[objectName]);     // ������Ʈ ��ųʸ����� ������Ʈ ���� ������
        poolObject.GetComponent<PoolAble>().pool = objectPoolDic[objectName];   // ������ ������Ʈ�� PoolAble ���� ������Ʈ Ǯ�� ����� ������Ʈ Ǯ ����
        return poolObject;  // ���õ� ������Ʈ ��ȯ
    }

    /// <summary>
    /// ������Ʈ Ǯ ���� �����ǰ� �ִ� ������Ʈ�� �뿩�ϴ� �Լ�
    /// </summary>
    /// <param name="poolObject"></param>
    private void OnTakeFromPool(GameObject poolObject)
    {
        poolObject.SetActive(true);     // ������Ʈ Ȱ��ȭ ���·� ����
    }

    /// <summary>
    /// ������Ʈ Ǯ ���� �뿩�� ������Ʈ�� Ǯ�� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="poolObject"></param>
    private void OnReturnedToPool(GameObject poolObject)
    {
        poolObject.SetActive(false);    // ������Ʈ ��Ȱ��ȭ ���·� ����
    }

    /// <summary>
    /// ������Ʈ Ǯ ���� ������Ʈ�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="poolObject"></param>
    private void OnDestroyPoolObject(GameObject poolObject)
    {
        Destroy(poolObject);    // ������Ʈ ����
    }

    /// <summary>
    /// ������Ʈ Ǯ ���� ������Ʈ�� �뿩�ؿ� �� ����ϴ� �Լ�
    /// </summary>
    /// <param name="gameobjectName"></param>
    /// <returns>�뿩�Ǵ� ������Ʈ</returns>
    public GameObject GetPoolObject(string gameobjectName)
    {
        objectName = gameobjectName;    // �뿩�� ������Ʈ key�� ����

        if (!objectDic.ContainsKey(gameobjectName))     // ������Ʈ Ǯ�� ��ϵ� ������Ʈ���� üũ 
        {
            Debug.LogFormat("{0} ������ƮǮ�� ��ϵ��� ���� ������Ʈ�Դϴ�.", gameobjectName);
            return null;    // ��ϵ��� �ʾҴٸ� null ��ȯ
        }

        return objectPoolDic[gameobjectName].Get();     // ������Ʈ Ǯ�� ��ϵǾ� �ִٸ� ������Ʈ ��ȯ
    }
}
