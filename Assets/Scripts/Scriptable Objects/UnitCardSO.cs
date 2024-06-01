using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Unit Card", menuName ="Scriptable Objects/Unit Card")]
public class UnitCardSO : CardSO
{
    [SerializeField] double initialDamage = 0;
    public Level level;

    public double InitialDamage { get => initialDamage; }
}
