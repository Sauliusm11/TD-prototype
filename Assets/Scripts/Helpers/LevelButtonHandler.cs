using TMPro;
using UnityEngine;

public class LevelButtonHandler : MonoBehaviour
{
    TMP_Text buttonText;
    GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Clicked()
    {
        buttonText = transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        manager.ConfirmLoading(buttonText.text.ToString());
    }
}
