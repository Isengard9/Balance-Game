using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [SerializeField] Slider balanceBar;

    #endregion


    [SerializeField] float minMaxRotValue = 50;

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
        MaxRotationControl();
    }

    /// <summary>
    /// Syncs balanceBar with rotation
    /// </summary>
    public void BalanceBarControl(float value)
    {
        balanceBar.value = value * -1;
    }

    /// <summary>
    /// Controls the rotation of player
    /// </summary>
    public void MaxRotationControl()
    {
        if(balanceBar.value >= minMaxRotValue || balanceBar.value <= (minMaxRotValue * -1))
        {
            OnLevelFailed();
        }
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
        WinPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RetryButton()
    {
        LostPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region OnLevel..

    public void OnLevelSuccessed()
    {
        PlayerManager.instance.FinishProcess();

        GamePanel.SetActive(false);
        WinPanel.SetActive(true);

        isGameEnded = true;
    }

    public void OnLevelFailed()
    {
        PlayerManager.instance.FailProcess();

        GamePanel.SetActive(false);
        LostPanel.SetActive(true);

        isGameEnded = true;
    }

    #endregion
}
