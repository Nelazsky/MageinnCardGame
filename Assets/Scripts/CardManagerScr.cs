using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Card
{
    public string Name;
    public Sprite Logo;
    public int Attack, Defense;
    public bool CanAttack;

    public bool IsAlive
    {
        get
        {
            return Defense > 0;
        }
    }

    public Card(string name, string logoPath, int attack, int defense)
    {
        Name = name;
        Logo = Resources.Load<Sprite>(logoPath);
        Attack = attack;
        Defense = defense;
        CanAttack = false;
    }

    public void ChangeAttackState(bool can)
    {
        CanAttack = can;
    }

    public void GetDamage(int damage)
    {
        Defense -= damage;
    }
}

public static class CardManager
{
    public static List<Card> AllCards = new List<Card>();
}
public class CardManagerScr : MonoBehaviour
{
    public void Awake()
    {
        CardManager.AllCards.Add(new Card("Airat", "Sprites/Cards/1", 5, 5));
        CardManager.AllCards.Add(new Card("Misha", "Sprites/Cards/2", 15, 15));
        CardManager.AllCards.Add(new Card("Rustem", "Sprites/Cards/3", 5, 5));
        CardManager.AllCards.Add(new Card("Ruslan", "Sprites/Cards/4", 5, 5));
        CardManager.AllCards.Add(new Card("Vova", "Sprites/Cards/5", 1, 1));
        CardManager.AllCards.Add(new Card("Dima", "Sprites/Cards/6", 10, 10));
        CardManager.AllCards.Add(new Card("Rifkat", "Sprites/Cards/7", 5, 5));
    }
}
