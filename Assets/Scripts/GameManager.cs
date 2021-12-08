using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    private int score;
    public bool isPause { get; set; }
    public bool isGameOver { get; private set; }
    public bool isGameClear { get; private set; }
    public GameObject pauseMenu;
    public GameObject dieMenu;
    [SerializeField]
    private int gameClearWave = 5;

    private void Awake()
    {
        if (Instance != this) Destroy(gameObject);
    }

    public void AddScore(int newScore)
    {
        if (!isGameOver)
        {
            score += newScore;

            PlayerHUD.Instance.UpdateScoreText(score);

        }
    }
    private void Update()
    {

        GameClear();
    }

    public void UpdatePauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause==false)
            {
                Pause();
                PauseMenuSetActive();
            }
            else
            {
                
                Continue();
                PauseMenuDisable();

            }
        }
    }
    public void Continue()
    {
        isPause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (PlayerController.isSlowMode)
        {
            Time.timeScale = 0.2f;
        }else
        Time.timeScale = 1.0f;
        
    }

    public void GameClear()
    {
        if (isGameClear) return;
        if (EnemyMemoryPool.Wave == gameClearWave+1)
        {
            isGameClear = true;
            Pause();
            PlayerHUD.Instance.SetActiveClearUI(true);
        }
    }

    public void PauseMenuSetActive()
    {
        pauseMenu.SetActive(true);
    } 
    public void PauseMenuDisable()
    {
        pauseMenu.SetActive(false);
    }
    public void DieMenuSetActive(bool isDie)
    {
        dieMenu.SetActive(isDie);
    }
    public void Pause()
    {
        Time.timeScale = 0.0f;
        isPause = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
    }
    public void Restart()
    {
        if(dieMenu != null)
        {
            DieMenuSetActive(false);
        }
        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.SetActiveClearUI(false);
        }
        Continue();
        EnemyMemoryPool.Wave = 0;
        EnemyMemoryPool.DestroyAllEnemy();
        SceneManager.LoadScene(1);
        
    }
    public void NewGame()
    {
        Pause();
        SceneManager.LoadScene(0);
    }
}
