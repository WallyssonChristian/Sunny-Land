using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{

    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartHalf;
    [SerializeField] private Sprite heartEmpty;

    private TextMeshProUGUI cherryCount;
    private int cherrys = 0;
    private Text debugLog;
    private Image heart;

    void Start()
    {
        cherryCount = GameObject.Find("CherryCount").GetComponent<TextMeshProUGUI>();
        debugLog = GameObject.Find("DebugLog").GetComponent<Text>();
        heart = GameObject.Find("Heart1").GetComponent<Image>();
    }

    public void AddCherryScore() {
        cherrys += 1;
        cherryCount.text = cherrys.ToString();
    }

    public void UpdateFoxLife(int currentLife) {

        switch (currentLife) {
            case 0:
                heart.sprite = heartEmpty;
                break;
            case 1:
                heart.sprite = heartHalf;
                break;
            default:
                heart.sprite = heartFull;
                break;
        }

    }

    public void UpdateDebugLog(int valor) {
        debugLog.text = valor.ToString();
    }

}
