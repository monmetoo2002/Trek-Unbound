using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TutorialsZone : MonoBehaviour
{
    public GameObject player;
    public List<Image> displayImages; // Danh sách các Image

    private bool isPlayerInZone = false;
    void Start()
    {
        foreach (var image in displayImages)
        {
            image.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            isPlayerInZone = true;
            ShowImages();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            isPlayerInZone = false;
            HideImages();
        }
    }

    public void ShowImagesIfPlayerInZone()
    {
        if (isPlayerInZone)
        {
            ShowImages();
        }
    }

    void ShowImages()
    {
        foreach (var image in displayImages)
        {
            image.gameObject.SetActive(true);
        }
    }

    public void HideImages()
    {
        foreach (var image in displayImages)
        {
            image.gameObject.SetActive(false);
        }
    }
}