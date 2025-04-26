using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Web3Unity.Scripts.Library.ETHEREUEM.EIP;

public class CardGameManager : MonoBehaviour, IOnEventCallback
{
    public List<List<string>> PlayerDeckNFTs = new List<List<string>>();

    public NFTCard[] DeckCards;

    [Header("UI")]
    public GameObject TablePanel;
    public GameObject DeckPanel;
    public GameObject LoadingScreen;
    public GameObject WaitingPanel;
    public GameObject ChooseCardPanel;
    public GameObject ChooseCardNonTurnPanel;

    [Header("Table Panel")]
    public Button OtherPlayerDeckBtn;
    public PlayerInfo[] PlayerInfoObjects;
    public Text SelectedAttributeDisplay;
    public List<NFTCard> TableNFTCards;

    [SerializeField]
    Attributes SelectedAttribute;

    [Header("Select Card Panel")]
    [SerializeField]
    NFTCard SelectedCard;

    [SerializeField]
    Text CardSelectionInfoText;


    [Header("Select Card Non Turn Panel")]
    [SerializeField]
    NFTCard SelectedNonTurnCard;
    public Text AttributeInfoText;


    List<string> MyDeckTokenIds = new List<string>();
    int SelectedCardIndex = 0;

    bool GameStarted;


    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void Start()
    {
        MyDeckTokenIds.Clear();
        MyDeckTokenIds = GameDataMP.myDeck.Keys.ToList();

        foreach (NFTCard card in TableNFTCards)
            card.gameObject.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            EnableWaitingPanel();
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers) && !GameStarted)
        {
            Debug.Log("START GAME");
            GameStarted = true;
            StartGameForAllEvent();
        }
    }

    public const byte StartGameForAllEventCode = 1;

    public void StartGameForAllEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(StartGameForAllEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    public const byte SendSelectedCardAndAttributeEventCode = 2;

    public void SendSelectedCardAndAttributeEvent()
    {
        object content = new object[] { SelectedAttribute.ToString(), MyDeckTokenIds[SelectedCardIndex] };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SendSelectedCardAndAttributeEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public const byte ShowDownEventCode = 3;

    public void ShowDownEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(ShowDownEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case StartGameForAllEventCode:
                WaitingPanel.SetActive(false);

                foreach (var p in PhotonNetwork.PlayerList)
                    PlayerInfoObjects[p.ActorNumber - 1].SetPlayerName(p.NickName);

                //For now first turn is of master client
                if (PhotonNetwork.IsMasterClient)
                {
                    //Select Attribute and Card
                    OnActivateSelectCardPanel();
                }
                break;

            case SendSelectedCardAndAttributeEventCode:

                if (ChooseCardPanel.activeSelf)
                    ChooseCardPanel.SetActive(false);

                if (ChooseCardNonTurnPanel.activeSelf)
                    ChooseCardNonTurnPanel.SetActive(false);

                object[] data = (object[])photonEvent.CustomData;

                string attributeselected = data[0].ToString();
                Enum.TryParse(attributeselected, out SelectedAttribute);

                SelectedAttributeDisplay.text = "Attribute: " + SelectedAttribute.ToString();

                string NFTSelectedTokenID = data[1].ToString();
                Debug.Log(NFTSelectedTokenID);

                foreach(var tb in TableNFTCards)
                {
                    if (!tb.gameObject.activeSelf)
                    {
                        tb.GetAndDisplayNFTMetaData(NFTSelectedTokenID);
                        tb.gameObject.SetActive(true);
                        break;
                    }
                }

                if (PhotonNetwork.CurrentRoom.CustomProperties[GameDataMP.TURN_PROP] != null)
                {
                    bool isRST = (int)PhotonNetwork.CurrentRoom.CustomProperties[GameDataMP.RST_PROP] == PhotonNetwork.LocalPlayer.ActorNumber;

                    if ((int)PhotonNetwork.CurrentRoom.CustomProperties[GameDataMP.TURN_PROP] == PhotonNetwork.LocalPlayer.ActorNumber)
                        PassTurnAndSetRoomProperties(isRST);
                }
                else
                {
                    if (PhotonNetwork.IsMasterClient)
                        PassTurnAndSetRoomProperties(true); //Master Client first turn
                }

                break;

            case ShowDownEventCode:
                //Display Cards for now
                foreach (var tb in TableNFTCards)
                {
                    Debug.Log(tb.name + "   " + tb.gameObject.activeSelf);    
                    if (tb.gameObject.activeSelf)
                    {
                        tb.GetComponent<TableCards>().ShowCardBack(false);
                    }
                }

                CompareCardsAndFindWinner();

                break;
        }
    }

    public void ViewMyDeck()
    {
        ActivateLoadingScreen(true);
        DeckPanel.SetActive(true);

        int crdind = 0;
        foreach (var mycard in GameDataMP.myDeck)
        {
            DeckCards[crdind].SetNFTCardData(mycard.Value.MPCardMetaData, mycard.Value.CardImage);
            DeckCards[crdind].gameObject.SetActive(true);
            crdind++;
        }
        ActivateLoadingScreen(false);
    }

    public void ViewOpponentsDeck()
    {
        ActivateLoadingScreen(true);
        DeckPanel.SetActive(true);

        int crdind = 0;
        foreach (var crd in PlayerDeckNFTs[PhotonNetwork.PlayerListOthers[0].ActorNumber - 1])
        {
            Debug.Log(crd);
            DeckCards[crdind].GetAndDisplayNFTMetaData(crd);
            DeckCards[crdind].gameObject.SetActive(true);
            crdind++;
        }

        ActivateLoadingScreen(false);
    }

    public void SetSelectedAttribute(Attributes selattrib)
    {
        SelectedAttribute = selattrib;
    }

    public void SelectCard()
    {
        if (SelectedAttribute != Attributes.None)
        {
            SendSelectedCardAndAttributeEvent();
        }
        else
            DisplayCardSelectionInfoMessage("Please select an attribute to continue!");
    }


    public void PassTurnAndSetRoomProperties(bool isRoundStartTurn)
    {
        Debug.Log("Next player: " + PhotonNetwork.LocalPlayer.GetNext().ActorNumber);

        if(PhotonNetwork.CurrentRoom.CustomProperties[GameDataMP.RST_PROP] != null && (PhotonNetwork.LocalPlayer.GetNext().ActorNumber == (int)PhotonNetwork.CurrentRoom.CustomProperties[GameDataMP.RST_PROP]))
        {
            //Showdown
            Debug.Log("ShowDown!!");
            ActivateLoadingScreen(true);
            StartCoroutine(ShowDownAfterDelay(5));
            return;
        }

        ExitGames.Client.Photon.Hashtable setTurnValue = new ExitGames.Client.Photon.Hashtable();
        setTurnValue.Add(GameDataMP.TURN_PROP, PhotonNetwork.LocalPlayer.GetNext().ActorNumber);
        
        if (isRoundStartTurn)
            setTurnValue.Add(GameDataMP.RST_PROP, PhotonNetwork.LocalPlayer.ActorNumber);

        PhotonNetwork.CurrentRoom.SetCustomProperties(setTurnValue);

        Debug.Log("Turn passed to: " + PhotonNetwork.LocalPlayer.GetNext().ActorNumber);
    }

    IEnumerator ShowDownAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ActivateLoadingScreen(false);
        ShowDownEvent();
    }
    void CompareCardsAndFindWinner()
    {
        List<float> cardscores = new List<float>();
        Dictionary<NFTCard,float> cardstosort = new Dictionary<NFTCard,float>();
        foreach(NFTCard tbc in TableNFTCards)
        {
            if (tbc.gameObject.activeSelf)
            {
                Debug.Log("Type: " + SelectedAttribute.ToString());
                Debug.Log("Value: " + tbc.NFTAttributesData[((int)SelectedAttribute - 1)].GetValueAgainstType());

                var score = JSONReader.Instance.GetCorrespondingScore(SelectedAttribute.ToString(), tbc.NFTAttributesData[((int)SelectedAttribute - 1)].GetValueAgainstType());

                cardscores.Add(score);
                cardstosort.Add(tbc, score);
                Debug.Log(score);
            }
        }

        var mySortedList = cardstosort.OrderByDescending(d => d.Value).ToList();
        Debug.Log("HIGHEST: " + mySortedList[0].Key.GetCurrentCardData().MPCardMetaData.name + "  " + mySortedList[0].Value);
        mySortedList[0].Key.gameObject.GetComponent<TableCards>().WinnerText.SetActive(true);

    }


    #region UI

    void DeSelectAll()
    {
        TablePanel.SetActive(false);
        DeckPanel.SetActive(false);
        LoadingScreen.SetActive(false);
        WaitingPanel.SetActive(false);
        ChooseCardPanel.SetActive(false);
        ChooseCardNonTurnPanel.SetActive(false);
    }

    public void EnableWaitingPanel()
    {
        DeSelectAll();
        TablePanel.SetActive(true);
        WaitingPanel.SetActive(true);
        WaitingPanel.GetComponentInChildren<Text>().text = "Waiting for other players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public void ActivateLoadingScreen(bool activate)
    {
        LoadingScreen.SetActive(activate);
    }

    public void OnActivateSelectCardPanel()
    {
        DeSelectAll();
        TablePanel.SetActive(true);
        ChooseCardPanel.SetActive(true);
        SelectedCard.SetNFTCardData(GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].MPCardMetaData, GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].CardImage);
    }

    public void OnClickRightSelectCard(bool turn)
    {
        SelectedCardIndex++;

        if (SelectedCardIndex >= MyDeckTokenIds.Count)
            SelectedCardIndex = 0;

        if (turn)
            SelectedCard.SetNFTCardData(GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].MPCardMetaData, GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].CardImage);
        else
            SelectedNonTurnCard.SetNFTCardData(GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].MPCardMetaData, GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].CardImage);

    }

    public void OnClickLeftSelectCard(bool turn)
    {
        SelectedCardIndex--;

        if (SelectedCardIndex <= 0)
            SelectedCardIndex = MyDeckTokenIds.Count - 1;

        if (turn)
            SelectedCard.SetNFTCardData(GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].MPCardMetaData, GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].CardImage);
        else
            SelectedNonTurnCard.SetNFTCardData(GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].MPCardMetaData, GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].CardImage);

    }

    public void DisplayCardSelectionInfoMessage(string msg)
    {
        CardSelectionInfoText.text = msg;
    }

    public void OnClickExitDeckView()
    {
        DeckPanel.SetActive(false);
        foreach (var dc in DeckCards)
        {
            dc.ClearCard();
        }
    }


    public void OnActivateSelectCardNonTurnPanel()
    {
        SelectedCardIndex = 0;
        DeSelectAll();
        TablePanel.SetActive(true);
        ChooseCardNonTurnPanel.SetActive(true);
        AttributeInfoText.text = SelectedAttribute.ToString();
        SelectedNonTurnCard.SetNFTCardData(GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].MPCardMetaData, GameDataMP.myDeck[MyDeckTokenIds[SelectedCardIndex]].CardImage);
    }
    #endregion


    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
