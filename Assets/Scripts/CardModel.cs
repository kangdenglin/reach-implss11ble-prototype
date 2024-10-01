using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CardModel : MonoBehaviour
{
    public GameObject ParentPanel; //for new gameobject created during the game
    public GameObject losePanel;
    public GameObject winPanel;

    public Sprite[] cardsStack; //playing cards
    public Image currCard;
    private List<int> cardsOrder = new List<int>(); //order of playing cards
    public int cursorInCardOrder = 0;

    public int score = 0;
    public int pointsForRound = 1;
    public int extraLives = 0; // Number of lives for Jokers
    public TextMeshProUGUI hintText;
    public bool hasHint = false;

    public Button nextbutton;
    // betting buttons
    public Button upbutton; 
    public Button equalbutton;
    public Button downbutton;
    public Button spadeButton;
    public Button clubButton;
    public Button heartButton;
    public Button diamondButton;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI lifeText;

    // Start is called before the first frame update
    void Start()
    {
        //initially set win/lose panels inactive
        ToggleLosePanel(false);
        ToggleWinPanel(false);
        // not showing the current card and betting buttons
        currCard.enabled = false;
        DisableBetButtons();
        hintText.enabled = false; //no hint text inititally


        nextbutton.interactable = false; // make next button unclickable

        //shuffle cards and put it into a list
        CreateCardOrder();
        ShuffleCards();

        while (IsAbilityCard(cardsOrder[0]))
        {
            ShuffleCards(); // Reshuffle if the first card is an ability card
        }

        ShowFirstCard();
        nextbutton.interactable = true; // make next button clickable (player ready to bet)
        Debug.Log("Cards Order: " + string.Join(", ", cardsOrder));
    }

    public void ShowCurrCard()
    {
        int indexInStack = cardsOrder[cursorInCardOrder];
        currCard.enabled = true; //show the back of the card
        if(hasHint)
        {
            hintText.enabled = true;
            hasHint = false;
        } else {
            hintText.enabled = false; //hint is used. close it
        }
        EnableBetButtons();

        //testing
        /*int cardNumber = (indexInStack+1) % 13;
        int cardSuit = indexInStack / 13;
        Debug.Log("This card is: " + cardNumber + " and suit: " +cardSuit + " with index: " + indexInStack);*/
    }

    private int FindPreCardIndexNotAbility(int cardIndex)
    {
        int sign = -1;
        while(sign == -1)
        {
            int cardNumber = (cardsOrder[cardIndex] + 1) % 13;
            if (cardNumber == 11 || cardNumber == 12 || cardNumber == 0 || cardIndex >= 52)
            {
                cardIndex--;
                continue;
            }
            else
            {
                sign = 1;
            }
            
        }

        
        return cardIndex;
        
        //Debug.Log("isability card cardnumber:" + cardNumber);
        //return (cardNumber == 11 || cardNumber == 12 || cardNumber == 0 || cardIndex >= 52); // J, Q, K, or Joker
    }

    public void checkBetResult(string betOperation)
    {
        DisableBetButtons(); //stop betting

        int indexInStack = cardsOrder[cursorInCardOrder]; // cursorInCardOrder is random generated card list ptr, start from 0
        int cardNumber = (indexInStack+1) % 13; // card printed number
        int cardSuit = indexInStack / 13;
        //int preInStack = FindPreCardNotAbility(cardsOrder[cursorInCardOrder - 1]); //find previous card and that card is not ability card
        int preInStack = FindPreCardIndexNotAbility(cursorInCardOrder - 1);
        //int preInStack = cardsOrder[cursorInCardOrder - 1];
        //int prevCardNumber = (preInStack+1) % 13; //prev card to compare bigger/same/smaller
        int prevCardNumber = (cardsOrder[preInStack] + 1) % 13; //card printed number that closest to curr ptr card
        Debug.Log("cursorInCardOrder: " + cursorInCardOrder);
        Debug.Log("preInStack: " + preInStack + " prevCardNumber: " + prevCardNumber + " cardNumber: " + cardNumber);
        // Handle Joker cards (card index 52 and 53 are Jokers)
        
        
        if (IsAbilityCard(indexInStack))
        {

            HandleFaceCard(cardNumber, indexInStack);
            return;
        }

        bool isLose = true;
        if (betOperation.Equals("spade") && (cardSuit == 0))
        {
            isLose = false;
            score += 1* pointsForRound;
            //Debug.Log("Guessing Spade Correct!");
        }
        if (betOperation.Equals("club") && (cardSuit == 1))
        {
            isLose = false;
            score = score + 1 * pointsForRound;
            //Debug.Log("Guessing Club Correct!");
        }
        if (betOperation.Equals("diamond") && (cardSuit == 2))
        {
            isLose = false;
            score = score + 1 * pointsForRound;
            //Debug.Log("Guessing Diamond Correct!");
        }
        if (betOperation.Equals("heart") && (cardSuit == 3))
        {
            isLose = false;
            score = score + 1 * pointsForRound;
            //Debug.Log("Guessing Heart Correct!");
        }
        if (betOperation.Equals("up") && (cardNumber > prevCardNumber))
        {
            isLose = false;
            score = score + 1 * pointsForRound;
            //Debug.Log("Guessing Bigger Correct!");
        }
        if (betOperation.Equals("equal") && (cardNumber == prevCardNumber))
        {
            isLose = false;
            score = score + 3 * pointsForRound;
            //Debug.Log("Guessing same Correct!");
        }
        if (betOperation.Equals("down") && (cardNumber < prevCardNumber))
        {
            isLose = false;
            score = score + 1 * pointsForRound;
            //Debug.Log("Guessing smaller Correct!");
        }


        //scoreText.text = "Score: " + score;
        scoreText.text = "Score: " + score.ToString();


        if (isLose) //player guesses wrongly
        {
           if (extraLives > 0) // Check if player has an extra life from a Joker
            {
                extraLives --;
                lifeText.text = "you have " + extraLives.ToString() + " extra life";
                Debug.Log("Incorrect, but you have an extra life! Lives left: " + extraLives);
            }
            else
            {
                Debug.Log("Lose!!!! card num:" + cardNumber + " prev card num: " + prevCardNumber);
                Debug.Log("Incorrect! Game Over.");

                Score.totalScore = score;
                SceneManager.LoadScene("LoseScene");
                //StartCoroutine(ShowGameOver(indexInStack));

                return;
            } 
        }

        cursorInCardOrder ++;

        if (pointsForRound == 2) {
            pointsForRound = 1; //face card used in this round, hence back to 1
        }

        StartCoroutine(HideCardAfter(1)); // this is to hide the back card image
        StartCoroutine(BecomePrevCard(indexInStack));

        if (score >= 11)
        {
            SceneManager.LoadScene("WinScene");
            //ToggleWinPanel(true); // player win
            return;
        }
        Debug.Log("Current score: " + score);
    }

    // don't use this please, it will not give you the index. We stucked on this for hours.
    private int FindPreCardNotAbility(int index)
    {
        while (IsAbilityCard(index))
        {
            Debug.Log("Previous card is ability card. Find the one before");
            index --;
        }
        return index;
    }

    private void HandleFaceCard(int cardNumber, int indexInStack)
    {
        StartCoroutine(HideCardAfter(1));
        StartCoroutine(MoveCardToAbilityPanel(indexInStack)); // Move face card to ability panel
        int nextInStack = cardsOrder[cursorInCardOrder + 1];
        if (cardNumber == 11) // Jack
        {
            int nextCardSuit = nextInStack / 13;
            ProvideColorHint(nextCardSuit); // Give color hint for next card
            //Debug.Log("Jack drawn! Providing hint for the next card.");
        }
        else if (cardNumber == 12) // Queen
        {
            int nextCardNumber = (nextInStack + 1) % 13;
            //Debug.Log("Queen drawn! Providing odd/even hint for the next card.");
            ProvideOddEvenHint(nextCardNumber); // Hint for next card
        }
        else if (cardNumber == 0) // King
        {
            //Debug.Log("King drawn! Points for this round will be doubled.");
            pointsForRound = 2; // Mark the next round for double points
        }
        else if (indexInStack == 52 || indexInStack == 53) // Joker
        {
            Debug.Log("need to handle joker!!!");
            int nextCardNumber = (nextInStack + 1) % 13;
            HandleJoker();
        }
        //Debug.Log("before ++ called");
        cursorInCardOrder ++; // Move to the next card
        //Debug.Log("++ called");
    }

    IEnumerator MoveCardToAbilityPanel(int preIndex)
    {
        yield return new WaitForSeconds (1);
        // Display the ability card in the ability panel (right side)
        GameObject abilityCard = new GameObject();
        Image abilityImage = abilityCard.AddComponent<Image>();
        abilityImage.sprite = cardsStack[preIndex];
        abilityCard.transform.localScale = new Vector2(0.01938f,0.01938f);
        abilityCard.transform.position = new Vector2(0,-3);
        abilityCard.GetComponent<RectTransform>().SetParent(ParentPanel.transform);
        abilityCard.SetActive(true);

        yield return new WaitForSeconds (1);
        abilityCard.transform.position = new Vector2(5, -3); // Move to the right (ability panel)
    }

    private void HandleJoker()
    {
        extraLives++;
        //cursorInCardOrder++;

        //TODO: check logic and display
        Debug.Log("HandleJoker called");
        lifeText.text = "you have " + extraLives.ToString() + " extra life";

        //StartCoroutine(HideCardAfter(1));
        //StartCoroutine(BecomePrevCard(cardsOrder[cursorInCardOrder]));

    }

    private void ProvideColorHint(int cardSuit)
    {
        int nextInStack = cardsOrder[cursorInCardOrder + 1];
        //Debug.Log("cardSuit: " + cardSuit + "next: " + nextInStack);
        string colorHint = "";
        int wantInStack = -1;
        int hintCardSuit = cardSuit;
        if (nextInStack == 11 ||
            nextInStack == 24 ||
            nextInStack == 37 ||
            nextInStack == 50)
        {
            wantInStack = cardsOrder[cursorInCardOrder + 2];
            hintCardSuit = wantInStack / 13;

            //Debug.Log("cardSuit: " + cardSuit + "next: " + nextInStack);
        }
        
        if (hintCardSuit == 0 || hintCardSuit == 1) // Spades and Clubs
        {
            colorHint = "Black";
        }
        else if (hintCardSuit == 2 || hintCardSuit == 3) // Hearts and Diamonds
        {
            colorHint = "Red";
        }
        hasHint = true;
        hintText.text = "Hint: The next number card suit color is " + colorHint;
    }

    private void ProvideOddEvenHint(int cardNumber)
    {
        hasHint = true;
        int wantInStack = -1;
        int nextInStack = cardsOrder[cursorInCardOrder + 1];
        int hintCardNumber = cardNumber;
        if (nextInStack == 10 ||
            nextInStack == 23 ||
            nextInStack == 36 ||
            nextInStack == 49)
        {
            wantInStack = cardsOrder[cursorInCardOrder + 2];
            hintCardNumber = wantInStack % 13;
            
        }
        if (hintCardNumber % 2 == 0) // Even number
        {
            hintText.text = "Hint: The next number card is even.";
        }
        else // Odd number
        {
            hintText.text = "Hint: The next number card is odd.";
        }
    }

    IEnumerator ShowGameOver(int finalCardIndex)
    {
        // Show the final card before displaying "Game Over"
        currCard.enabled = true;
        currCard.sprite = cardsStack[finalCardIndex];

        yield return new WaitForSeconds(2);  // Display the final card for 2 seconds

        ToggleLosePanel(true);  // Show the Game Over panel after the final card
    }

    // show current card that user need to bet
    IEnumerator BecomePrevCard(int preIndex)
    {
        // to make it more like flipping
        yield return new WaitForSeconds (1);
        GameObject prevCard = new GameObject();
        Image NewImage = prevCard.AddComponent<Image>();
        NewImage.sprite = cardsStack[preIndex];
        prevCard.transform.localScale = new Vector2(0.01938f,0.01938f);
        prevCard.transform.position = new Vector2(0,-3);
        //Assign the newly created Image GameObject as a Child of the Parent Panel.
        prevCard.GetComponent<RectTransform>().SetParent(ParentPanel.transform);
        prevCard.SetActive(true);

        //wait for 2 secs to stay
        yield return new WaitForSeconds (1);
        //move to the left
        prevCard.transform.position = new Vector2(-6,-3);
    }

    IEnumerator HideCardAfter(int sec) {
        yield return new WaitForSeconds (sec);
        currCard.enabled = false;
    }

    public void ToggleLosePanel(bool isShow) {
        losePanel.SetActive(isShow);
    }

    public void ToggleWinPanel(bool isShow) {
        winPanel.SetActive(isShow);
    }

    private void DisableBetButtons()
    {
        upbutton.gameObject.SetActive(false);
        downbutton.gameObject.SetActive(false);
        equalbutton.gameObject.SetActive(false);
        spadeButton.gameObject.SetActive(false);
        clubButton.gameObject.SetActive(false);
        heartButton.gameObject.SetActive(false);
        diamondButton.gameObject.SetActive(false);
    }

    private void EnableBetButtons()
    {
        upbutton.gameObject.SetActive(true);
        downbutton.gameObject.SetActive(true);
        equalbutton.gameObject.SetActive(true);
        spadeButton.gameObject.SetActive(true);
        clubButton.gameObject.SetActive(true);
        heartButton.gameObject.SetActive(true);
        diamondButton.gameObject.SetActive(true);
    }

    private bool IsAbilityCard(int cardIndex)
    {
        int cardNumber = (cardIndex+1) % 13;
        //cardNumber = (cardsOrder[cardIndex] + 1) % 13;
        Debug.Log("isability card cardnumber:" + cardNumber);
        return (cardNumber == 11 || cardNumber == 12 || cardNumber == 0 || cardIndex >= 52); // J, Q, K, or Joker
    }

    

    // Function to show the first card at the start of the game
    private void ShowFirstCard()
    {
        int indexInStack = cardsOrder[cursorInCardOrder];
        currCard.enabled = true;
        StartCoroutine(HideCardAfter(1));
        StartCoroutine(BecomePrevCard(indexInStack));
        cursorInCardOrder++;
    }

    //create a list of numbers from 0 to 54 (13*4 + 2 Joker)
    private void CreateCardOrder()
    {
        for(int i=0; i< cardsStack.Length; i++)
        {
            cardsOrder.Add(i);
        }
    }

    private void ShuffleCards()
    {
        int numOfCardsleft = cardsOrder.Count;
        while (numOfCardsleft > 1)
        {
            numOfCardsleft --;
            int k = Random.Range(0,numOfCardsleft);
            int value = cardsOrder[k];
            cardsOrder[k] = cardsOrder[numOfCardsleft];
            cardsOrder[numOfCardsleft] = value;
        }


        // demo use only
        //cardsOrder[0] = 1;
        //cardsOrder[1] = 2; //上
        //cardsOrder[2] = 9; // 上
        //cardsOrder[3] = 52; // Joker
        //cardsOrder[4] = 5; // 左1
        //cardsOrder[5] = 45; // 7 heart
        //cardsOrder[6] = 4;// 下
        //cardsOrder[7] = 28; // 3 红心 下
        //cardsOrder[8] = 3; // 4 上
        //cardsOrder[9] = 11; // Q
        //cardsOrder[10] = 6; //上
        //cardsOrder[11] = 12; // K
        //cardsOrder[12] = 1;//下
        //cardsOrder[13] = 11;// J
        //cardsOrder[14] = 9;//上
    }
}
