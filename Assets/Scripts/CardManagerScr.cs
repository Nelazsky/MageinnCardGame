using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Card
{
    public string Name;
    public Sprite Logo;
    public int Attack, Defense, Manacost;
    public bool CanAttack;
    public bool IsPlaced;

    public bool IsAlive
    {
        get
        {
            return Defense > 0;
        }
    }

    public Card(string name, string logoPath, int attack, int defense, int manacost)
    {
        Name = name;
        Logo = Resources.Load<Sprite>(logoPath);
        Attack = attack;
        Defense = defense;
        Manacost = manacost;
        CanAttack = false;
        IsPlaced = false;
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
        CardManager.AllCards.Add(new Card("Airat", "Sprites/Cards/1", 3, 7, 5));
        CardManager.AllCards.Add(new Card("Misha", "Sprites/Cards/2", 15, 15, 10));
        CardManager.AllCards.Add(new Card("Rustem", "Sprites/Cards/3", 7, 7, 5));
        CardManager.AllCards.Add(new Card("Ruslan", "Sprites/Cards/4", 5, 5, 4));
        CardManager.AllCards.Add(new Card("Vova", "Sprites/Cards/5", 2, 6, 3));
        CardManager.AllCards.Add(new Card("Petya", "Sprites/Cards/6", 1, 8, 5));
        CardManager.AllCards.Add(new Card("Rifkat", "Sprites/Cards/7", 7, 3, 4));
    }
}
