using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public void LoadStartGame()
    {
        StartCoroutine("LoadStartGameScene");
    } 
    public void LoadGame()
    {
        StartCoroutine("LoadGameScene");
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public IEnumerator LoadStartGameScene()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        GameManager.Instance.NewGame();
    }
    public IEnumerator LoadGameScene()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        GameManager.Instance.Restart();
    } 
  
}
