using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] startButtons;
    
    private void Awake()
    {
        
        if (!PlayerPrefs.HasKey("FirstPlay"))
        {
            // Lần đầu chơi, xóa toàn bộ dữ liệu và thiết lập giá trị mặc định
            PlayerPrefs.DeleteAll(); 
            PlayerPrefs.SetInt("FirstPlay", 1); 
            PlayerPrefs.SetInt("UnlockedLevel", 1); 
            PlayerPrefs.Save();
        }
        
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for(int i=0; i < startButtons.Length; i++)
        {
            startButtons[i].interactable = false;
        }

        for(int i=0; i < Mathf.Min(unlockedLevel, startButtons.Length); i++) 
        {
            startButtons[i].interactable = true;
        }
    }
    
    public void OpenLevel(int levelId)
    {
        DataPersistenceManager.instance.ResetGameData(); // Reset toàn bộ dữ liệu trò chơi
        string levelName = "Level " + levelId;
        SceneManager.LoadSceneAsync(levelName);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }
}
