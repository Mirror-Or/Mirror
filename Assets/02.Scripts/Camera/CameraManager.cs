using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : IManager
{
    // 카메라 관련 변수
    private Dictionary<string, Camera> _cameraDictionary = new();

    // 마우스 커서가 잠겨있는지 확인
    public bool IsCursorLocked{ get { return Cursor.lockState == CursorLockMode.Locked; } }     
    public bool IsCursorVisible{ get { return Cursor.visible; } } // 마우스 커서가 보이는지 확인

    public void Initialize(string sceneName)
    {
        _cameraDictionary.Clear();

        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (Camera camera in cameras){
            _cameraDictionary.Add(camera.name, camera);
        }
    }

    /// <summary>
    /// 카메라를 반환하는 함수
    /// </summary>
    /// <param name="cameraName">가져오고자 하는 카메라 명칭</param>
    /// <returns>카메라 오브젝트 반환</returns>
    public Camera GetCamera(string cameraName){
        if(_cameraDictionary.ContainsKey(cameraName)){
            return _cameraDictionary[cameraName];
        }
    
        return null;
    }

    /// <summary>
    /// 카메라 컨트롤러를 반환하는 함수
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cameraName"></param>
    /// <returns></returns>
    public T GetCameraController<T>(string cameraName) where T : Component
    {
        Camera camera = GetCamera(cameraName);
        if (camera == null)
        {
            Debug.LogError($"카메라를 찾을 수 없습니다: {cameraName}");
            return null;
        }

        T controller = camera.GetComponent<T>();
        if (controller == null)
        {
            Debug.LogError($"{typeof(T).Name} 컴포넌트를 {cameraName} 카메라에서 찾을 수 없습니다.");
        }

        return controller;
    }
}
