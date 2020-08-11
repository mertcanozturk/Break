using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameColors { Blue = 0, Red = 1, Green = 2 };
    public enum GameStates { Playing = 0, WaitingForPlay = 1, MarketPlace = 2, Highscores = 3 }
    public GameStates gameState;

    public enum GameMode { Level = 1, Endless = 2 }
    public GameMode gameMode;

    public enum GameLevelMode { Classic = 1, Hard = 2 }
    public GameLevelMode gameLevelMode;

    public int level;
    public int diamond;
    public int health = 1;
    public int score = 0;
    public int SelectBallId;

    [SerializeField] public TextMeshProUGUI diamondText;

    public Color[] gamecolor;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        diamondText.text = diamond.ToString();
        SaveData();
        IncreaseDiamond(0);
    }


    private PlayerBall m_PlayerBall;
    public PlayerBall PlayerBall
    {
        get
        {
            if (m_PlayerBall == null)
            {
                m_PlayerBall = FindObjectOfType<PlayerBall>();
            }
            return m_PlayerBall;
        }
    }

    private AudioManager m_AudioManager;
    public AudioManager AudioManager
    {
        get
        {
            if (m_AudioManager == null)
            {
                m_AudioManager = FindObjectOfType<AudioManager>();
            }
            return m_AudioManager;
        }
    }

    private HeptagonCreator m_HeptagonCreator;
    public HeptagonCreator HeptagonCreator
    {
        get
        {
            if (m_HeptagonCreator == null)
            {
                m_HeptagonCreator = FindObjectOfType<HeptagonCreator>();
            }
            return m_HeptagonCreator;
        }
    }

    private Heart m_heart;
    public Heart Heart
    {
        get
        {
            if (m_heart == null)
            {
                m_heart = FindObjectOfType<Heart>();
            }
            return m_heart;
        }
    }

    public void SetPlayerBall(PlayerBall NewPb)
    {
        m_PlayerBall = NewPb;
    }


    public void SaveData()
    {
        if (!PlayerPrefs.HasKey("HighScore"))
        {
            PlayerPrefs.SetInt("SelectedBall", 0);
            PlayerPrefs.SetInt("Ball0", 1);
            PlayerPrefs.SetInt("Ball1", 0);
            PlayerPrefs.SetInt("Ball2", 0);
            PlayerPrefs.SetInt("HighScore", 0);
            PlayerPrefs.SetString("CurrentPlayerSkin", "Default");
        }
        if (PlayerPrefs.GetInt("level") == 0)
        {
            level = 1;
        }
        else
        {
            level = PlayerPrefs.GetInt("level");
        }
        diamond = PlayerPrefs.GetInt("diamond");
    }

    public void SetDiamond(int diamond)
    {
        this.diamond = diamond;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public void IncreaseDiamond(int amount)
    {
        GameManagerInGame.Instance.AudioManager.kristal.Play();
        GameManagerInGame.Instance.diamond += amount;

        diamondText.text = GameManagerInGame.Instance.diamond.ToString();
        PlayerPrefs.SetInt("diamond", GameManagerInGame.Instance.diamond);
    }
}
