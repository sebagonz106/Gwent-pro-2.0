using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Unit Card", menuName ="Scriptable Objects/Unit Card")]
public class UnitCard : Card
{
    [SerializeField] double initialDamage = 0;
    double damageOnField;
    double damage;
    public Level level;
    public Effect effect = Effects.VoidEffect;

    public double InitialDamage { get => initialDamage; }
    public double DamageOnField { get => damageOnField; }
    public double Damage { get => damage; set => damage = value; }

    public UnitCard(string name, Faction faction, CardType cardType, List<RangeType> availableRange, Material material, Sprite information, List<Card> currentPosition) : base(name, faction, cardType, availableRange, material, information, currentPosition)
    {
    }

    public UnitCard(string name, Material material) : base(name, material)
    {
    }

    public void AssignEffect(Effect effect)
    {
        this.effect = effect;
    }

    public bool Effect(params object[] parameters)
    {
        try
        {
            return effect.Invoke(parameters);
        }
        catch (System.NullReferenceException)
        {
            return Effects.VoidEffect(parameters);
        }
    }

    public void ResetDamage() //when a weathercard affects the damage of this card, the value will only be changed until it
                              //has been summed to the total damage of the player at the moment it's being calculated, 
                              //then it will return to the initial value
    {
        this.Damage = this.damageOnField;
    }

    public void InitializeDamage()
    {
        this.damageOnField = this.initialDamage;
        ResetDamage();
    }

    public void ModifyOnFieldDamage(double newDamage, bool modifyCurrentDamageAsWell = true)
    {
        // damageOnField - damage = modification suffered on board 
        if (modifyCurrentDamageAsWell) this.Damage = (newDamage - (this.damageOnField - this.Damage) > 0 ? newDamage - (this.damageOnField - this.Damage) : 0);

        this.damageOnField = newDamage;
    }
}
