using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPlayerInfo : MonoBehaviour
{
    Sprite Info;
    GameObject GameManager;
    MasterController masterController;

    private void Start()
    {
        if (this.gameObject.tag == "Fidel Body") Info = Resources.Load<Sprite>("Info/Fidel/Fidel Castro");
        else if (this.gameObject.tag == "Batista Body") Info = Resources.Load<Sprite>("Info/Batista/Fulgencio Batista");

        GameManager = GameObject.Find("Game Manager");
        masterController = GameManager.GetComponent<MasterController>();
    }

    private void OnMouseDown()
    {
        if (masterController.IsAnyInfoActive()) return;

        masterController.SavePanelOnWhenInformationDisplayed(false);
        masterController.OpenInfo(this.Info);
    }
}
