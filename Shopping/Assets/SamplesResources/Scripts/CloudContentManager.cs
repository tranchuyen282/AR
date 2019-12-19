using System;
using System.Collections;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CloudContentManager : MonoBehaviour
{
    #region PRIVATE_CONSTANTS
    // const string JSON_URL = "http://localhost:3000/";
    //const string JSON_URL = "https://citigo-restfulapi-demo.herokuapp.com/product/";
    const string JSON_URL = "https://citigo-restapi-demo.herokuapp.com/product/";
    // JSON File Key Strings
    const string TITLE_KEY = "title";
    const string AUTHOR_KEY = "author";
    const string AVERAGE_RATING_KEY = "average rating";
    const string QUANTITY = "quantity";
    const string YOUR_PRICE_KEY = "your price";
    const string THUMB_URL_KEY = "thumburl";
    const string INFO_URL_KEY = "infourl";
    #endregion // PRIVATE_CONSTANTS

    #region PROPERTIES
    string Title { get; set; }
    string Author { get; set; }
    int AverageRating { get; set; }
    int NumberOfRatings { get; set; }
    //float ListPrice { get; set; }
    float YourPrice { get; set; }
    string ImageUrl { get; set; }
    string BrowserURL { get; set; }
    #endregion // PROPERTIES


    #region PUBLIC_MEMBERS
    public Image m_LoadingIndicator;
    public Image m_Cover;
    public Text m_Title;
    public Text m_Author;
    public Text m_RatingStars;
    public Text m_RatingCount;
    //public Text m_ListPrice;
    public Text m_YourPrice;
    #endregion PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    readonly string[] starRatings = { "☆☆☆☆☆", "★☆☆☆☆", "★★☆☆☆", "★★★☆☆", "★★★★☆", "★★★★★" };
    bool wwwRequestInProgress;
    #endregion PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Update()
    {
        if (wwwRequestInProgress && m_LoadingIndicator)
        {
            m_LoadingIndicator.rectTransform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
        }
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void HandleMetadata(string metadata)
    {
        // metadata string will be in the following format: samplebook[1-3].json
        // concatenate the metadata string filename to the base JSON URL:
        // https://developer.vuforia.com/samples/cloudreco/json/samplebook[#].json
        //string JSON_URL = "http://"+ readURL() + ":3000/";
        string fullURL = JSON_URL + metadata;
        Debug.Log("FullURL: " + fullURL);
        // string fullURL = "http://localhost:3000/book.json";
        StartCoroutine(WebRequest(fullURL));
    }

    private string readURL()
    {
        string url = PlayerPrefs.GetString("ip", "");
        return url.Trim();
    }

    public void ClearBookData()
    {
        m_Cover.sprite = null;
        m_Cover.color = Color.black;
        m_Title.text = "Title";
        m_Author.text = "Author";
        m_RatingStars.text = starRatings[0];
        m_RatingCount.text = string.Format("Quantity: {0}", 0);
       // m_ListPrice.text = string.Format("${0}", "00.00");
        m_YourPrice.text = string.Format("Your Price\n{0} VNĐ", "0");
    }
    #endregion // PUBLIC_METHODS


    #region BUTTON_METHODS
    public void MoreInfoButton()
    {
        Application.OpenURL(BrowserURL);
    }
    #endregion // BUTTON_METHODS


    #region PRIVATE_METHODS
    void UpdateBookTextData()
    {
        Debug.Log("UpdateBookTextData() called.");

        m_Title.text = Title;
        m_Author.text = Author;
        m_RatingStars.text = starRatings[AverageRating];
        m_RatingCount.text = string.Format("Quantity: {0}", NumberOfRatings);
        m_YourPrice.text = string.Format("Your Price\n{0} VNĐ", YourPrice);
    }

    void ProcessWebRequest(WWW www)
    {
        Debug.Log("ProcessWebRequest() called: \n" + www.url);

        if (www.url.Contains(".json"))
        {
            ParseJSON(www.text);
            www.Dispose();
            UpdateBookTextData();
            StartCoroutine(WebRequest(ImageUrl));
        }
        else if ((www.url.Contains(".png") || www.url.Contains(".jpg")) && www.texture != null)
        {
            if (m_Cover)
            {
                m_Cover.sprite = Sprite.Create(www.texture,
                                               new Rect(0, 0, www.texture.width, www.texture.height),
                                               new Vector2(0.5f, 0.5f));
                m_Cover.color = Color.white;
                www.Dispose();
            }
        }
    }

    IEnumerator WebRequest(string url)
    {
        Debug.Log("WebRequest() called: \n" + url);

        wwwRequestInProgress = true;
        m_LoadingIndicator.enabled = true;

        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("WebRequest() failed. Your URL is null or empty.");
            yield return null;
        }

        WWW www = new WWW(url);
        yield return www;

        if (www.isDone)
        {
            Debug.Log("Done Loading: \n" + www.url);
            wwwRequestInProgress = false;
            m_LoadingIndicator.enabled = false;
        }

        if (string.IsNullOrEmpty(www.error))
        {
            // If error string is null or empty, then request was successful
            ProcessWebRequest(www);
        }
        else
        {
            Debug.LogError("Error With WWW Request: " + www.error);

            string error = "<color=red>" + www.error + "</color>" + "\nURL Requested: " + www.url;

            MessageBox.DisplayMessageBox("WWW Request Error", error, true, null);
        }
    }

    /// <summary>
    /// Parses a JSON string and returns a book data struct from that
    /// </summary>
    void ParseJSON(string jsonText)
    {
        Debug.Log("ParseJSON() called: \n" + jsonText);

        // Remove opening and closing braces and any spaces
        char[] trimChars = { '{', '}', ' ' ,'[',']'};

        // Remove double quote and new line chars from the JSON text
        jsonText = jsonText.Trim(trimChars).Replace("\"", "").Replace("\n", "");
        //jsonText = jsonText.Trim(trimChars);
        string[] jsonPairs = jsonText.Split(',');

        Debug.Log("# of JSON pairs: " + jsonPairs.Length);

        foreach (string pair in jsonPairs)
        {
            
           
            // Split pair into a max of two strings using first colon
            string[] keyValuePair = pair.Split(new char[] { ':' }, 2);
            keyValuePair[0] = keyValuePair[0].Trim();
            keyValuePair[1] = keyValuePair[1].Trim();
            if (keyValuePair.Length == 2)
            {
                switch (keyValuePair[0])
                {
                    
                    case TITLE_KEY:
                        Title = keyValuePair[1];
                        break;
                    case AUTHOR_KEY:
                        Author = keyValuePair[1];
                        break;
                    case AVERAGE_RATING_KEY:
                        AverageRating = int.Parse(keyValuePair[1], CultureInfo.InvariantCulture);
                        break;
                    case QUANTITY:
                        NumberOfRatings = int.Parse(keyValuePair[1], CultureInfo.InvariantCulture);
                        break;
                    case YOUR_PRICE_KEY:
                        YourPrice = float.Parse(keyValuePair[1], CultureInfo.InvariantCulture);
                        break;
                    case THUMB_URL_KEY:
                        ImageUrl = keyValuePair[1];                   
                        break;
                    case INFO_URL_KEY:
                        BrowserURL = keyValuePair[1];
                        break;
                }
            }
        }
    }

    #endregion //PRIVATE_METHODS
}
