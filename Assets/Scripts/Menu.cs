using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void LocalMultiplayer()
    {
        if (LeaderCardSelectionMenu.CheckStartGame()) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Exit()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}