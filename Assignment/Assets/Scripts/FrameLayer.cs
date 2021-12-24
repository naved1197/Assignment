using UnityEngine.UI;
using UnityEngine;

public class FrameLayer : MonoBehaviour
{
    [SerializeField] private RawImage image;
   public void IntialiseFrame(Texture2D img, int x, int y, int width, int height, Color c)
    {
        image.texture = img;
        RectTransform t = image.GetComponent<RectTransform>();
        t.sizeDelta = new Vector2(width, height);
        t.anchoredPosition = new Vector2(x, y);
        image.color = c;
    }
}
