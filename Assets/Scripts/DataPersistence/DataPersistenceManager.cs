using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]

    [SerializeField] private string fileName;

    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Tim thay hon 1 Data Persistence Manager trong scene nay");
        }
        instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Tải bất kỳ dữ liệu nào từ file sử dụng trình quản lý dữ liệu
        this.gameData = dataHandler.Load();

        // Nếu không có bất kỳ dữ liệu nào được tải, khởi chạy New Game

        if (this.gameData == null)
        {
            Debug.Log("Khong co du lieu. Khoi tao du lieu ve mac dinh.");
            NewGame();
        }

        // Đẩy dữ liệu đã được tải đến tất cả các scripts khác cần nó
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded death count = " + gameData.deathCount);
    }

    public void SaveGame()
    {
        // Chuyển dữ liệu sang các scripts khác để chúng có thể cập nhật
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        Debug.Log("Saved death count = " + gameData.deathCount);

        // Lưu dữ liệu vào 1 file sử dụng trình xử lý dữ liệu tệp
        dataHandler.Save(gameData);
    }

    public void ResetGameData()
    {
        this.gameData = new GameData();
        SaveGame(); // Lưu dữ liệu đã reset

        // Reset dữ liệu trong tất cả các đối tượng
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Đã reset dữ liệu trò chơi");
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void UpdateLastPlayedLevel(int levelId)
    {
        if (this.gameData != null)
        {
            this.gameData.lastPlayedLevel = levelId;
            SaveGame();
        }
    }

    public Vector3 GetLastCheckpointPosition()
    {
        // Trả về vị trí checkpoint cuối cùng nếu dữ liệu game tồn tại
        return this.gameData != null ? this.gameData.lastCheckpointPosition : Vector3.zero;
    }

    // Thêm phương thức để lấy màn chơi cuối cùng
    public int GetLastPlayedLevel()
    {
        return this.gameData != null ? this.gameData.lastPlayedLevel : 1;
    }
}