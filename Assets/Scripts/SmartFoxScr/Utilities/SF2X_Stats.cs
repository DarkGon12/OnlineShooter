using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

public class SF2X_Stats : MonoBehaviour
{
    private int AverageFPS { get; set; }
    private int[] fpsBuffer;
    private int fpsBufferIndex;
    private int frameRange = 120;
    public TMP_Text frameRate;
    public TMP_Text pingRate;
    public TMP_Text version;

    private void Start()
    {
        if (Resources.Load<TextAsset>("BuildDate"))
        {
            string dt = Resources.Load<TextAsset>("BuildDate").text;

            const string regex = "";

            string sanitised = Regex.Replace(dt, regex, string.Empty);

            version.SetText("Build: " + sanitised);
        }
    }

    void Update()
    {
        if (fpsBuffer == null || fpsBuffer.Length != frameRange)
        {
            InitializeBuffer();
        }
        UpdateBuffer();
        CalculateFPS();
        pingRate.SetText(NetworkManager.Instance.clientServerLag.ToString() + " ms Network Lag");

    }

    void InitializeBuffer()
    {
        if (frameRange <= 0)
        {
            frameRange = 1;
        }
        fpsBuffer = new int[frameRange];
        fpsBufferIndex = 0;
    }

    void UpdateBuffer()
    {
        fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (fpsBufferIndex >= frameRange)
        {
            fpsBufferIndex = 0;
        }
    }
    void CalculateFPS()
    {
        int sum = 0;
        for (int i = 0; i < frameRange; i++)
        {
            sum += fpsBuffer[i];
        }
        AverageFPS = sum / frameRange;
        frameRate.SetText(AverageFPS.ToString() + " fps for Device");
    }
  

}
