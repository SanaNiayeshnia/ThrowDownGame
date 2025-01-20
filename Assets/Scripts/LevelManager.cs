using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class LevelManager : MonoBehaviour
{
    public int currentLevel = 1; 
    public int maxThrows = 3;
    private int throwsLeft;
    public GameObject[] targets; 
    private bool gameCompleted = false; 
    public int requiredScorePerLevel = 50; 
    
    public TextMeshProUGUI scoreText; 

    void Start()
    {
        StartLevel();
    }

    void StartLevel()
    {
        gameCompleted = false; 
        throwsLeft = maxThrows;
        targets = GameObject.FindGameObjectsWithTag("Target");

        scoreText.text = "Min Score To Win: " + GetRequiredScore().ToString();
    }

    public void BallThrown()
    {
        if (gameCompleted) return; 

        if (AllTargetsDestroyed())
        {
            Debug.Log("مرحله کامل شد!");
            gameCompleted = true;
            LoadNextLevel();
        }
        else if (throwsLeft <= 0)
        {
            Debug.Log("بازی تمام شد! شما شکست خوردید.");
            RestartLevel();
        }
        throwsLeft--;
    }

    bool AllTargetsDestroyed()
    {
        foreach (GameObject target in targets)
        {
            if (target != null) return false; 
        }
        return true;
    }

 public int GetRequiredScore()
{
    return requiredScorePerLevel;
}

    public void LoadNextLevel()
    {
        if (gameCompleted) return; 

        currentLevel++;
        SceneManager.LoadScene("Level" + currentLevel);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
