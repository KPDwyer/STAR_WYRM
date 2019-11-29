using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("Refs")]
    public RingManager m_RingManager;
    public TimerManager m_TimerManager;
    public GlobalTimeManager m_GlobalTimeManager;
    public PlayerControls m_PlayerControls;
    public UIManager m_UIManager;
    public WaveManager m_WaveManager;
    [Space]
    public EnemyManager m_BaseEnemyManager;
    public EnemyManager m_TentacleEnemyManager;
    public EnemyManager m_BlockerEnemyManager;
    public EnemyManager m_SpheroidManager;
    [Space]
    public FXManager m_HitCircleFXManager;
    public FXManager m_SmashCircleFXManager;

    public static GameManager instance = null;

    private bool m_gameActive = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }

    [ContextMenu("Start")]
    public void BeginGame()
    {
        if (!m_gameActive)
        {
            m_gameActive = true;
            m_PlayerControls.BeginGame();
            m_RingManager.BeginGame();
            m_WaveManager.BeginGame();
        }
    }


    public void EndGame()
    {
        if (m_gameActive)
        {
            m_gameActive = false;
            m_PlayerControls.EndGame();
            //keep waves going maybe?
            //keep ring going maybe?
            m_UIManager.ShowGameOver(ExitToTitle);
        }
    }

    public void EnemyDestroyed(float ringGrowth)
    {
        m_RingManager.GrowRing(ringGrowth);
    }

    private void ExitToTitle()
    {
        SceneManager.LoadScene("MenUScene");
    }
}