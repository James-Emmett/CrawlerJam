using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { MainMenu, Playing, Inventory, Dead}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Player      m_Player = null;
    public ItemFactory  m_ItemFactory;
    public Dungeon      m_Dungeon;
    private AudioSource m_AudioSource = null;
    public GameState   m_GameState = GameState.MainMenu;
    public GameObject   m_StartScreen = null;
    public GameObject   m_OverState = null;

    public GameState GameState
    {
        get { return m_GameState; }
        set { m_GameState = value; RefreshState(); }
    }

    public Player Player 
    {
        get
        {
            if (m_Player == null)
            {
                m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            }

            return m_Player;
        }
    }

    public Inventory Inventory
    {
        get { return Player.m_Inventory; }
    }

    public ItemFactory ItemFactory
    {
        get { return m_ItemFactory; }
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource   = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshState();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void PlaySound(AudioClip clip, float pitch = 1)
    {
        m_AudioSource.pitch = pitch;
        m_AudioSource.PlayOneShot(clip);
    }

    public void RefreshState()
    {
        // Basically set the UI / HUD stuffs
        switch (m_GameState)
        {
            case GameState.MainMenu:

                if (Input.anyKey)
                {
                    m_StartScreen.SetActive(false);
                    m_Dungeon.GenerateNewDungeon();
                    m_GameState = GameState.Playing;
                }
                Inventory.m_Panel.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.Playing:
                Inventory.m_Panel.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case GameState.Inventory:
                Inventory.m_Panel.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.Dead:
                m_OverState.SetActive(true);
                if(Input.anyKey)
                {
                    m_OverState.SetActive(false);
                    m_Player.Initialize();
                    m_Dungeon.GenerateNewDungeon();
                    m_GameState = GameState.Playing;
                }
                break;
            default:
                Inventory.m_Panel.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
    }

    public void NextMap()
    {
        m_Dungeon.GenerateNewDungeon();
    }
}
