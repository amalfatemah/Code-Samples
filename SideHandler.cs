using Firebase.Storage;
using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class SideHandler : MonoBehaviour
{
    public BoxSides thisSide;

    string[] paths;

    public Toggle IsImage;
    public Button ImageUploadBtn;
    public GameObject ImageUpload;
    public RawImage DisplayImage;
    public InputField VideoUrl;
    public InputField Answer;

    public void OnToggleValueChange()
    {
        if (IsImage.isOn)
        {
            ImageUpload.SetActive(true);
            VideoUrl.gameObject.SetActive(false);
        }
        else
        {
            ImageUpload.SetActive(false);
            VideoUrl.gameObject.SetActive(true);
        }
    }


    public void ActivateImageOption()
    {
        ImageUpload.SetActive(true);
        VideoUrl.transform.parent.gameObject.SetActive(false);
    }

    public void ActivateVideoOption()
    {
        ImageUpload.SetActive(false);
        VideoUrl.transform.parent.gameObject.SetActive(true);
    }

    public void OnClickUploadPicture()
    {
        var extensions = new[] { new ExtensionFilter("Image Files", "png", "jpg", "jpeg") };
        paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);
        Debug.Log(paths[0]);
        if (paths.Length > 0)
        {
            StartCoroutine(GetTexture(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    IEnumerator GetTexture(string url)
    {
        UIManager.Instance.ActivateLoadingScreen(true);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            DisplayImage.texture = myTexture;
            FirebaseManager.Instance.UploadImage(www.downloadHandler.data, BoxData.myBoxInfo.BoxName, thisSide, OnPictureUploadCompleted);
        }
    }

    public void OnPictureUploadCompleted(StorageMetadata data, UploadStatus result)
    {
        if (result == UploadStatus.Success)
        {
            Debug.Log("Success: Picture Uploaded: " + data.Path);
            //Add image url to DB
            BoxData.myBoxInfo.Sides[(int)thisSide] = "true-" + data.Path;

            UIManager.Instance.ActivateLoadingScreen(false);

        }
        else if (result == UploadStatus.Error)
        {
            Debug.Log("Error: Picture not uploaded");
        }
    }

    public void OnEnterVideoUrl()
    {
        BoxData.myBoxInfo.Sides[(int)thisSide] = "false-" + VideoUrl.text;
    }

    public void SetVideoUrl(string url)
    {
        this.VideoUrl.text = url;
    }

    public void SetTexture(Texture2D texture)
    {
        DisplayImage.texture = texture;
    }
}
