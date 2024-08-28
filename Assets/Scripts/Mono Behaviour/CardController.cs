using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    CompiledCardVisual visual;
    Material rebelMaterial;
    Material batistaMaterial;
    CardsPositionWarehouse parent;
    Board board;
    Player player;
    PlayerMB playerMB;
    GameObject GameManager;
    MasterController masterController;
    public int indexOfThisInParent;
    public List<Zone> rangeTypes;
    public Sprite Info;
    public bool isSelected;
    Vector3 upPosition = new Vector3();
    float speed = 15.0f; // Speed of the movement
    public bool isOccupied = false;

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }

    private void Awake() //maybe solves the visual problem on hands first activation
    {
        rebelMaterial = Resources.Load<Material>("Compiler prefabs/rebel new card");
        batistaMaterial = Resources.Load<Material>("Compiler prefabs/batista new card");
        visual = this.GetComponentInChildren<CompiledCardVisual>();
        visual.gameObject.SetActive(false);
    }

    private void Start()
    {
        upPosition.x = this.transform.position.x;
        upPosition.y = this.transform.position.y;
        upPosition.z = this.transform.position.z;
        parent = this.gameObject.transform.parent.gameObject.GetComponent<CardsPositionWarehouse>();
        GameManager = GameObject.Find("Game Manager");
        masterController = GameManager.GetComponent<MasterController>();
        board = Board.Instance;
        playerMB = parent.tag.Contains("Batista") ? masterController.board.Batista : masterController.board.Fidel;
        player = playerMB.player;
        indexOfThisInParent = Array.IndexOf(parent.positions, this.gameObject);

        if (this.gameObject.tag == "BattlefieldCard" || this.gameObject.tag == "WeatherCard" || this.gameObject.tag == "BonusCard")
        {
            if (this.gameObject.name.Contains("Melee"))      this.rangeTypes = new List<Zone> { Zone.Melee } ;
            else if (this.gameObject.name.Contains("Range")) this.rangeTypes = new List<Zone> { Zone.Range } ;
            else                                             this.rangeTypes = new List<Zone> { Zone.Siege } ;
        }

        else if (this.gameObject.tag == "LeaderCard")
        {
            if(player.Leader.Info.Material is null)
            {
                gameObject.GetComponent<Renderer>().material = player.Name == "Fidel" ? rebelMaterial : batistaMaterial;
                visual.gameObject.SetActive(true);
                visual.UpdateInfo(player.Leader);
            }
            else
            {
                this.GetComponent<Renderer>().material = player.Leader.Info.Material;
                this.Info = player.Leader.Info.Information;
            }

            this.IsOccupied = true;
        }
    }

    public void OnMouseDown()
    {
        if (this.masterController.IsAnyInfoActive() || !(this.masterController.IsPlayersPanelActive()||this.player.LeaderCardSelected)) return;

        #region Leaders
        if(this.gameObject.tag == "LeaderCard")
        {
            if (player.LeaderCardSelected)
            {
                player.LeaderCardSelected = false;
                masterController.EffectException();
                return;
            }
            else OpenInfoPanel(player.Leader, !board.ValidTurn && player.Equals(board.GetCurrentPlayer()) && !player.LeaderEffectUsedThisRound);
        }
        #endregion

        #region Hand cards
        else if (this.gameObject.tag == "HandCard")
        {
            if (player.LeaderCardSelected)
            {
                player.LeaderCardSelected = false;
                masterController.EffectException();
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
            if (this.gameObject.tag == "WeatherCard" && !this.isOccupied)      //empty slots of weather cards can be filled
            {                                                                 //by both players, so empty weather cards player will
                player = board.GetCurrentPlayer();                           //update every time a weather card is clicked
                playerMB = masterController.board.GetMBPlayer[player];
            }                                
                                                                                

            if (player.LeaderCardSelected)
            {
                if (!isOccupied) GameManager.GetComponent<MasterController>().EffectException();
                else
                {
                    List<Card> list = GetList();

                    GameManager.GetComponent<LeaderSkillPanel>().LeaderSkillWhenCardSelected(player, list[indexOfThisInParent], list);
                }

                player.LeaderCardSelected = false;
                return;
            }

            if (!board.ValidTurn)
            {
                #region Play normal card
                if (!isOccupied)
                {
                    GameObject collection;

                    if (this.player.PlayerFaction == Faction.Batista) collection = GameObject.Find("Batista Hand");
                    else    /*--------------------------------->*/    collection = GameObject.Find("Fidel Hand");

                    for (int i = 0; i < collection.transform.childCount; i++)
                    {
                        GameObject child = collection.transform.GetChild(i).gameObject;

                        if (child.GetComponent<CardController>().isSelected && player.Hand[i].AvailableRange.Contains(this.rangeTypes[0]))
                        {
                            if ((this.gameObject.tag == "WeatherCard" && player.Hand[i] is WeatherCard) ||
                                (this.gameObject.tag == "BattlefieldCard" && !(new List<CardType> { CardType.Bait, CardType.Bonus, CardType.Weather }).Contains(player.Hand[i].CardType)) ||
                                (this.gameObject.tag == "BonusCard" && player.Hand[i] is BonusCard))
                            {
                                if (playerMB.PlayCard(i, indexOfThisInParent, this.rangeTypes[0]))
                                {
                                    child.SetActive(false); //just in case jeje
                                    this.IsOccupied = true;
                                }
                                else masterController.GeneralException();
                            }
                            else masterController.GeneralException();
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

                    if (this.player.PlayerFaction == Faction.Batista) collection = GameObject.Find("Batista Hand");
                    else    /*--------------------------------->*/    collection = GameObject.Find("Fidel Hand");

                    for (int i = 0; i < collection.transform.childCount; i++)
                    {
                        GameObject child = collection.transform.GetChild(i).gameObject;
                        if (child.GetComponent<CardController>().isSelected && player.Hand[i] is BaitCard bait)
                        {
                            if (this.gameObject.tag == "WeatherCard" && (!(board.Weather[indexOfThisInParent] is WeatherCard weather) || !weather.Owner.Equals(this.player))) break;

                            if (bait.Effect(this.player.context.UpdatePlayerInstance(GetList(), GetList()[indexOfThisInParent])))
                            {
                                BaitFound = true;
                                masterController.board.UpdateView(true);
                                board.ValidTurn = true;
                            }

                            break;
                        }
                    }

                    if (!BaitFound) OpenInfoPanel(GetList()[indexOfThisInParent]);
                }
                #endregion
            }
            else if (isOccupied) OpenInfoPanel(GetList()[indexOfThisInParent]);
            else GameManager.GetComponent<MasterController>().GeneralException();
        }
        #endregion

        else GameManager.GetComponent<MasterController>().GeneralException();
    }

    private void Disable(CardController cardController)
    {
        cardController.isSelected = false;
        cardController.upPosition.y = cardController.transform.position.y - 5;
        float step = cardController.speed * Time.deltaTime; // Calculate the step size

        // Move our position a step further from the target.
        cardController.transform.position = Vector3.MoveTowards(cardController.transform.position, cardController.upPosition, step);
    }

    private void OpenInfoPanel(Card card, bool leader = false)
    {
        if (!isOccupied)
        {
            masterController.GeneralException();
            return;
        }

        masterController.SavePanelOnWhenInformationDisplayed(false);
        if (card.Info.Main is null) masterController.OpenInfo(this.Info, leader);
        else masterController.OpenCompiledCardInfo(card, leader);
    }

    public void Occupy(Card card)
    {
        gameObject.SetActive(true);
        IsOccupied = true;
        if (name.Contains("Hand")) this.rangeTypes = card.AvailableRange;

        if(card.Info.Main is null) 
        {
            this.gameObject.GetComponent<Renderer>().material = card.Info.Material;
            Info = card.Info.Information;
        }
        else
        {
            try
            {
                visual.gameObject.SetActive(true);
            }
            catch { Debug.Log(this.name); }
            visual.UpdateInfo(card);
            gameObject.GetComponent<Renderer>().material = (card.FactionEnum is Faction.Fidel)? rebelMaterial : batistaMaterial;
        }
    }

    public void Desoccupy()
    {
        gameObject.GetComponent<Renderer>().material = Utils.BaseCard.Info.Material;
        IsOccupied = false;
        if(!(visual is null)) visual.gameObject.SetActive(false);
    }

    List<Card> GetList () => this.gameObject.tag == "WeatherCard" ? board.Weather :
                             this.gameObject.tag == "BonusCard" ? this.player.Battlefield.Bonus : 
                             this.player.ListByZone[rangeTypes[0]];
}
