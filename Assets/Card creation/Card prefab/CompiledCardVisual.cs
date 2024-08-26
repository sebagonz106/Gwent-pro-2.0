using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompiledCardVisual : MonoBehaviour
{
    [SerializeField] GameObject body;
    [SerializeField] SpritesWarehouse sprWh;
    [SerializeField] Material rebelMaterial;
    [SerializeField] Material batistaMaterial;
    [SerializeField] Image mainImage;
    [SerializeField] Image faction;
    [SerializeField] Image type;
    [SerializeField] TMP_Text cardName;
    [SerializeField] TMP_Text damage;
    [SerializeField] TMP_Text description;

    void UpdateInfo(Card card)
    {
        cardName.text = card.Name;
        damage.text = card.InitialDamage.ToString();
        description.text = card.Description;
        mainImage.sprite = card.Info.Main;

        if(card.FactionEnum is Faction.Fidel)
        {
            faction.sprite = sprWh.Rebels;
            body.GetComponent<Renderer>().material = rebelMaterial;
        }
        else
        {
            faction.sprite = sprWh.Batista;
            body.GetComponent<Renderer>().material = batistaMaterial;
        }

        switch (card.CardType)
        {
            case CardType.Unit:
                type.sprite = ((UnitCard)card).Level is Level.Golden? sprWh.Golden : sprWh.Silver;
                break;
            case CardType.Bonus:
                type.sprite = sprWh.Bonus;
                break;
            case CardType.Leader:
                type.sprite = sprWh.Leader;
                break;
            case CardType.Weather:
                type.sprite = sprWh.Weather;
                break;
            case CardType.Clear:
                type.sprite = sprWh.Clear;
                break;
            case CardType.Bait:
                type.sprite = sprWh.Bait;
                break;
            default:
                type.sprite = sprWh.Silver;
                break;
        }
    }
}
