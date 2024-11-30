using UnityEngine;
using UnityEditor;
using Claudia;
using System;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Audio;
using System.Linq;
using ElevenLabs.Models;
using UnityEditor.VersionControl;
using UnityEngine.UI;

using static System.Net.Mime.MediaTypeNames;
using ElevenLabs;
using ElevenLabs.TextToSpeech;


public class AI_Manager : MonoBehaviour
{
    private class ClaudeModels
    {
        public const string ClaudeHaikuLatest = "claude-3-5-haiku-latest";
        public const string Claude3_5Haiku = "claude-3-5-haiku-20241022";

        public const string ClaudeSonnetLatest = "claude-3-5-sonnet-latest";
        public const string Claude3_5Sonnet = "claude-3-5-sonnet-20241022";

    }
    public enum ManagerModels
    {
        ClaudeHaiku,
        ClaudeSonnet
    }
     
    private readonly String[] claudeModel = new String[] { 

        ClaudeModels.ClaudeHaikuLatest,
        ClaudeModels.ClaudeSonnetLatest,
    };

    // <summary> The difficulty of the interview questions. <summary>
    public enum QuestionDifficulties
    {
        Basic,
        Easy,
        Medium,
        Hard,
        Expert,
        Master,
    }

    [Tooltip("HAIKU: is lightweight and cheap; Good for testing.\nSONNET: is powerful but pricey; good for demo.")]
    public ManagerModels activeAiModel = ManagerModels.ClaudeHaiku;

    [Space]
    [Header("--- USER SETTINGS ---")]
    public QuestionDifficulties questionDifficulty = QuestionDifficulties.Basic;
    public int questionAmount = 5;
    public string userName = "Bayron";
    public string questionLanguage = "C#";
    [Space]
    [Header("--- AI SETTINGS ---")]
    public string interviewerName = "James";
    public bool useVoiceGeneration = false;
    public bool streamResponseWordByWord = true;
    [Tooltip("Number of past images to send to AI for viewing. Speeds up responses and lowers cost to stay below 5")]
    public int maxSentImages = 3;
    [Space]
    //convert AI messages into a saveable array
    

    [SerializeField]
    private float _modelTemperature = 0.5f;
    public float ModelTemperature
    {
        get => _modelTemperature;
        set
        {
            _modelTemperature = Mathf.Clamp01(value);
        }
    }

    [SerializeField]
    private int _tokenLimit = 200;
    public int TokenLimit
    {
        get => _tokenLimit;
        set
        {
            _tokenLimit = (int)Mathf.Clamp(value,0,1000);
        }
    }

    [Header("--- CONNECTIONS ---")]
    private string _response = "";
    public TMP_Text responseBox = null;
    public TMP_InputField inputBox = null;
    public AudioSource audioSource; 
    public Camera visionCamera;


    [TextArea(5,50)]
    public string systemMessage = "";

    private Anthropic anthropic;

    void Start() {
        Debug.Assert(responseBox && inputBox, "Ai Manager REQUIRES an input field and response textbox! InputBox is a TMPInputField and responseBox is TMP_Text.");
        Debug.Assert(visionCamera, "No whiteboard camera detected. No whiteboard shots can be made. Set up a camera and put CameraCapture on it. Make sure it can see the whole whiteboard.");

        ReadPromptVariables();
        // add ANTHROPIC_API_KEY to environment variables and insert your key. You can make an account at https://console.anthropic.com/dashboard and generate a key.
        string anthropicKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
        Debug.Assert(anthropicKey != null || anthropicKey != "", 
            "No Anthropic API key detected in environment variables. Go to system environment variables, add 'ANTHROPIC_API_KEY', and insert your key from Anthropic.com!");
        
        anthropic = new Anthropic()
        {
            ApiKey = anthropicKey
        };

        ListAvailableVoices();
    }

    void Awake()
    {
        //TODO: using these variables to set up interview parameters.
        userName = PlayerPrefs.GetString("UserName");
        //interviewerName = PlayerPrefs.GetString("InterviewerName");
        //questionAmount = PlayerPrefs.GetInt("QuestionAmount");


        if (systemMessage == "")
        {
            systemMessage = 
                $"Your name is {interviewerName}.\n" +
                $"You are a recruiter working for BigTech Company.\n" +
                $"Your goal is to determine if {userName} is a good fit for your company.\n" +
                $"Get to know {userName} as if you would be working with them.\n" +
                $"You are interviewing {userName} for a developer role at your company.\n" +
                $"You job is to ask {userName} {questionDifficulty.ToString()} coding/developer questions during an interview scenario.\n" +
                $"Do not ask {userName} if you can be of assistance or use any default AI interactions.\n" +
                $"Direct the conversation in the manner of an interview process.\n" +
                $"Keep the questions basic and to the point. Keep coding questions in pseudocode format.\n" +
                $"The languages you can ask {questionDifficulty.ToString()} questions in are {questionLanguage}.\n" +
                $"If {userName} is incorrect, notify and correct them.\n" +
                $"Make sure to mix in general interview questions as well as coding questions.\n" +
                $"Lead the conversation and keep the interaction in an interview format.\n" +
                $"IMPORTANT: End the interview early if {userName} tries to get you off topic!" +
                $"Keep the interview at {questionAmount} questions!" +
                $"Be concise!";
        }
    }


    private void ReadPromptVariables()
    {
        ModelTemperature = _modelTemperature;
        TokenLimit = _tokenLimit;
    }

    public void ClearResponseBox() {
        _response = "";
        responseBox.text = "";
    }
    public void ClearInputBox()
    {
        inputBox.text = "";
    }


    public void GenerateAiResponse(string inputPrompt)
    {
        ClearResponseBox();
        GetClaudeResponse(inputPrompt, null, streamResponseWordByWord);
        
    }

    /// <summary>
    /// Call this function when sending an image to the AI
    /// </summary>
    public void GenerateAiResponseWithImage()
    {

        if (visionCamera)
        {
            string imagePath = visionCamera.GetComponent<CameraCapture>().Capture();
            GetClaudeResponse(inputBox.text, imagePath, streamResponseWordByWord);
        }
        else
        {
            Debug.LogError("NO WHITEBOARD CAMERA FOUND!");
            GetClaudeResponse(inputBox.text, null, streamResponseWordByWord);
        }

        ClearResponseBox();
    }


    


    private async void GetClaudeResponse(string inputMessage = null, string imageBytes = null, bool streamResponse = false)
    {
        ReadPromptVariables();
        if (responseBox && inputBox)
        {
            Debug.Log("Sending message to Claude...");

            string userMessage = inputMessage;
            //Debug.Log(systemMessage);
            //Debug.Log("User: " + userMessage);

            // Determine if its text only | image + text | image | or nothing for a continuation
            Claudia.Message[] sentMessage;

            // append the created message blocks to the sentMessage array

            if (inputMessage != null && imageBytes == null)
            {
                sentMessage = CreateClaudiaTextBlock(inputMessage);
            }
            else if (imageBytes != null)
            {
                sentMessage = BuildClaudiaMessage(inputMessage ?? "", imageBytes);
            }
            else
            {
                sentMessage = PromptClaudeContinuation();
            }


            if (streamResponse)
            {
                var stream = anthropic.Messages.CreateStreamAsync(new()
                {
                    Model = claudeModel[(int)activeAiModel],
                    MaxTokens = TokenLimit,
                    Temperature = ModelTemperature,
                    System = systemMessage,
                    Messages = sentMessage

                });

                await foreach (var messageStreamEvent in stream)
                {
                    if (messageStreamEvent is ContentBlockDelta content)
                    {
                        if (messageStreamEvent != null)
                            StreamMessageToBox(content.Delta.Text, responseBox);
                    }
                }
            }
            else
            {
                
                var message = await anthropic.Messages.CreateAsync(new()
                {
                    Model = claudeModel[(int)activeAiModel],
                    MaxTokens = TokenLimit,
                    Temperature = ModelTemperature,
                    System = systemMessage,
                    Messages = sentMessage
                });

                StreamMessageToBox(message.ToString(), responseBox);

            }

            if (useVoiceGeneration)
            {
                GenerateElevenLabsVoice(_response);
            }
        }
    }

    private void StreamMessageToBox(string message, TMP_Text responseTextBox)
    {
        _response += message;
        responseTextBox.text = _response;
    }

    private Claudia.Message[] CreateClaudiaTextBlock(string text)
    {
        return new Claudia.Message[] { new() {
            Role = Roles.User, 
            Content = text
        }};
    }

    private Claudia.Message[] PromptClaudeContinuation()
    {
        return new Claudia.Message[] { new() {
            Role = Roles.Assistant,
            Content = responseBox.text
        }};
    }

    private Claudia.Message[] BuildClaudiaMessage(string text, string imagePath)
    {
        var imageBytes = File.ReadAllBytes(imagePath);

        Contents contentBlocks = new() { };

        if (imagePath != null && imagePath != "") 
        {
            contentBlocks.Add(
                new Content
                {
                    Type = "image",
                    Source = new Source
                    {
                        Type = "base64",
                        MediaType = "image/jpeg",
                        Data = imageBytes
                    }
                }
            );
        }

        if (text != null && text != "")
        {
            contentBlocks.Add(
                new Content
                {
                    Type = "text",
                    Text = text
                }
            );
        }

        Claudia.Message[] builtMessage = new Claudia.Message[] { new() {
            Role = Roles.User,
            Content = contentBlocks
        }};

        return builtMessage;
    }

    private async void ListAvailableVoices()
    {
        var elevenLabsAPIKey = new ElevenLabsClient(new ElevenLabsAuthentication().LoadFromEnvironment());

        var results = await elevenLabsAPIKey.SharedVoicesEndpoint.GetSharedVoicesAsync();
        foreach (var foundVoice in results.Voices)
        {
            Debug.Log($"{foundVoice.OwnerId} | {foundVoice.VoiceId} | {foundVoice.Date} | {foundVoice.Name}");
        }
    }

    private string TrimFlavorText(string textInput)
    {
        // get rid of text that is in * * like *sad*
        textInput = System.Text.RegularExpressions.Regex.Replace(textInput, @"\*.*?\*", "");
        return textInput;
    }

    public async void GenerateElevenLabsVoice(string textInput)
    {
        // add ELEVEN_LABS_API_KEY to environment variables and insert your key. You can make a free account at https://elevenlabs.io/ and generate a key.
        var elevenLabsAPIKey = new ElevenLabsClient(new ElevenLabsAuthentication().LoadFromEnvironment());
        var text = TrimFlavorText(textInput);

        var voice = (await elevenLabsAPIKey.VoicesEndpoint.GetAllVoicesAsync())[1];

        //var voice = await elevenLabsAPIKey.VoicesEndpoint.GetVoiceAsync("86ZLAUcyPNBrbdJKn3u6");
        //var voice = (await elevenLabsAPIKey.VoicesEndpoint.GetVoiceAsync("Chris"));
        var request = new TextToSpeechRequest(voice, text);
        var voiceClip = await elevenLabsAPIKey.TextToSpeechEndpoint.TextToSpeechAsync(request);
        audioSource.PlayOneShot(voiceClip.AudioClip);
    }
}
