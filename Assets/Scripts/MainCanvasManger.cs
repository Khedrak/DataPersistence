using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvasManger : MonoBehaviour
{
    public Text bestScoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bestScoreText.text = GameManager.Instance.GetBestScore();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
