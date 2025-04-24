using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Web3Unity.Scripts.Library.ETHEREUEM.EIP;
using Web3Unity.Scripts.Library.Ethers.Contracts;
using Web3Unity.Scripts.Library.Ethers.Providers;

public class NFTCharacterManager : MonoBehaviour
{
    public iconCharacter iconCharacter;
    public Character hero;

    string ContractAbi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"ENABLE_REVEAL\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"Item_Count\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"Items\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"supply\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"order_cnt\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"order_id\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"base_uri\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"PUBLIC_PRICE\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"airdropAddress\",\"type\":\"address[]\"},{\"internalType\":\"uint256\",\"name\":\"numberOfTokens\",\"type\":\"uint256\"}],\"name\":\"airdrop\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"currentID\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getTokenId\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"numberOfTokens\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string[]\",\"name\":\"baseURI\",\"type\":\"string[]\"},{\"internalType\":\"uint256[]\",\"name\":\"supply\",\"type\":\"uint256[]\"},{\"internalType\":\"bool\",\"name\":\"isEnable\",\"type\":\"bool\"}],\"name\":\"setBaseInfo\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"publicPrice\",\"type\":\"uint256\"}],\"name\":\"setMintPrice\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"paymentAddress\",\"type\":\"address\"}],\"name\":\"setPaymentAddress\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"unrevealURI\",\"type\":\"string\"}],\"name\":\"setUnrevealURI\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
    string contract = "XXXXXXXXXXXXXXXXXX";

    CharacterData champData = new CharacterData();

    string fixJson(string jsonname, string value)
    {
        value = "{\"" + jsonname + "\":" + value + "}";
        return value;
    }
   
    public void LoadNFTCharacterInGame()
    {
        if (string.IsNullOrEmpty(Account.metamask))
        {
            NotiveManager.get.notiveInfo("Metamask not connected!");
            return;
        }
        GameManager.get.listWalletCharacterNFTs.Clear();

        GetAllERC721InAccount(); //Get All NFTs in account
    }

    #region Contract Methods
    int NumOfNFTsToLoad;
    int balance;
    async void GetAllERC721InAccount()
    {
        NotiveManager.get.notiveInfo();
#if UNITY_IOS
        string wallet = Account.metamask;
#else
        string wallet = PlayerPrefs.GetString("Account");
#endif
        balance = await ERC721.BalanceOf(contract, wallet);

        if (balance <= 0)
        {
            Debug.Log("Balance less than 0");
            NotiveManager.get.notiveInfo("No NFT found!");
        }
        else
        {
            NumOfNFTsToLoad = balance;
            await CallTokenOfOwnerByIndexMethod(NumOfNFTsToLoad - 1); //Get Token ID
        }
    }

    async Task CallTokenOfOwnerByIndexMethod(int ind)
    {
        NotiveManager.get.notiveInfo();

        string method = "tokenOfOwnerByIndex";
        JsonRpcProvider provider = RPC.GetInstance.Provider();
        Contract Mthdcontract = new Contract(ContractAbi, contract, provider);
        object[] contractData = await Mthdcontract.Call(method, new object[2]
        {
#if UNITY_IOS
            (object) Account.metamask,
#else
            (object) PlayerPrefs.GetString("Account"),
#endif
            (object) ind
        });

        Debug.Log("TokenId: " + contractData[0].ToString());

        //Get Token Uri
        await CallContractMethodTokenURI(int.Parse(contractData[0].ToString()));
    }

    async Task CallContractMethodTokenURI(int tokenid)
    {
        string method = "tokenURI";

        JsonRpcProvider provider = RPC.GetInstance.Provider();
        Contract Mthdcontract = new Contract(ContractAbi, contract, provider);

        object[] contractData = await Mthdcontract.Call(method, new object[]
        {
            (object) tokenid,
        });

        string tokenuri = contractData[0].ToString();

        //Get metadata from uri
        await GetNFTMetaData(tokenuri);
    }
    #endregion

    public async Task GetNFTMetaData(string uri)
    {
        if (string.IsNullOrEmpty(uri) || uri.Contains("error"))
        {
            Debug.Log("Error reading metadata");
        }
        else
        {
            //fetch json from uri
            UnityWebRequest webRequest = UnityWebRequest.Get(uri);
            await webRequest.SendWebRequest();
            //save NFT data
            champData = hero.GetComponent<Character>().GetCharacterJson(Encoding.UTF8.GetString(webRequest.downloadHandler.data));
            champData.isNFT = true;
            champData.characterPermanentName = champData.characterName;
            champData.NFTuri = uri;

            //Get NFT data from DB to compare with latest metadata
            GetNFTDataFromDB();
        }
    }


    void GetNFTDataFromDB()
    {
        string CategoryEntry = champData.characterPermanentName.Split(" ")[0];
        string NameEntry = champData.characterPermanentName.Replace(" #", "_");
        Database.get.GetCharacterNFTData(CategoryEntry, NameEntry, async (check, message) =>
        {
            if (check)
            {
                var jsonn = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
                champData.characterJson = jsonn["characterJson"].ToString();
                Debug.Log(champData.characterJson);

                var key = champData.tokenID;

                //To check within the game if NFT has been transferred outside of the game
                bool TransferredNFT = false;
                if (jsonn.ContainsKey("OwnerID"))
                {
                    string owneridDB = jsonn["OwnerID"].ToString();
                    if (owneridDB.ToLower().Equals(Account.metamask.ToLower()))
                    {
                        //If the current ownwer id obtained from NFT is the same as the owner id stored in the game database, this means the NFT was NOT transferred
                        TransferredNFT = false;
                        Debug.Log("NFT has NOT been transferred");
                    }
                    else
                    {
                        //If the current ownwer id from NFT is different that means the owner id in the DB is of old owner => NFT was transferred
                        TransferredNFT = true;
                        Debug.Log("NFT has been transferred");
                    }
                }


                if (!TransferredNFT) //NFT has not been transferred
                {
                    await OnNotNftTransferredAsync(jsonn, key, false);
                }
                else //NFT has been transferred
                {
                    OnNftTransfer(jsonn, CategoryEntry, NameEntry);
                }
            }
        }, true);
    }
}



[Serializable]
public class NFTCharData
{
    public string name;
    public string HeroClass;
    public string tokenid;

    public NFTCharData(string name, string heroClass, string tokenid)
    {
        this.name = name;
        HeroClass = heroClass;
        this.tokenid = tokenid;
    }
}

[Serializable]
public class NFTCharDataList
{
    public List<NFTCharData> nftchampions;
}

[Serializable]
public class ItemsList
{
    public List<dataParams> listitems;
}

