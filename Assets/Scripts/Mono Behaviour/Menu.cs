using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] LeaderCardSelectionMenu fidel;
    [SerializeField] LeaderCardSelectionMenu batista;

    public void LocalMultiplayer()
    {
        if (fidel.CheckStartGame() && batista.CheckStartGame()) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Exit()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}
