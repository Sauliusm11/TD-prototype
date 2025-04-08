using System.Collections;
using TMPro;
using UnityEngine;
/// <summary>
/// Basic fps counter taken from unity discusions and adapted for the project.
/// </summary>
public class FpsCounter : MonoBehaviour
{

    TMP_Text text;
    public int Fps { get; private set; }
    [SerializeField] private float fpsRefreshTime = 1f;

    private WaitForSecondsRealtime _waitForSecondsRealtime;

    private void OnValidate()
    {
        SetWaitForSecondsRealtime();
    }

    private IEnumerator Start()
    {
        text = gameObject.GetComponent<TMP_Text>();
        SetWaitForSecondsRealtime();

        while (true)
        {
            Fps = (int)(1 / Time.unscaledDeltaTime);
            text.text = "Fps: " + Fps;
            yield return _waitForSecondsRealtime;
        }
    }

    private void SetWaitForSecondsRealtime()
    {
        _waitForSecondsRealtime = new WaitForSecondsRealtime(fpsRefreshTime);
    }

}
