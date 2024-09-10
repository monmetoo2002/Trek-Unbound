using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour
{
    public GameObject darkPanel;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI gemCountText;
    public TextMeshProUGUI deathCountText;

    private bool isTouchFinishLine = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && isTouchFinishLine)
        {
            isTouchFinishLine = false;

            ShowDarkPanel();
            ShowWinScreen();

            UnlockNewLevel();
            Time.timeScale = 0;

        }
        else
        {
            return;
        }
    }

    void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex", 1))
        {
            // Nếu màn chơi hiện tại lớn hơn hoặc bằng màn chơi cao nhất đã hoàn thành, thì mở khóa màn tiếp theo
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
    private void ShowDarkPanel()
    {
        darkPanel.gameObject.SetActive(true);
    }    
    private void ShowWinScreen()
    {
        // Ẩn UI của game
        GameManager.instance.HideGameUI();

        winText.gameObject.SetActive(true);

        // Hiển thị số gem nhặt được
        int gemCount = GameManager.instance.GetCurrentGemCount();
        gemCountText.text = "Gems collected: " + gemCount;
        gemCountText.gameObject.SetActive(true);

        // Hiển thị số lần chết
        int deathCount = GameManager.instance.GetDieCount();
        deathCountText.text = "Deaths: " + deathCount;
        deathCountText.gameObject.SetActive(true);        
    }
    public void SelectChapter()
    {
        SceneManager.LoadScene("Select Chapter");
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }
}
