using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompiledCardVisual : MonoBehaviour
{
    [SerializeField] bool isInfo;
    [SerializeField] Image mainImage;
    [SerializeField] Image faction;
    [SerializeField] Image type;
    [SerializeField] TMP_Text cardName;
    [SerializeField] TMP_Text damage;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text range;

    public void UpdateInfo(Card card)
    {
        bool hasRange = true;
        cardName.text = card.Name;
        damage.text = card.InitialDamage.ToString();
        description.text = card.Description;
        mainImage.sprite = isInfo ? card.Info.Information : card.Info.Main;

        if (card.FactionEnum is Faction.Fidel) faction.sprite = SpritesWarehouse.Instance.Rebels;
        else faction.sprite = SpritesWarehouse.Instance.Batista;

        switch (card.CardType)
        {
            case CardType.Unit:
                type.sprite = ((UnitCard)card).Level is Level.Golden ? SpritesWarehouse.Instance.Golden : SpritesWarehouse.Instance.Silver;
                break;
            case CardType.Bonus:
                type.sprite = SpritesWarehouse.Instance.Bonus;
                break;
            case CardType.Leader:
                type.sprite = SpritesWarehouse.Instance.Leader;
                hasRange = false;
                break;
            case CardType.Weather:
                type.sprite = SpritesWarehouse.Instance.Weather;
                break;
            case CardType.Clear:
                type.sprite = SpritesWarehouse.Instance.Clear;
                break;
            case CardType.Bait:
                type.sprite = SpritesWarehouse.Instance.Bait;
                break;
            default:
                type.sprite = SpritesWarehouse.Instance.Silver;
                break;
        }

        range.text = "";
        if (hasRange)
        {
            foreach (var item in card.AvailableRange)
            {
                switch (item)
                {
                    case Zone.Melee:
                        range.text += isInfo ? "Melee-" : "M-";
                        break;
                    case Zone.Range:
                        range.text += isInfo ? "Range-" : "R-";
                        break;
                    case Zone.Siege:
                        range.text += isInfo ? "Siege-" : "S-";
                        break;
                    default:
                        break;
                }
            }
            range.text = range.text.Substring(0, range.text.Length - 1);
        }
    }
}
