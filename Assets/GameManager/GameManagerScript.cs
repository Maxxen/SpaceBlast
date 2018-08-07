using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public int floor = 0;
    public int seed = 1337;
    public static int score = 0;
    static bool comboActive = false;
    static int combo = 1;
    static float comboTime = 0;
    const float BASE_COMBO_TIME = 2.5f;

    System.Random random;
    GameObject map;

    static UnityEngine.UI.Text comboCounter;
    static UnityEngine.UI.Text comboTimer;

	// Use this for initialization
	void Start () {
        map = GameObject.Find("Map");
        comboCounter = GameObject.Find("/UI/GameHUD/ComboBox/ComboCounter").GetComponent<UnityEngine.UI.Text>();
        comboTimer = GameObject.Find("/UI/GameHUD/ComboBox/ComboTimer").GetComponent<UnityEngine.UI.Text>();
        StartGame();
	}

    public void StartGame()
    {
        random = new System.Random(seed);
        var mapgen = map.GetComponent<Map>();
        mapgen.GenerateMap(random);
    }
	
	// Update is called once per frame
	void Update () {
        CountdownCombo();
        if (comboActive)
        {
            comboTimer.text = (comboTime - Time.time).ToString("0.00");
        }
	}

    public static void IncreaseCombo()
    {
        comboActive = true;
        comboTime = Time.time + (BASE_COMBO_TIME + (combo * 1.05f));
        combo++;

        comboCounter.text = combo.ToString() + "x";
    }

    void CountdownCombo()
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
}
