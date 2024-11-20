using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using HuggingFace.API;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;

public class SpeechRecognition : MonoBehaviour
{
    [SerializeField] private bool _recording;
    [SerializeField] private TMP_Text _recognizedText;
    //[SerializeField] private TextMeshProUGUI _recognizedText;
    public Ai_Manager AI_Manager;

    private AudioClip _audioClip;
    private byte[] _audioData;


    // Start is called before the first frame update
    void Start()
    {
        //StartRecording();
        
    }

    public void StartRecording()
    {
        _recognizedText.text = "Listening...";
        _audioClip = Microphone.Start(null, true, 300, 44100);
    }

    public void FixedUpdate()
    {
       //if (!_recording)
       //{
       //    StopRecording();
       //}
    }

    public void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * _audioClip.channels];
        if (samples.Length == 0)
        {
            Debug.LogError($"Position is {position}, no audio recorded!");
            return;
        }
        _audioClip.GetData(samples, 0);
        _audioData = EncodeAsWAV(samples, _audioClip.frequency, _audioClip.channels);
        _recording = false;
        _recognizedText.text = "Stopped Listening...";
        SendRecording();
    }

    void SendRecording()
    {
        HuggingFaceAPI.AutomaticSpeechRecognition(_audioData, (response) =>
        {
            _recognizedText.text = response;
            AI_Manager.GenerateAiResponse(response);
        }, error =>
        {
           _recognizedText.text = error;
        });
    }

    byte[] EncodeAsWAV(float[] samples, int sampleRate, int channels)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
        writer.Write(36 + samples.Length * 2);
        writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
        writer.Write(new char[4] { 'f', 'm', 't', ' ' });
        writer.Write(16);
        writer.Write((ushort)1);
        writer.Write((ushort)channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * 2);
        writer.Write((ushort)(2));
        writer.Write((ushort)16);
        writer.Write(new char[4] { 'd', 'a', 't', 'a' });
        writer.Write(samples.Length * 2);
        foreach (var sample in samples)
        {
            writer.Write((short)(sample * 32767));
        }

        writer.Close();
        return stream.ToArray();
    }
}

#region GUI CONTROLS
[CustomEditor(typeof(SpeechRecognition))]
public class InspectorButtonsNEW : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SpeechRecognition myScript = (SpeechRecognition)target;
        if (GUILayout.Button("Start Recording")) //can name as anything
        {
            myScript.StartRecording();
        }
        if (GUILayout.Button("Stop Recording"))
        {
            myScript.StopRecording();
        }
    }
}
#endregion
