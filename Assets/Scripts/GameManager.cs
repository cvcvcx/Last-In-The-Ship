using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool isGameOver { get; private set; }

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
}
