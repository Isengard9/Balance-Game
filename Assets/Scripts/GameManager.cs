using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool isGameStarted = false;
    public static bool isGameEnded = false;

    #region Canvas

    [SerializeField] GameObject StartPanel;

    #endregion


    public int RightBoxCount, LeftBoxCount = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    #region Buttons

    public void StartButton()
    {
        isGameStarted = true;
        isGameEnded = false;
        StartPanel.SetActive(false);
    }

    #endregion
}
