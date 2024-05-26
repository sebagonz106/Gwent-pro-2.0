using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Context
{
    public MasterController MasterController { get; private set; }
    public Board Board { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public Player EnemyPlayer { get; private set; }
    public List<Card> CurrentPosition { get; private set; }
    public Card CurrentCard { get; private set; }

    #region Singleton
    //private Context instance;
    //public Context Instance => instance.Equals(null) ? SetInstance() : instance;
    //private Context()
    //{
    //    this.Board = Board.board;
    //    this.CurrentPlayer = this.Board.Batista;
    //    this.EnemyPlayer = this.Board.Fidel;
    //    this.MasterController = this.Board.masterController;
    //    this.CurrentPosition = this.Board.Weather;
    //    this.CurrentCard = Utils.BaseCard;
    //}
    //private Context SetInstance()
    //{
    //    instance = new Context();
    //    return instance;
    //}
    #endregion

    public Context(Player enemy)
    {
        this.Board = Board.board;
        this.CurrentPlayer = this.Board.Batista;
        this.EnemyPlayer = this.Board.Fidel;
        this.MasterController = this.Board.masterController;
        this.CurrentPosition = this.Board.Weather;
        this.CurrentCard = Utils.BaseCard;
    }

    public Context UpdateInstance(List<Card> position, Card target)
    {
        this.CurrentCard = target;
        this.CurrentPosition = position;
        return this;
    }
}
