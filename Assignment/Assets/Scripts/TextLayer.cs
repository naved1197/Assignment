using TMPro;
using UnityEngine;

public class TextLayer : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI displayTxt;

    public void InitialiseText(string text,int x,int y,int width,int height,Color c)
    {
        displayTxt.text = text;
        RectTransform t = displayTxt.GetComponent<RectTransform>();
        t.sizeDelta = new Vector2(width, height);
        t.anchoredPosition = new Vector2(x, y);
        displayTxt.color = c;
    }
}
