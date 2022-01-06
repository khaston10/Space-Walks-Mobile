using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalController : MonoBehaviour
{
    public static GlobalController Instance;

    #region Global Variables
    public int currentPuzzleDifficulty;

    public int amountOfEasySolved;
    public int amountOfMediumSolved;
    public int amountOfHardSolved;

    public float averageTimeForEasy;
    public float averageTimeForMed;
    public float averageTimeForHard;

    public float musicVolume;
    public float sfxVolume;

    #endregion


    private void Awake()
    {
        amountOfEasySolved = PlayerPrefs.GetInt("amountOfEasySolved");
        amountOfMediumSolved = PlayerPrefs.GetInt("amountOfMediumSolved");
        amountOfHardSolved = PlayerPrefs.GetInt("amountOfHardSolved");
        averageTimeForEasy = PlayerPrefs.GetFloat("averageTimeForEasy");
        averageTimeForMed = PlayerPrefs.GetFloat("averageTimeForMed");
        averageTimeForHard = PlayerPrefs.GetFloat("averageTimeForHard");

        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }
}
