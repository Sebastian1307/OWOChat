using UnityEngine;

public class CallUIManager : MonoBehaviour
{
    public UDPVideoSender videoSender;
    public UDPAudioSender audioSender;

    public void ToggleCamera()
    {
        videoSender.ToggleCamera();
    }

    public void ToggleMic()
    {
        audioSender.ToggleMic();
    }

    public void EndCall()
    {
        Application.Quit();
    }
}
