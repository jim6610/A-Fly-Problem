using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private Text timeDisplay;
    [SerializeField] private bool startImmediately;

    private bool _startTimer;
    private float _seconds;

    private void Awake()
    {
        _seconds = GameManager.levelTimer;

        _startTimer = startImmediately;

        UpdateLabel(_seconds);
    }

    void Update()
    {
        if (!_startTimer) return;

        _seconds -= Time.deltaTime;

        var timeAsInt = Mathf.FloorToInt(_seconds);

        // Music pitches up when timer reaches 30s
        if (timeAsInt < 30)
        {
            GameObject.Find("BackgroundMusic").gameObject.GetComponent<AudioSource>().pitch = 1.15f;
        }

        // Level over conditions: timer runs out or all flies killed
        if (timeAsInt < 0 || GameManager.flyKillCount == GameManager.startingNumberOfFlies)
        {
            _startTimer = false;
            GameManager.LevelOver(timeAsInt);
        }

        

        UpdateText(_seconds);
    }

    public void StartTimer()
    {
        _startTimer = true;
    }

    void UpdateText(float displayTime)
    {
        if (displayTime < 0)
        {
            // Prevents seeing the timer hit -1 for a split second
            displayTime = 0;
        }

        UpdateLabel(displayTime);
    }


    private void UpdateLabel(float displayTime)
    {
        float minutes = Mathf.FloorToInt(displayTime / 60);
        float seconds = Mathf.FloorToInt(displayTime % 60);

        timeDisplay.text = $"{minutes:00}:{seconds:00}";
    }

}
