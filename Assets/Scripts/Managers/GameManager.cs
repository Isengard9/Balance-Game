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
    [SerializeField] GameObject GamePanel;
    [SerializeField] GameObject WinPanel;
    [SerializeField] GameObject LostPanel;

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
        StartPanel.SetActive(false);
        GamePanel.SetActive(true);
        isGameStarted = true;
        isGameEnded = false;
        
    }

    public void NextButton()
    {

    }

    public void RetryButton()
    {

    }

    #endregion

    #region OnLevel..

    public void OnLevelSuccessed()
    {
        GamePanel.SetActive(false);
        WinPanel.SetActive(true);

        isGameEnded = true;
    }

    public void OnLevelFailed()
    {
        GamePanel.SetActive(false);
        LostPanel.SetActive(true);

        isGameEnded = true;
    }

    #endregion
}
