using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CompiledCardVisual : MonoBehaviour
{
    [SerializeField] GameObject card;
    [SerializeField] Material rebelMaterial;
    [SerializeField] Material batistaMaterial;
    [SerializeField] Sprite mainImage;
    [SerializeField] Sprite faction;
    [SerializeField] Sprite type;
    [SerializeField] TMP_Text cardName;
    [SerializeField] TMP_Text damage;
    [SerializeField] TMP_Text description;
}
