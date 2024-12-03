using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using HuggingFace.API;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class SpeechRecognition : MonoBehaviour
{

    [SerializeField] private bool _recording = false;
    [SerializeField] private TMP_Text _recognizedText;

    [Tooltip("The input field where the user's name will be displayed.")]
    public TMP_InputField nameInputField;

    [Tooltip("The input field where the user's speech will be displayed.")]
    public TMP_InputField targetInputField;

    public TMP_Text recordingNotification;
    public bool canRecord = false;
    [Tooltip("If true, the AI will respond directly to the user's speech, else it will be sent to the text box first.")]
    public bool directReponses = false;
    //[SerializeField] private TextMeshProUGUI _recognizedText;
    public AI_Manager AI_Manager;

    public InputActionProperty m_RightHandRecordAction = new InputActionProperty(new InputAction("Right Hand Record", expectedControlType: "float"));
    public InputActionProperty RightHandRecordAction
    {
        get => m_RightHandRecordAction;
        set => SetInputActionProperty(ref m_RightHandRecordAction, value);
    }

    public InputActionProperty m_LeftHandRecordAction = new InputActionProperty(new InputAction("Left Hand Record", expectedControlType: "float"));
    public InputActionProperty LeftHandRecordAction
    {
        get => m_LeftHandRecordAction;
        set => SetInputActionProperty(ref m_LeftHandRecordAction, value);
    }

    private AudioClip _audioClip;
    private byte[] _audioData;


    // Start is called before the first frame update
    void Start()
    {
        //StartRecording();

    }

    void Update()
    {
        if (canRecord) { 
            float recordPressed = ReadRecordButtons();
            if (!_recording && recordPressed > 0)
            {
                _recording = true;
                Debug.Log("BUTTON PRESSED!");
                recordingNotification.text = "-RECORDING!-";
                StartRecording();
            }

            if (_recording && !(recordPressed > 0))
            {
                _recording = false; 
                recordingNotification.text = "Hold A to record";
                Debug.Log("BUTTON RELEASED!");
                StopRecording();
            }
        }
    }

        // recording Name Input on Main Menu
        public void RecordNameForDuration(float duration)
    {
        if (Microphone.IsRecording(null))
        {
            Debug.LogWarning("Already recording!");
            return;
        }

        Debug.Log($"Recording for {duration} seconds...");
        _audioClip = Microphone.Start(null, true, Mathf.CeilToInt(duration), 44100);

        StartCoroutine(StopRecordingAfterDuration(duration));
    }

    private IEnumerator StopRecordingAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (!Microphone.IsRecording(null))
        {
            Debug.LogWarning("Not recording!");
            yield break;
        }

        int postion = Microphone.GetPosition(null);
        Microphone.End(null);

        if (_audioClip == null || postion <= 0)
        {
            Debug.LogError("No Audio Recorded");
            yield break;
        }

        float[] samples = new float[postion * _audioClip.channels];
        _audioClip.GetData(samples, 0);
        _audioData = EncodeAsWAV(samples, _audioClip.frequency, _audioClip.channels);

        ProcessNameInput();
    }

    private void ProcessNameInput()
    {
        HuggingFaceAPI.AutomaticSpeechRecognition(_audioData, (response) =>
        {
            Debug.Log($"Recognized Name before REGEX: {response}");
            response = CleanName(response);
            Debug.Log($"Recognized Name after REGEX: {response}");
            targetInputField.text = response;
        }, error =>
        {
            Debug.LogError(error);
        });
    }

    private string CleanName(string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            Debug.LogWarning("Name is empty!");
            return response;
        }

        response = System.Text.RegularExpressions.Regex.Replace(response, @"\.+$", "");

        return response;
    }

    public void StartRecording()
    {
        _recognizedText.text = "Listening...";
        _audioClip = Microphone.Start(null, true, 300, 44100);
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
            if (directReponses)
            {
                AI_Manager.GenerateAiResponse(response);
                targetInputField.text = response;
            }
            else
            {
                targetInputField.text = response;
            }
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

    //on VR A press, start recording
    void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
    {
        if (Application.isPlaying)
            property.DisableDirectAction();

        property = value;

        if (Application.isPlaying && isActiveAndEnabled)
            property.EnableDirectAction();
    }

    public float ReadRecordButtons()
    {
        var leftHandValue = m_LeftHandRecordAction.action?.ReadValue<float>() ?? 0.0f;
        var rightHandValue = m_RightHandRecordAction.action?.ReadValue<float>() ?? 0.0f;

        return leftHandValue + rightHandValue;
    }

    public void SetCanRecord(bool value)
    {
        canRecord = value;
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
