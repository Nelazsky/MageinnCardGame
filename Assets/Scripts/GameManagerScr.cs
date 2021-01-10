using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class Game
{
    public List<Card> EnemyDeck, PlayerDeck;

    public Game()
    {
        EnemyDeck = GiveDeckCard();
        PlayerDeck = GiveDeckCard();
    }

    List<Card> GiveDeckCard()
    {
        List<Card> list = new List<Card>();
        for (int i = 0; i < 10; i++)
            list.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
        return list;
    }
}

public class GameManagerScr : MonoBehaviour
{
    public Game CurrentGame;
    public Transform EnemyHand, PlayerHand, EnemyField, PlayerField;
    public GameObject CardPref;
    private int Turn, TurnTime = 30;
    public TextMeshProUGUI TurmTimeTxt;
    public Button EndTurnBtn;

    public int PlayerMana = 10; 
    public int EnemyMana = 10;
    public TextMeshProUGUI PlayerManaText, EnemyManaText;

    public int PlayerHP, EnemyHP; 
    public TextMeshProUGUI PlayerHPText, EnemyHPText;

    public GameObject ResultGO;
    public TextMeshProUGUI ResultText;
    
    public List<CardInfoScr>    PlayerHandCards = new List<CardInfoScr>(),
                                PlayerFieldCards = new List<CardInfoScr>(),
                                EnemyHandCards = new List<CardInfoScr>(),
                                EnemyFieldCards = new List<CardInfoScr>();

    public bool IsPlayerTurn
    {
        get
        {
            return Turn % 2 == 0;
        }
    }

    void Start()
    {
        Turn = 0;
        
        CurrentGame = new Game();
        
        GiveHandCards(CurrentGame.EnemyDeck, EnemyHand);
        GiveHandCards(CurrentGame.PlayerDeck, PlayerHand);

        PlayerHP = EnemyHP = 30;
        
        ShowMana();
        
        StartCoroutine(TurnFunc());
    }

    void GiveHandCards(List<Card> deck, Transform hand)
    {
        int i = 0;
        while(i++ < 4)
            GiveCardToHand(deck, hand);
    }

    void GiveCardToHand(List<Card> deck, Transform hand)
    {
        if (deck.Count == 0)
            return;

        Card card = deck[0];

        GameObject cardGO = Instantiate(CardPref, hand, false);

        if (hand == EnemyHand)
        {
            cardGO.GetComponent<CardInfoScr>().HideCardInfo(card);
            EnemyHandCards.Add(cardGO.GetComponent<CardInfoScr>());
        }
        else{
            cardGO.GetComponent<CardInfoScr>().ShowCardInfo(card, true);
            PlayerHandCards.Add(cardGO.GetComponent<CardInfoScr>());
            cardGO.GetComponent<AttackedCardScr>().enabled = false;

        }
        deck.RemoveAt(0);
    }

    IEnumerator TurnFunc()
    {
        TurnTime = 30;

        TurmTimeTxt.text = TurnTime.ToString();
        
        foreach (var card in PlayerFieldCards )
        {
            card.DeHighlightCard();
        }

        if (IsPlayerTurn)
        {
            foreach (var card in PlayerFieldCards )
            {
                card.SelfCard.ChangeAttackState(true);
                card.HighlightCard();
            }
            
            while (TurnTime-- > 0)
            {
                TurmTimeTxt.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            foreach (var card in EnemyFieldCards)
            {
                card.SelfCard.ChangeAttackState(true);
            }

            while (TurnTime-- > 27)
            {
                TurmTimeTxt.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
                
            }

            if (EnemyHandCards.Count > 0)
                EnemyTurn(EnemyHandCards);
        }
        ChangeTurn();
    }

    void EnemyTurn(List<CardInfoScr> cards)
    {
        int count = cards.Count == 1 ? 1 : 
                    Random.Range(0, cards.Count);

        for (int i = 0; i < count; i++)
        {
            if(EnemyFieldCards.Count > 5 || EnemyMana == 0)
                break;

            List<CardInfoScr> cardList = cards.FindAll(x => EnemyMana >= x.SelfCard.Manacost);
            
            if(cardList.Count == 0)
                break;
            
            ReduceMana(false, cardList[0].SelfCard.Manacost);

            cardList[0].ShowCardInfo(cardList[0].SelfCard, false);
            cardList[0].transform.SetParent(EnemyField);

            EnemyFieldCards.Add(cardList[0]);
            EnemyFieldCards.Remove(cardList[0]);
        }

        foreach (var activeCard in EnemyFieldCards.FindAll(x => x.SelfCard.CanAttack))
        {
            if (Random.Range(0, 2) == 0 && PlayerFieldCards.Count > 0)
            {
                var enemy = PlayerFieldCards[Random.Range(0, PlayerFieldCards.Count)];

                Debug.Log(activeCard.SelfCard.Name + " (" + activeCard.SelfCard.Attack + ";" + activeCard.SelfCard.Defense + ") " +
                          "---->" + enemy.SelfCard.Name + " (" + enemy.SelfCard.Attack + ";" + enemy.SelfCard.Defense + ")");
                
                activeCard.SelfCard.ChangeAttackState(false);
                CardsFight(enemy, activeCard);
            }
            else
            {
                Debug.Log(activeCard.SelfCard.Name + " (" + activeCard.SelfCard.Attack + ") Attacked Hero" );
                activeCard.SelfCard.ChangeAttackState(false);
                DamageHero(activeCard, false);
            }
        }
    }

    void ChangeTurn()
    {
        StopAllCoroutines();
        Turn++;
        
        EndTurnBtn.interactable = IsPlayerTurn;

        if (IsPlayerTurn)
        {
            GiveNewCards();

            PlayerMana = 10;
            EnemyMana = 10;

            ShowMana();
        }

        StartCoroutine(TurnFunc());
    }

    void GiveNewCards()
    {
        GiveCardToHand(CurrentGame.EnemyDeck, EnemyHand);
        GiveCardToHand(CurrentGame.PlayerDeck, PlayerHand);
    }

    public void CardsFight(CardInfoScr playerCard, CardInfoScr enemyCard)
    {
        playerCard.SelfCard.GetDamage(enemyCard.SelfCard.Attack);
        enemyCard.SelfCard.GetDamage(playerCard.SelfCard.Attack);

        if (!playerCard.SelfCard.IsAlive)
            DestroyCard(playerCard);
        else
            playerCard.RefreshData();
        
        if (!enemyCard.SelfCard.IsAlive)
            DestroyCard(enemyCard);
        else
            enemyCard.RefreshData();
    }

    void DestroyCard(CardInfoScr card)
    {
        card.GetComponent<CardMovementScr>().OnEndDrag(null);
            
        if (EnemyFieldCards.Exists(x => x == card))
            EnemyFieldCards.Remove(card);
        
        if (PlayerFieldCards.Exists(x => x == card))
            PlayerFieldCards.Remove(card);
        
        Destroy(card.gameObject);
    }

    void ShowMana()
    {
        PlayerManaText.text = PlayerMana.ToString();
        EnemyManaText.text = EnemyMana.ToString();
    }

    void ShowHP()
    {
        PlayerHPText.text = PlayerHP.ToString();
        EnemyHPText.text = EnemyHP.ToString();
    }

    public void ReduceMana(bool playerMana, int manacost)
    {
        if (playerMana)
        {
            PlayerMana = Mathf.Clamp(PlayerMana - manacost, 0, int.MaxValue);  
        }
        else
        {
            EnemyMana = Mathf.Clamp(EnemyMana - manacost, 0, int.MaxValue);  
        }
        ShowMana();
    }

    public void DamageHero(CardInfoScr card, bool isEnemyAttacked )
    {
        if (isEnemyAttacked)
        {
            EnemyHP = Mathf.Clamp(EnemyHP - card.SelfCard.Attack, 0, int.MaxValue);
        }
        else
        {
            PlayerHP = Mathf.Clamp(PlayerHP - card.SelfCard.Attack, 0, int.MaxValue);
        }

        ShowHP();
        card.DeHighlightCard();    
        CheckForResult();
    }

    void CheckForResult()
    {
        if (EnemyHP == 0 || PlayerHP == 0)
        {
            ResultGO.SetActive(true);
            StopAllCoroutines();
            
            if (EnemyHP == 0)
            {
                ResultText.text = "YOU ARE WIN!";
            }
            else
            {
                ResultText.text = "YOU ARE LOSE...";
            }
        }
    }
}
