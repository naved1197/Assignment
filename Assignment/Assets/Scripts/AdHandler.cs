using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

#region Json Class Object
[Serializable]
public class Position
{
    public int x;
    public int y;
    public int width;
    public int height;
}
[Serializable]
public class Placement
{
    public Position position;
}
[Serializable]
public class Operations
{
    public string name;
    public string argument;
}
[Serializable]
public class Layer
{
    public string type;
    public string path;
    public List<Placement> placement;
    public List<Operations> operations;
}
[Serializable]
public class Root
{
    public List<Layer> layers;
}
#endregion
public class AdHandler : MonoBehaviour
{
    [Space]
    [Header("Rendering")]
    [SerializeField]private Transform canvasParent;
    [SerializeField]private GameObject textlayer;
    [SerializeField]private GameObject framelayer;
    [SerializeField]private GameObject loading;
    [Space]
    [Header("User Inputs")]
    [SerializeField] private TMP_InputField userInput;
    [SerializeField] private GameObject inputPanel;
    [SerializeField] private GameObject templatePanel;
    [Space]
    [Header("Error")]
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private GameObject errorPanel;
    private Root layer;
    private Texture2D texture=null;
    private Color color=Color.black;
    private string inputTxt = "";
    private const string textOnly = "http://lab.greedygame.com/arpit-dev/unity-assignment/templates/text_only.json";
    private const string textColor = "http://lab.greedygame.com/arpit-dev/unity-assignment/templates/text_color.json";
    private const string frameOnly = "http://lab.greedygame.com/arpit-dev/unity-assignment/templates/frame_only.json";
    private const string frameColor = "http://lab.greedygame.com/arpit-dev/unity-assignment/templates/frame_color.json";

    #region UI functions
    //get the user input
    public void GetUserInput()
    {
        inputTxt = userInput.text;
        if (inputTxt.Length > 0)
        {
            templatePanel.SetActive(true);
            inputPanel.SetActive(false);
        }
        else
        {
            HandleError("Text is empty");
            userInput.text = "";
        }
    }
    //Select and show the template
    public void SelectTemplate(int type)
    {
        switch (type)
        {
            case 0:
                StartCoroutine(ProcessRequest(textOnly));
                break;
            case 1:
                StartCoroutine(ProcessRequest(textColor));
                break;
            case 2:
                StartCoroutine(ProcessRequest(frameOnly));
                break;
            case 3:
                StartCoroutine(ProcessRequest(frameColor));
                break;
            case 4:
                StartCoroutine(ProcessRequest(textOnly));
                StartCoroutine(ProcessRequest(frameOnly));
                break;
            default: HandleError("Select one");
                break;
        }
        templatePanel.SetActive(false);
        loading.SetActive(true);
    }
    //Handle Error
    void HandleError(string txt)
    {
        loading.SetActive(false);
        errorMessage.text = txt;
        errorPanel.SetActive(true);
    }
    //Reset the scene
    public void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    #endregion
    #region Rendering

    //Get Json Template from the url
    private IEnumerator ProcessRequest(string uri)
    {
        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();
        if (request.result !=UnityWebRequest.Result.Success)
        {
            HandleError("Reset Scene");
        }
        else
        {
           // Debug.Log(request.downloadHandler.text);
            layer = JsonUtility.FromJson<Root>(request.downloadHandler.text);
            RenderLayers(layer.layers[0]);
        }
    }
    //Render layers according to the type
    void RenderLayers(Layer item)
    {
        if (item.type.Equals("text"))
        {
            Position p = item.placement[0].position;
            if (item.operations.Count == 0)
                RenderTextLayer(p.x, p.y, p.width, p.height);
            else
            {
                Operations operation = item.operations[0];
                switch (operation.name)
                {
                    case "color":
                        RenderTextLayer(p.x, p.y, p.width, p.height, operation.argument);
                        break;
                    default:
                        RenderTextLayer(p.x, p.y, p.width, p.height);
                        break;
                }
            }

        }
        else if (item.type.Equals("frame"))
        {
            Position p = item.placement[0].position;
            if (item.operations.Count == 0)
                StartCoroutine(RenderFrameLayer(item.path, p.x, p.y, p.width, p.height));
            else
            {
                Operations operation = item.operations[0];
                switch (operation.name)
                {
                    case "color":
                        StartCoroutine(RenderFrameLayer(item.path, p.x, p.y, p.width, p.height, operation.argument));
                        break;
                    default:
                        StartCoroutine(RenderFrameLayer(item.path, p.x, p.y, p.width, p.height));
                        break;
                }
            }
        }
    }
    //Render Text Layers
    void RenderTextLayer(int x,int y,int width,int height,string hexCode=null)
    {
        if(hexCode!=null)
        {
            if (ColorUtility.TryParseHtmlString(hexCode, out color))
            {
                ;
            }
            else
            {
                color = Color.black;
            }
        }
        loading.SetActive(false);
        textlayer.GetComponent<TextLayer>().InitialiseText(inputTxt, x, y, width, height, color);
        textlayer.SetActive(true);
    }
    //Render frame layers
    IEnumerator RenderFrameLayer(string url, int x, int y, int width, int height, string hexCode = null)
    {
        yield return StartCoroutine(LoadAndRender(url));
        if (hexCode != null)
        {
            if (ColorUtility.TryParseHtmlString(hexCode, out color))
            {
                ;
            }
            else
            {
                color = Color.white;
            }
        }
        else
        {
            color = Color.white;
        }
        loading.SetActive(false);
        framelayer.GetComponent<FrameLayer>().IntialiseFrame(texture, x, y, width, height, color);
        framelayer.SetActive(true);

    }

    //Load image from the path
    IEnumerator LoadAndRender(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            texture = null;
        }
        else
        {
            texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
    #endregion
}
