using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    CardsPositionWarehouse parent;
    Board board;
    Player player;
    GameObject GameManager;
    public int indexOfThisInParent;
    public List<Zone> rangeTypes;
    public Sprite Info;
    public bool isSelected;
    Vector3 upPosition = new Vector3();
    float speed = 15.0f; // Speed of the movement
    public bool isOccupied = false;

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }

    private void Start()
    {
        upPosition.x = this.transform.position.x;
        upPosition.y = this.transform.position.y;
        upPosition.z = this.transform.position.z;
        parent = this.gameObject.transform.parent.gameObject.GetComponent<CardsPositionWarehouse>();
        GameManager = GameObject.Find("Game Manager");
        board = GameManager.GetComponent<MasterController>().board;
        player = parent.tag.Contains("Batista") ? board.Batista : board.Fidel;
        indexOfThisInParent = (this.gameObject.tag == "WeatherCard" || this.gameObject.tag == "BonusCard")? 2 - Array.IndexOf(parent.positions, this.gameObject) : Array.IndexOf(parent.positions, this.gameObject);

        if (this.gameObject.tag == "BattlefieldCard" || this.gameObject.tag == "WeatherCard" || this.gameObject.tag == "BonusCard")
        {
            if (this.gameObject.name.Contains("Melee"))      this.rangeTypes = new List<Zone> { Zone.Melee } ;
            else if (this.gameObject.name.Contains("Range")) this.rangeTypes = new List<Zone> { Zone.Range } ;
            else                                             this.rangeTypes = new List<Zone> { Zone.Siege } ;
        }

        else if (this.gameObject.tag == "LeaderCard")
        {
            this.GetComponent<Renderer>().material = this.player.Leader.material;
            this.Info = this.player.Leader.information;
            this.IsOccupied = true;
        }
    }

    private void OnMouseDown()
    {
        if (this.board.masterController.IsAnyInfoActive() || !(this.board.masterController.IsPlayersPanelActive()||this.player.LeaderCardSelected)) return;

        #region Leaders
        if(this.gameObject.tag == "LeaderCard")
        {
            if(!GameManager.GetComponent<MasterController>().validTurn && player.Equals(board.GetCurrentPlayer()) && !player.LeaderEffectUsedThisRound)
            {
                    OpenInfoPanel(true);
            }
            else OpenInfoPanel();
        }
        #endregion

        #region Hand cards
        else if (this.gameObject.tag == "HandCard")
        {
            if (player.LeaderCardSelected)
            {
                player.LeaderCardSelected = false;
                GameManager.GetComponent<MasterController>().EffectException();
                return;
            }
            if (!isSelected)
            {
                foreach (GameObject card in parent.positions)
                {
                    CardController cardController = card.GetComponent<CardController>();
                    if (cardController.isSelected)
                    {
                        Disable(cardController); //disables every other selected card in hand
                        break; //this is going to run every time a hand card is selected, so only one card will need to be disabled in worst case scenario
                    }
                }

                isSelected = true;
                upPosition.y = this.transform.position.y + 5;
                float step = speed * Time.deltaTime; // Calculate the step size

                // Move our position a step closer to the target.
                transform.position = Vector3.MoveTowards(transform.position, upPosition, step);
            }
            else Disable(this);
        }
        #endregion

        #region Battlfield and weather cards
        else if (this.gameObject.tag == "BattlefieldCard" || this.gameObject.tag == "WeatherCard" || this.gameObject.tag == "BonusCard")
        {
            if (this.gameObject.tag == "WeatherCard" && !this.isOccupied) player = board.GetCurrentPlayer(); //empty slots of weather cards can be filled
                                                                                                            //by both players, so empty weather cards player will 
                                                                                                           //update every time a weather card is clicked

            if (player.LeaderCardSelected)
            {
                if (!isOccupied)
                {
                    player.LeaderCardSelected = false;
                    GameManager.GetComponent<MasterController>().EffectException();
                    return;
                }
                else
                {
                    List<Card> list = this.gameObject.tag == "WeatherCard" ? board.Weather : 
                                      this.gameObject.tag == "BonusCard" ? this.player.Battlefield.Bonus : Utils.GetListByRangeType(this.player, this.rangeTypes[0]);

                    GameManager.GetComponent<LeaderSkillPanel>().StaysInBattlefieldModifierSkill(list[indexOfThisInParent], list);
                    player.LeaderCardSelected = false;
                    return;
                }
            }

            if (!GameManager.GetComponent<MasterController>().validTurn)
            {
                #region Play normal card
                if (!isOccupied)
                {
                    GameObject collection;

                    if (this.player.PlayerName == Faction.Batista) collection = GameObject.Find("Batista Hand");
                    else                                           collection = GameObject.Find("Fidel Hand");

                    for (int i = 0; i < collection.transform.childCount; i++)
                    {
                        GameObject child = collection.transform.GetChild(i).gameObject;

                        if (child.GetComponent<CardController>().isSelected && child.GetComponent<CardController>().rangeTypes.Contains(this.rangeTypes[0]))
                        {
                            if ((this.gameObject.tag == "WeatherCard" && player.Hand[i] is WeatherCard) ||
                                (this.gameObject.tag == "BattlefieldCard" && !(new List<CardType> { CardType.Bait, CardType.Bonus, CardType.Weather }).Contains(player.Hand[i].CardType)) ||
                                (this.gameObject.tag == "BonusCard" && player.Hand[i] is BonusCard))
                            {
                                if (this.player.PlayCard(i, indexOfThisInParent, this.rangeTypes[0]))
                                {
                                    child.SetActive(false); //just in case jeje
                                    this.IsOccupied = true;
                                    GameManager.GetComponent<MasterController>().validTurn = true;
                                }
                                else GameManager.GetComponent<MasterController>().GeneralException();
                            }
                            else GameManager.GetComponent<MasterController>().GeneralException();
                            break;
                        }
                    }
                }
                #endregion

                #region Play bait card
                else
                {
                    bool BaitFound = false;
                    GameObject collection;

                    if (this.player.PlayerName == Faction.Batista) collection = GameObject.Find("Batista Hand");
                    else                                           collection = GameObject.Find("Fidel Hand");

                    for (int i = 0; i < collection.transform.childCount; i++)
                    {
                        GameObject child = collection.transform.GetChild(i).gameObject;
                        if (child.GetComponent<CardController>().isSelected && player.Hand[i] is BaitCard bait)
                        {
                            if (this.gameObject.tag == "WeatherCard" && (!(board.Weather[indexOfThisInParent] is WeatherCard weather) || !weather.PlayerThatPlayedThisCard.Equals(this.player))) break;

                            if (bait.Effect(this.player,
                                            this.gameObject.tag == "WeatherCard" ? board.Weather : this.gameObject.tag == "BonusCard" ? this.player.Battlefield.Bonus : Utils.GetListByRangeType(this.player, this.rangeTypes[0]),
                                            indexOfThisInParent))
                            {
                                BaitFound = true;
                                board.UpdateView(true);
                                GameManager.GetComponent<MasterController>().validTurn = true;
                            }

                            break;
                        }
                    }

                    if (!BaitFound) OpenInfoPanel();
                }
                #endregion
            }
            else if (isOccupied) OpenInfoPanel();
            else GameManager.GetComponent<MasterController>().GeneralException();
        }
        #endregion

        else GameManager.GetComponent<MasterController>().GeneralException();
    }

    public void AssignRangeForHandCard (List<Zone> list) { this.rangeTypes = list; }

    private void Disable(CardController cardController)
    {
        cardController.isSelected = false;
        cardController.upPosition.y = cardController.transform.position.y - 5;
        float step = cardController.speed * Time.deltaTime; // Calculate the step size

        // Move our position a step further from the target.
        cardController.transform.position = Vector3.MoveTowards(cardController.transform.position, cardController.upPosition, step);
    }

    private void OpenInfoPanel(bool leader = false)
    {
        if (!isOccupied)
        {
            GameManager.GetComponent<MasterController>().GeneralException();
            return;
        }

        GameManager.GetComponent<MasterController>().SavePanelOnWhenInformationDisplayed();
        GameManager.GetComponent<MasterController>().PanelOnWhenInformationDisplayed.SetActive(false);

        GameObject infoPanel = leader ? GameManager.GetComponent<MasterController>().LeaderInfoPanel : GameManager.GetComponent<MasterController>().InfoPanel;
        infoPanel.GetComponent<Image>().sprite = this.Info;
        infoPanel.SetActive(true);
    }
}
