using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesWarehouse
{
    public Sprite Rebels;
    public Sprite Batista;
    public Sprite Golden;
    public Sprite Silver;
    public Sprite Bait;
    public Sprite Bonus;
    public Sprite Weather;
    public Sprite Clear;
    public Sprite Leader;

    static SpritesWarehouse instance;

    public static SpritesWarehouse Instance
    {
        get
        {
            if (instance is null) instance = new SpritesWarehouse();
            return instance;
        }
    }

    SpritesWarehouse()
    {
        Rebels = Resources.Load<Sprite>("Factions/M267");
        Batista = Resources.Load<Sprite>("Factions/Batista army");
        Golden = Resources.Load<Sprite>("Type/Oro");
        Silver = Resources.Load<Sprite>("Type/Plata");
        Bait = Resources.Load<Sprite>("Type/Señuelo");
        Bonus = Resources.Load<Sprite>("Type/Aumento");
        Weather = Resources.Load<Sprite>("Type/Clima");
        Clear = Resources.Load<Sprite>("Type/Despeje");
        Leader = Resources.Load<Sprite>("Type/Lider");
    }
}
