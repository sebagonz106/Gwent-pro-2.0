using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderCardSelectionMenu : MonoBehaviour
{
    public string leader = "";
    [SerializeField] TMP_InputField input;
    [SerializeField] string faction;

    private void Update()
    {
        input.gameObject.SetActive(PlayerMB.Leaders.Count > 4);
    }

    public void SaveInfo()
    {
        PlayerPrefs.SetString(faction  + " Leader", leader);
    }

    public void SelectCamilo()
    {
        leader = "Camilo Cienfuegos";
    }
    public void SelectChe()
    {
        leader = "Ernesto Che Guevara";
    }
    public void SelectCantillo()
    {
        leader = "Eulogio Cantillo";
    }
    public void SelectTabernilla()
    {
        leader = "Francisco Tabernilla";
    }
    public void SelectCreated()
    {
        if (input.text.Length == 0) return;

        if (PlayerMB.Leaders.ContainsKey(input.text.Trim()))
        {
            if (PlayerMB.Leaders[input.text].Faction == faction)
            {
                leader = input.text;
                input.text = "";
                input.placeholder.GetComponent<TMP_Text>().text = "Carta deseada:";
            }
            else
            {
                string name = input.text;
                input.text = "";
                input.placeholder.GetComponent<TMP_Text>().text = "La carta '" + name + "' pertenece a otra facción.";
            }
        }
        else
        {
            string name = input.text;
            input.text = "";
            input.placeholder.GetComponent<TMP_Text>().text = "La carta '" + name + "' no existe.";
        }
    }

    public bool CheckStartGame()
    {
        if (leader != "")
        {
            leader = "";
            return true;
        }
        else return false;
    }
}
