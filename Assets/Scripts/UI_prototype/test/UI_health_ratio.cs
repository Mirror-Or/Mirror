using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_health_ratio : MonoBehaviour
{
    //ü�¹� UI
    public Image img_health_ratio;

    //ü�� ���� 0 ~ 0.75
    [SerializeField]
    private float ratio;

    // Start is called before the first frame update
    void Start()
    {
        ratio = 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        ratio -= 0.3f * Time.deltaTime;
        if (ratio < 0.01f)
            ratio = 0.75f;
        //�׽�Ʈ�� �ڵ�� ������Ʈ �����Ȳ�� ���� �̺�Ʈ �߻����� ó�� ����
        Mathf.Clamp(ratio, 0f, 0.75f);
        img_health_ratio.fillAmount = ratio;
    }
}
