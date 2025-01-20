using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI remainingThrowsText; 
    public TextMeshProUGUI scoreText; 
    public Slider speedBar; 
    public int maxThrows = 3; 
    private int remainingThrows; 


    void Start()
    {
        remainingThrows = maxThrows;
        UpdateRemainingThrowsText();
        speedBar.value = 0; 
    }

    public void DecreaseThrowCount()
    {
        remainingThrows--;
        UpdateRemainingThrowsText();
    }

    public void UpdateSpeedBar(float speed)
    {
        speedBar.value = Mathf.Clamp01(speed);
    }

    private void UpdateRemainingThrowsText()
    {
        remainingThrowsText.text = "Throws Left: " + remainingThrows;
    }

    public void UpdateScore(int score){
        scoreText.text="Total Score: "+ score;

    }


}

