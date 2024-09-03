using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleClick : MonoBehaviour
{
    MasterController masterController;
    CardController cardController;
    Camera mainCamera;
    float firstClickTime = 0;
    float limitTime = 0.25f;
    bool check = true;
    int clickCount = 0;

    bool active => this.gameObject.name.Contains("Fidel") ? !Board.Instance.IsBatistaPlayingOrAboutToPlay : Board.Instance.IsBatistaPlayingOrAboutToPlay;

    private void Start()
    {
        cardController = this.gameObject.GetComponent<CardController>();
        masterController = GameObject.Find("Game Manager").GetComponent<MasterController>();
        mainCamera = this.gameObject.name.Contains("Fidel") ? masterController.cameras[1] : masterController.cameras[2];
    }

    private void Update()
    {
        if (active && Input.GetMouseButtonUp(0) && !(masterController.IsAnyInfoActive()) && masterController.IsPlayersPanelActive())
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == gameObject)
                {
                    clickCount++;
                    if (check && clickCount == 1)
                    {
                        firstClickTime = Time.time;
                        StartCoroutine(DoubleClickAction());
                    }
                }
            }
        }
    }

    IEnumerator DoubleClickAction()
    {
        check = false;
        while (Time.time - firstClickTime < limitTime)
        {
            if (clickCount>=2)
            {
                masterController.SavePanelOnWhenInformationDisplayed(false);
                masterController.OpenInfo(cardController.Info);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        clickCount = 0;
        check = true;
    }
}
