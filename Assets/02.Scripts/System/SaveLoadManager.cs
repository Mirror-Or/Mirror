using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// SaveLoadManager 클래스
/// </summary>
public class SaveLoadManager
{
	/// <summary>
	/// data를 filePath에 저장
	/// </summary>
	/// <param name="data">저장할 데이터</param>
	/// <param name="filePath">저장 경로</param>
    public static void Save<T>(T data, string filePath)
    {
        string json = JsonUtility.ToJson(data, true); // prettyPrint 옵션으로 보기 쉽게 저장
        File.WriteAllText(filePath, json);
    }

	/// <summary>
	/// filePath에서 데이터를 불러옴
	/// </summary>
	/// <param name="filePath"></param>
    public static T Load<T>(string filePath)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);  // Resources 폴더에서 TextAsset 로드
        if (textAsset != null)
        {
            string json = textAsset.text;
            return JsonUtility.FromJson<T>(json);
        }
        else
        {
            Debug.LogWarning($"{filePath} 파일이 존재하지 않습니다.");
            return default(T);	// 파일이 존재하지 않을 경우 기본값 반환
        }
    }
}