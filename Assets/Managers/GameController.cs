using Assets.Scripts.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour {
    

    public int Score { get; private set; }
    public bool IsPaused { get; private set; }

    public int floor = 0;
    public int seed = 1337;

    public delegate void OnPauseEventHandler();
    public OnPauseEventHandler OnPause;
    public delegate void OnResumeEventHandler();
    public OnResumeEventHandler OnResume;

    static bool comboActive = false;
    static int combo = 1;
    static float comboTime = 0;

    public Text comboCounter;
    public Text comboTimer;
    public Text scoreCounter;
    public Text floorCounter;

    public Text resultFloor;
    public Text resultScore;

    public GameObject mainMenu;
    public GameObject gameScreen;
    public GameObject levelUpScreen;
    public GameObject pauseMenuScreen;
    public GameObject gameOverScreen;

    public EnemyStats chaserStats;
    public EnemyStats shooterStats;
    public EnemyStats bomberStats;

    GameObject currentScreen;

    System.Random random;
    GameObject map;

	// Use this for initialization
	void Start () {
        map = GameObject.Find("Map");
        comboCounter = GameObject.Find("/UI/GameScreen/ComboBox/ComboCounter").GetComponent<UnityEngine.UI.Text>();
        comboTimer = GameObject.Find("/UI/GameScreen/ComboBox/ComboTimer").GetComponent<UnityEngine.UI.Text>();

        currentScreen = mainMenu;
        currentScreen.SetActive(true);
        gameScreen.SetActive(false);
        levelUpScreen.SetActive(false);
        pauseMenuScreen.SetActive(false);
        gameOverScreen.SetActive(false);

	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Escape) && currentScreen == gameScreen)
        {
            GotoPauseMenu();
        }

        if (!IsPaused)
        {
            CountdownCombo();
            if (comboActive)
            {
                comboTimer.text = (comboTime - Time.time).ToString("0.00");
            }
        }
	}

    public void NewGame()
    {
        floor = 0;
        Score = 0;
        seed = System.DateTime.Now.Millisecond;
        random = new System.Random(seed);

        var mapgen = map.GetComponent<Map>();
        mapgen.GenerateMap(random);

        scoreCounter.text = "Score: " + Score;
        floorCounter.text = "Floor: " + floor;
    }

    public void NextFloor()
    {
        chaserStats.LevelUpAttribute((EnemyAttribute)random.Next(0, 3));
        shooterStats.LevelUpAttribute((EnemyAttribute)random.Next(0, 3));
        bomberStats.LevelUpAttribute((EnemyAttribute)random.Next(0, 3));

        floor++;
        floorCounter.text = "Floor: " + floor;

        var mapgen = map.GetComponent<Map>();
        mapgen.GenerateMap(random);
    }

    public void RetryGame()
    {
        floor = 0;
        Score = 0;
        random = new System.Random(seed);

        var mapgen = map.GetComponent<Map>();
        mapgen.GenerateMap(random);
    }

    public void GotoPauseMenu()
    {
        Pause();
        currentScreen.SetActive(false);
        currentScreen = pauseMenuScreen;
        currentScreen.SetActive(true);

    }

    public void GotoLevelUpMenu()
    {
        Pause();
        currentScreen.SetActive(false);
        currentScreen = levelUpScreen;
        currentScreen.SetActive(true);
    }

    public void GotoGameOver()
    {
        Pause();
        currentScreen.SetActive(false);
        currentScreen = gameOverScreen;
        currentScreen.SetActive(true);

        resultFloor.text = "Floor: " + floor.ToString();
        resultScore.text = "Score: " + Score.ToString();
    }

    public void GotoMainMenu()
    {
        Pause();
        currentScreen.SetActive(false);
        currentScreen = mainMenu;
        currentScreen.SetActive(true);
    }

    public void GotoGame()
    {
        Resume();
        currentScreen.SetActive(false);
        currentScreen = gameScreen;
        currentScreen.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Pause()
    {
        if (!IsPaused)
        {
            IsPaused = true;
            Time.timeScale = 0f;
            OnPause();
        }
    }

    void Resume()
    {
        if (IsPaused)
        {
            IsPaused = false;
            Time.timeScale = 1f;
            OnResume();
        }
    }

    public void IncreaseCombo()
    {
        comboActive = true;
        comboTime = Time.time + 5f;
        combo++;

        comboCounter.text = combo.ToString() + "x";
    }

    public void CountdownCombo()
    {
        if (comboActive)
        {
            if(Time.time > comboTime)
            {
                combo = 0;
                comboTime = 0;
                comboActive = false;
                comboCounter.text = "";
                comboTimer.text = "";
            }
        }
    }

    public void AddScore(int score)
    {
        Score += (score * combo);

        scoreCounter.text = "Score: " + Score;
    }
}
