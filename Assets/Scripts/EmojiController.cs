using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiController : MonoBehaviour
{
    public GameObject Emoji;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    private void Start()
    {
        button1.onClick.AddListener(() => ChangeSprite(button1.GetComponent<Image>().sprite));
        button2.onClick.AddListener(() => ChangeSprite(button2.GetComponent<Image>().sprite));
        button3.onClick.AddListener(() => ChangeSprite(button3.GetComponent<Image>().sprite));
        button4.onClick.AddListener(() => ChangeSprite(button4.GetComponent<Image>().sprite));
    }

    private void ChangeSprite(Sprite newSprite)
    {
        Emoji.GetComponent<Image>().sprite = newSprite;
    }



}
