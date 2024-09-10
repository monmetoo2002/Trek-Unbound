using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;
    private int currentGemCount;
    private int savedGemCount;

    private HashSet<string> temporaryCollectedGems = new HashSet<string>(); // Lưu trữ tạm thời các viên gem đã thu thập
    private HashSet<string> collectedGems = new HashSet<string>(); // Lưu trữ vĩnh viễn các viên gem đã thu thập sau khi qua checkpoint
    private Dictionary<string, Gem> gemDictionary = new Dictionary<string, Gem>();

    public TextMeshProUGUI gemText;
    public TextMeshProUGUI dieText;
    public GameObject gemImage;
    public GameObject dieImage;
    public PauseMenu pauseMenu;
    private int deathCount;
    private Vector3 lastCheckpointPosition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadData(GameData data)
    {
        this.deathCount = data.deathCount;
        this.lastCheckpointPosition = data.lastCheckpointPosition;

        currentGemCount = 0; // Đặt lại số lượng gem trước khi tính toán lại
        collectedGems.Clear(); // Xóa các gem đã thu thập trước đó
        temporaryCollectedGems.Clear(); // Xóa các gem tạm thời đã thu thập


        foreach (var gem in FindObjectsOfType<Gem>())
        {
            gemDictionary[gem.GetId()] = gem;
            if (data.gemsCollected.TryGetValue(gem.GetId(), out bool collected) && collected)
            {
                gem.gameObject.SetActive(false);
                currentGemCount++;
                collectedGems.Add(gem.GetId());
            }
        }
        UpdateGemText(); // Cập nhật giao diện người dùng với số lượng đá quý hiện tại

        
        // Di chuyển nhân vật tới checkpoint cuối cùng
        if (SceneManager.GetActiveScene().buildIndex == data.lastPlayedLevel)
        {
            transform.position = lastCheckpointPosition;
        }

    }

    public void SaveData(GameData data)
    {
        data.deathCount = this.deathCount;
        data.lastCheckpointPosition = this.lastCheckpointPosition;

        foreach (var gem in gemDictionary.Values)
        {
            data.gemsCollected[gem.GetId()] = collectedGems.Contains(gem.GetId());
        }
    }

    private void Start()
    {
        UpdateGemText();
        UpdateDieText();
    }

    private void Update()
    {
        UpdateGemText();
        UpdateDieText();
    }

    public void AddGem(Gem gem)
    {
        currentGemCount++;
        temporaryCollectedGems.Add(gem.GetId()); // Tạm thời đánh dấu gem đã thu thập
        gem.gameObject.SetActive(false); // Đảm bảo gem không xuất hiện lại trong lúc chơi
        UpdateGemText();
    }

    public void SaveGemCount()
    {
        savedGemCount = currentGemCount;
        // Chuyển các gem thu thập tạm thời vào lưu trữ vĩnh viễn sau khi qua checkpoint
        collectedGems.UnionWith(temporaryCollectedGems);
        temporaryCollectedGems.Clear();
    }

    public void ResetGemCountToLastCheckpoint()
    {
        currentGemCount = savedGemCount;
        UpdateGemText();

        // Hồi sinh lại chỉ những viên gem đã thu thập từ sau checkpoint gần nhất
        foreach (var gemId in temporaryCollectedGems)
        {
            if (gemDictionary.TryGetValue(gemId, out Gem gem))
            {
                gem.gameObject.SetActive(true);
            }
        }
        temporaryCollectedGems.Clear(); // Xóa bộ sưu tập tạm thời khi reset
    }

    private void UpdateGemText()
    {
        gemText.text = currentGemCount.ToString();
    }

    private void UpdateDieText()
    {
        dieText.text = deathCount.ToString();
    }

    public void IncreaseDieCount()
    {
        deathCount++;
        DataPersistenceManager.instance.SaveGame();
        UpdateDieText();
    }

    public int GetCurrentGemCount()
    {
        return currentGemCount;
    }

    public int GetDieCount()
    {
        return deathCount;
    }

    public void UpdateLastCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        DataPersistenceManager.instance.UpdateLastPlayedLevel(SceneManager.GetActiveScene().buildIndex); // Cập nhật level hiện tại
        DataPersistenceManager.instance.SaveGame(); // Lưu lại toàn bộ game
    }

    public Vector3 GetLastCheckpointPosition()
    {
        return lastCheckpointPosition;
    }


    public void HideGameUI()
    {
        gemText.gameObject.SetActive(false);
        dieText.gameObject.SetActive(false);
        gemImage.SetActive(false);
        dieImage.SetActive(false);
        pauseMenu.HidePauseButton();
    }
}
