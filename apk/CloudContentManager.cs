using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class CloudContentManager : MonoBehaviour
{
    #region PRIVATE_CONSTANTS
    const string JSON_URL = "https://citigio-demo-api.herokuapp.com/product/";
    // JSON File Key Strings
    const string TITLE_KEY = "name";
    const string AVERAGE_RATING_KEY = "rating";
    //const string THUMB_URL_KEY = "thumburl";
    const string INFO_URL_KEY = "infourl";
    const string STORE1_NAME = "store1name";
    const string STORE2_NAME = "store2name";
    const string STORE3_NAME = "store3name";
    const string PRICE_1 = "price1";
    const string PRICE_2 = "price2";
    const string PRICE_3 = "price3";
    const string STORES = "stores";

    #endregion // PRIVATE_CONSTANTS

    #region PROPERTIES
    string Title { get; set; }
    int AverageRating { get; set; }
    string Store1 { get; set; }
    int Price1 { get; set; }
    string Store2 { get; set; }
    int Price2 { get; set; }
    string Store3 { get; set; }
    int Price3 { get; set; }
    int Stores { get; set; }
    //string ImageUrl { get; set; }
    string BrowserURL { get; set; }
    #endregion // PROPERTIES


    #region PUBLIC_MEMBERS
    //public Image m_LoadingIndicator;
    //public Image m_Cover;
    public Text m_Title;
    public Text m_RatingStars;
    public Text m_Store1;
    public Text m_Price1;
    public Text m_Store2;
    public Text m_Price2;
    public Text m_Store3;
    public Text m_Price3;
    public Text m_Stores;
    #endregion PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    readonly string[] starRatings = { "☆☆☆☆☆", "★☆☆☆☆", "★★☆☆☆", "★★★☆☆", "★★★★☆", "★★★★★" };
    bool wwwRequestInProgress;
    #endregion PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Update()
    {
        //if (wwwRequestInProgress && m_LoadingIndicator)
        //{
        //    m_LoadingIndicator.rectTransform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
        //}
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void HandleMetadata(string metadata)
    {
        // metadata string will be in the following format: name.json
        string fullURL = JSON_URL + metadata;
        

        StartCoroutine(WebRequest(fullURL));
    }

    public void ClearBookData()
    {
        //m_Cover.sprite = null;
        //m_Cover.color = Color.black;
        m_Title.text = "KiotViet";
        m_RatingStars.text = starRatings[0];
        m_Stores.text = string.Format("Number of stores: {}", 0);
        m_Store1.text = "KiotViet";
        m_Store2.text = "KiotViet";
        m_Store3.text = "KiotViet";
        m_Price1.text = string.Format("Price: {0} VNĐ", "0");
        m_Price2.text = string.Format("Price: {0} VNĐ", "0");
        m_Price3.text = string.Format("Price: {0} VNĐ", "0");
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
        m_RatingStars.text = starRatings[AverageRating];
        m_Stores.text = string.Format("Stores: {0}", AverageRating);
        m_Store1.text = "1." + Store1;
        m_Store2.text = "2." + Store2;
        m_Store3.text = "3." + Store3;
        m_Price1.text = string.Format("Pirce: {0} VNĐ", Price1);
        m_Price2.text = string.Format("Pirce: {0} VNĐ", Price2);
        m_Price3.text = string.Format("Pirce: {0} VNĐ", Price3);
    }

    void ProcessWebRequest(WWW www)
    {
        Debug.Log("ProcessWebRequest() called: \n" + www.url);

        if (www.url.Contains(".json"))
        {
            ParseJSON(www.text);
            www.Dispose();
            UpdateBookTextData();
            //StartCoroutine(WebRequest(ImageUrl));
        }
        //else if ((www.url.Contains(".png") || www.url.Contains(".jpg")) && www.texture != null)
        //{
        //    if (m_Cover)
        //    {
        //        m_Cover.sprite = Sprite.Create(www.texture,
        //                                       new Rect(0, 0, www.texture.width, www.texture.height),
        //                                       new Vector2(0.5f, 0.5f));
        //        m_Cover.color = Color.white;
        //        www.Dispose();
        //    }
        //}
    }

    IEnumerator WebRequest(string url)
    {
        Debug.Log("WebRequest() called: \n" + url);

        wwwRequestInProgress = true;
        //m_LoadingIndicator.enabled = true;

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
        //    m_LoadingIndicator.enabled = false;
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
        char[] trimChars = { '{', '}', ' ', '[', ']' };

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
                    case AVERAGE_RATING_KEY:
                        AverageRating = int.Parse(keyValuePair[1], CultureInfo.InvariantCulture);
                        break;
                    case STORES:
                        Stores = int.Parse(keyValuePair[1], CultureInfo.InvariantCulture);
                        break;
                    case STORE1_NAME:
                        Store1 = keyValuePair[1];
                        break;
                    case STORE2_NAME:
                        Store2 = keyValuePair[1];
                        break;
                    case STORE3_NAME:
                        Store3 = keyValuePair[1];
                        break;
                    case PRICE_1:
                        Price1 = int.Parse(keyValuePair[1], CultureInfo.InvariantCulture);
                        break;
                    case PRICE_2:
                        Price2 = int.Parse(keyValuePair[1], CultureInfo.InvariantCulture);
                        break;
                    case PRICE_3:
                        Price3 = int.Parse(keyValuePair[1], CultureInfo.InvariantCulture);
                        break;
                    //case THUMB_URL_KEY:
                    //    ImageUrl = keyValuePair[1];
                    //    break;
                    case INFO_URL_KEY:
                        BrowserURL = keyValuePair[1];
                        break;
                }
            }
        }
    }

    #endregion //PRIVATE_METHODS
}
