using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private TMPro.TextMeshProUGUI m_timerText;
    private float m_time = 0.0f;
    [SerializeField]
    private float m_deltaTime = 1.0f; // Update every 1 second
    private bool m_isRunning = false;

    private GameObject m_hint;
    private Button hintButton;


    public delegate void TimerStopDelegate(float time);
    public TimerStopDelegate OnTimerStop;

    // Start is called before the first frame update
    void Start()
    {
        m_timerText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (m_timerText == null)
        {
            Debug.Log("Timer Text component not found.");
            return;
        }

        m_timerText.gameObject.SetActive(false);

        // hint
        m_hint = transform.Find("Hint")?.gameObject;
        hintButton = m_hint.GetComponentInChildren<Button>();
        if (hintButton != null)
        {
            hintButton.onClick.AddListener(() =>
            {
                m_hint.SetActive(false);

                // go back to the main menu
                MainManager.Instance.SaveCurrentSave(-1, 0, 0, ProgressEnum.ChallengeAccepting);
                MainManager.Instance.NextLevel(ProgressEnum.Unknown);
            });
        }

        m_hint.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // running to fast
    }

    public void StartTimer(float time)
    {
        if (!m_isRunning)
        {
            m_isRunning = true;
            m_time = time;
            m_timerText.gameObject.SetActive(true);
            StartCoroutine(UpdateTimer());
        }
        else
        {
            Debug.Log("Timer is already running.");
        }
    }

    public void StopTimer()
    {
        OnTimerStop?.Invoke(m_time);
        if (m_time <= 0.0f)
        {
            m_hint.SetActive(true);
        }

        m_isRunning = false;
        m_time = 0.0f;
        m_timerText.gameObject.SetActive(false);
    }

    IEnumerator UpdateTimer()
    {
        while (m_isRunning)
        {
            m_time -= m_deltaTime;
            if (m_time <= 0.0f)
            {
                m_time = 0.0f;
                m_isRunning = false;
                StopTimer();
                yield break;
            }
            m_timerText.text = $"{(int)m_time / 60}:{(int)m_time % 60:D2}";
            yield return new WaitForSeconds(m_deltaTime);
        }
    }
}
