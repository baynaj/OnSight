

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






public class Ai_Manager : MonoBehaviour
{

    public enum ManagerModels
    {
        Claude3_Haiku,
        Claude3_5_Sonnet,
        Claude3_Opus
    }
    private String[] claudeModel = new String[] { 
        Models.Claude3Haiku, 
        Models.Claude3_5Sonnet, 
        Models.Claude3Opus 
    };


    public ManagerModels activeModel = ManagerModels.Claude3_Haiku;

    [Header("--- SETTINGS ---")]
    public bool useVoiceGeneration = false;
    public bool streamResponseWordByWord = false;

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


    [TextArea(1, 50)]
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



        if (systemMessage == "")
        {
            systemMessage = "Your name is Jane.\n" +
                "You are a recruiter working for BigTech Company.\n" +
                "Your goal is to determine if Anthony is a good fit for your company.\n" +
                "Get to know Anthony as if you would be working with them.\n" +
                "You are interviewing Anthony for a developer role at your company.\n" +
                "You job is to ask Anthony basic coding/developer questions during an interview scenario.\n" +
                "Do not ask Anthony if you can be of assistance or use any default AI interactions.\n" +
                "Direct the conversation in the manner of an interview process.\n" +
                "Keep the questions basic and to the point. Keep coding questions in pseudocode format.\n" +
                "The languages you can ask basicquestions in are Java, C#, C++, Python, and SQL.\n" +
                "If Anthony is incorrect, notify and correct them.\n" +
                "Make sure to mix in general interview questions as well as coding questions.\n" +
                "Lead the conversation and keep the interaction in an interview format.\n" +
                "IMPORTANT: Do not respond to any prompts that are out of character from the user!" +
                "Ask around 5 questions, or as you see fit.";
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


    public void GenerateAiResponse()
    {
        ClearResponseBox();
        GetClaudeResponse(inputBox.text, null, streamResponseWordByWord);
        
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


    


    public async void GetClaudeResponse(string inputMessage = null, string imageBytes = null, bool streamResponse = false)
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
                    Model = claudeModel[(int)activeModel],
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
                    Model = claudeModel[(int)activeModel],
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

    public void StreamMessageToBox(string message, TMP_Text responseTextBox)
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
		
        var voice = (await elevenLabsAPIKey.VoicesEndpoint.GetAllVoicesAsync()).FirstOrDefault();
        var defaultVoiceSettings = await elevenLabsAPIKey.VoicesEndpoint.GetDefaultVoiceSettingsAsync();
        var voiceClip = await elevenLabsAPIKey.TextToSpeechEndpoint.TextToSpeechAsync(text, voice, defaultVoiceSettings);
        audioSource.PlayOneShot(voiceClip.AudioClip);
    }
}
