using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        // Sử dụng Path.Combine để truy cập vào các hệ điều hành khác nhau có những path khác nhau
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // Tải dữ liệu được tuần tự hóa thành file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Chuyển dữ liệu từ Json về C#
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Phat hien loi khi co gang de luu du lieu ve file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        // Sử dụng Path.Combine để truy cập vào các hệ điều hành khác nhau có những path khác nhau
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // Tạo đường dẫn cho thư muc phòng tường hợp nó chưa tồn tại trên máy tính 
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Tuần tự hóa dữ liệu trò chơi thành chuỗi JSON
            string dataToStore = JsonUtility.ToJson(data, true);

            // Viết dữ liệu ra file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Phat hien loi khi co gang de luu du lieu ve file: " + fullPath + "\n" + e);
        }
    }
}