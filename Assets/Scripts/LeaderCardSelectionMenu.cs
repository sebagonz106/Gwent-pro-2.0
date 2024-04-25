using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderCardSelectionMenu : MonoBehaviour
{
    static string rebelLeader = "";
    static string batistaLeader = "";

    public void SaveInfo()
    {
        PlayerPrefs.SetString("Rebel Leader", rebelLeader);
        PlayerPrefs.SetString("Batista Leader", batistaLeader);
    }

    public void SelectCamilo()
    {
        rebelLeader = "Camilo Cienfuegos";
    }
    public void SelectChe()
    {
        rebelLeader = "Ernesto Che Guevara";
    }
    public void SelectCantillo()
    {
        batistaLeader = "Eulogio Cantillo";
    }
    public void SelectTabernilla()
    {
        batistaLeader = "Francisco Tabernilla";
    }
    public static bool CheckStartGame() => (rebelLeader != "") && (batistaLeader != "");
}
