using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ThrowBall : MonoBehaviour
{
    public float minThrowSpeed = 20f;
    public float maxThrowSpeed = 50f;
    public float throwAngle = 10f;
    public Rigidbody rb;

    [Header("Key Settings")]
    public KeyCode throwKey = KeyCode.Space;
    public KeyCode increaseAngleKey = KeyCode.UpArrow;
    public KeyCode decreaseAngleKey = KeyCode.DownArrow;
    public KeyCode rotateRightKey = KeyCode.RightArrow;
    public KeyCode rotateLeftKey = KeyCode.LeftArrow;

    public LineRenderer trajectoryLine;
    public LevelManager levelManager;
    public UIController uiController;

    private float throwSpeed;
    private float holdTime = 0f;
    private Vector3 throwDirection;
    private bool hasThrown = false;
    private float rotationY = 90f;
    private int remainingThrows = 3;
    private List<GameObject> targets = new List<GameObject>();
    private List<Vector3> targetInitialPositions = new List<Vector3>();
    private int score = 0;
    private Vector3 initialPosition;

    public GameObject winPanel;
    public GameObject losePanel;

    public Button restartButton;
    public Button nextLevelButton;
    public GameObject targetPrefab;
    public float respawnDelay = 2.0f;
    public Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);
    private int currentThrowScore = 0;
    public AudioSource collisionSoundSource;
    




    void Start()
    {
        initialPosition = transform.position;
        throwSpeed = minThrowSpeed;

        if (trajectoryLine == null)
        {
            Debug.LogError("LineRenderer به اسکریپت متصل نشده است!");
        }

        trajectoryLine.gameObject.layer = LayerMask.NameToLayer("IgnoreLine");

        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Target"))
        {
            targets.Add(target);
            targetInitialPositions.Add(target.transform.position);
        }

        winPanel.SetActive(false);
        losePanel.SetActive(false);

        restartButton.onClick.AddListener(RestartLevel);
        nextLevelButton.onClick.AddListener(GoToNextLevel);

        uiController.UpdateScore(score);
    }

    void Update()
    {
        if (!hasThrown)
        {
            HandleInput();
            CalculateThrowDirection();
            DisplayTrajectory();
        }

        if (Input.GetKeyUp(throwKey) && !hasThrown && remainingThrows > 0)
        {
            Throw();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKey(increaseAngleKey))
        {
            throwAngle = Mathf.Clamp(throwAngle + 10f * Time.deltaTime, 5f, 85f);
        }
        if (Input.GetKey(decreaseAngleKey))
        {
            throwAngle = Mathf.Clamp(throwAngle - 10f * Time.deltaTime, 5f, 85f);
        }

        if (Input.GetKey(rotateRightKey))
        {
            rotationY += 100f * Time.deltaTime;
        }
        if (Input.GetKey(rotateLeftKey))
        {
            rotationY -= 100f * Time.deltaTime;
        }

        if (Input.GetKey(throwKey))
        {
            holdTime += Time.deltaTime;
            throwSpeed = Mathf.Clamp(minThrowSpeed + holdTime * 5f, minThrowSpeed, maxThrowSpeed);

            uiController.UpdateSpeedBar(throwSpeed / maxThrowSpeed);
        }
    }

    private void CalculateThrowDirection()
    {
        float angleInRadians = throwAngle * Mathf.Deg2Rad;
        throwDirection = Quaternion.Euler(0, rotationY, 0) * new Vector3(0, Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
    }

    private void DisplayTrajectory()
    {
        if (trajectoryLine == null) return;

        int segmentCount = 30;
        trajectoryLine.positionCount = segmentCount;

        Vector3 startPosition = transform.position;
        Vector3 velocity = throwDirection * throwSpeed;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 position = startPosition + velocity * t + 0.5f * Physics.gravity * t * t;
            trajectoryLine.SetPosition(i, position);
        }
    }

    private void Throw()
    {
        if (rb == null || levelManager == null)
        {
            Debug.LogError("Rigidbody یا LevelManager تنظیم نشده است!");
            return;
        }

        rb.velocity = throwDirection * throwSpeed;
        hasThrown = true;
        trajectoryLine.positionCount = 0;
        uiController.UpdateSpeedBar(0);


    remainingThrows--;
    currentThrowScore = 0; 

        uiController.DecreaseThrowCount();

        if (targets.Count == 0)
        {
            RespawnTargets();
        }

        if (remainingThrows <= 0)
        {
            StartCoroutine(CheckLevelProgression());
        }

        StartCoroutine(ResetThrowAfterDelay(2f));
    }

    private IEnumerator RespawnTargetsWithDelay()
    {
        yield return new WaitForSeconds(respawnDelay); 
        RespawnTargets();
    }

    private void RespawnTargets()
    {
        for (int i = 0; i < targetInitialPositions.Count; i++)
        {
            if (i < targets.Count)
            {
                GameObject target = targets[i];
                target.SetActive(true);
                target.transform.position = targetInitialPositions[i];
            }
            else
            {
                GameObject newTarget = Instantiate(targetPrefab, targetInitialPositions[i], Quaternion.identity);
                newTarget.transform.localScale = targetScale; 
                targets.Add(newTarget);
            }
        }
    }

    private IEnumerator ResetThrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetThrow();
    }

    public void ResetThrow()
    {
        hasThrown = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = initialPosition;
        throwSpeed = minThrowSpeed;
        holdTime = 0f;

        DisplayTrajectory();
    }

    private IEnumerator CheckLevelProgression()
    {
        yield return new WaitForSeconds(2f);

        if (score >= levelManager.GetRequiredScore())
        {
            ShowWinPanel();
        }
        else
        {
            ShowLosePanel();
        }
    }


private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Target"))
    {
        GameObject target = collision.gameObject;

        if (targets != null && targets.Contains(target))
        {
            targets.Remove(target);
            target.SetActive(false); 
            currentThrowScore += 10; 
            score += 10;

            // Play collision sound
            if (collisionSoundSource != null)
            {
                collisionSoundSource.Play();
            }

            if (uiController != null)
            {
                uiController.UpdateScore(score);
            }
        }

        if (targets.Count == 0 && remainingThrows > 0)
        {
            StartCoroutine(RespawnTargetsWithDelay());
        }
    }
}



    private void ShowWinPanel()
    {
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    private void ShowLosePanel()
    {
        losePanel.SetActive(true);
        winPanel.SetActive(false);
    }

    private void RestartLevel()
    {
        levelManager.RestartLevel();
        ResetThrow();
        losePanel.SetActive(false);
    }

    private void GoToNextLevel()
    {
        levelManager.LoadNextLevel();
        ResetThrow();
        winPanel.SetActive(false);
    }
}