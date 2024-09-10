using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseButton;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;

        // Ẩn các hình ảnh của TutorialsZone khi pause menu hiện lên
        HideAllTutorialZones();
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

        ShowAllTutorialZones();
    }
    private void HideAllTutorialZones()
    {
        // Tìm tất cả các TutorialsZone trong scene và ẩn hình ảnh của chúng
        TutorialsZone[] allTutorialZones = FindObjectsOfType<TutorialsZone>();
        foreach (TutorialsZone zone in allTutorialZones)
        {
            zone.HideImages();
        }
    }
    private void ShowAllTutorialZones()
    {
        TutorialsZone[] allTutorialZones = FindObjectsOfType<TutorialsZone>();
        foreach (TutorialsZone zone in allTutorialZones)
        {
            zone.ShowImagesIfPlayerInZone();
        }
    }

    public void HidePauseButton()
    {
        pauseButton.SetActive(false);
    }

    public void Home()
    {
        // Lưu dữ liệu trò chơi trước khi quay lại Main Menu
        DataPersistenceManager.instance.SaveGame();

        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }

    public void SelectChapter()
    {
        SceneManager.LoadScene("Select Chapter");
        Time.timeScale = 1;
    }
}
