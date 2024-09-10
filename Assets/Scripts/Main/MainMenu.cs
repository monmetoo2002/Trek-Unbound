using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Select Chapter");
    }

    public void ContinueGame()
    {
        int lastPlayedLevel = DataPersistenceManager.instance.GetLastPlayedLevel();
        Vector3 lastCheckpointPosition = DataPersistenceManager.instance.GetLastCheckpointPosition();

        // Chỉ tải level nếu dữ liệu hợp lệ
        if (lastPlayedLevel > 0)
        {
            SceneManager.LoadSceneAsync(lastPlayedLevel);
            StartCoroutine(MovePlayerToCheckpoint(lastCheckpointPosition));
        }
        else
        {
            SceneManager.LoadSceneAsync(1);
        }
    }

    private IEnumerator MovePlayerToCheckpoint(Vector3 checkpointPosition)
    {
        yield return new WaitForSeconds(0.1f); // Đợi scene load hoàn toàn
        transform.position = checkpointPosition;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    

}
