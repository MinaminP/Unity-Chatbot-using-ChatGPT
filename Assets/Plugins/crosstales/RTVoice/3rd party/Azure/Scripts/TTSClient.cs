#if NET_4_6 || NET_STANDARD_2_0
//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
//
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/Cognitive-Speech-TTS
//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// IMPORTANT: THIS CODE ONLY WORKS WITH THE .NET 4.6 SCRIPTING RUNTIME

// Note that Unity 2017.x doesn't recognize the namespace System.Net.Http by default.
// This is why we added mcs.rsp with "-r:System.Net.Http.dll" in it in the Assets folder.
using UnityEngine;

namespace Crosstales.RTVoice.Azure
{
   /// <summary>
   /// This class demonstrates how to get a valid O-auth token
   /// </summary>
   public class Authentication
   {
      private string AccessUri;
      private string apiKey;
      private string accessToken;
      private System.Threading.Timer accessTokenRenewer;

      private readonly System.Net.Http.HttpClient client;

      //Access token expires every 10 minutes. Renew it every 9 minutes only.
      private const int RefreshTokenDuration = 9;

      public Authentication()
      {
         client = new System.Net.Http.HttpClient();
      }

      /// <summary>
      /// The Authenticate method needs to be called separately since it runs asynchronously
      /// and cannot be in the constructor, nor should it block the main Unity thread.
      /// </summary>
      /// <param name="issueTokenUri"></param>
      /// <param name="key"></param>
      /// <returns>Authentication task</returns>
      public async System.Threading.Tasks.Task<string> Authenticate(string issueTokenUri, string key)
      {
         AccessUri = issueTokenUri;
         apiKey = key;

         accessToken = await HttpClientPost(issueTokenUri, key);

         // Renew the token every specified minutes
         accessTokenRenewer = new System.Threading.Timer(OnTokenExpiredCallback,
            this,
            System.TimeSpan.FromMinutes(RefreshTokenDuration),
            System.TimeSpan.FromMilliseconds(-1));

         return accessToken;
      }

      public string GetAccessToken()
      {
         return accessToken;
      }

      private async void RenewAccessToken()
      {
         string newAccessToken = await HttpClientPost(AccessUri, apiKey);
         //swap the new token with old one
         //Note: the swap is thread unsafe
         accessToken = newAccessToken;

         if (Util.Config.DEBUG)
            Debug.Log("Renewed token for user (" + apiKey + "): " + accessToken);
      }

      private void OnTokenExpiredCallback(object stateInfo)
      {
         try
         {
            RenewAccessToken();
         }
         catch (System.Exception ex)
         {
            Debug.Log("Failed renewing access token: " + ex);
         }
         finally
         {
            try
            {
               accessTokenRenewer.Change(System.TimeSpan.FromMinutes(RefreshTokenDuration), System.TimeSpan.FromMilliseconds(-1));
            }
            catch (System.Exception ex)
            {
               Debug.Log("Failed to reschedule the timer to renew access token: " + ex);
            }
         }
      }

      /// <summary>
      /// Asynchronously calls the authentication service via HTTP POST to obtain
      /// </summary>
      /// <param name="accessUri"></param>
      /// <param name="key"></param>
      /// <returns></returns>
      private async System.Threading.Tasks.Task<string> HttpClientPost(string accessUri, string key)
      {
         System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, accessUri);
         request.Headers.Add("Ocp-Apim-Subscription-Key", key);
         request.Content = new System.Net.Http.StringContent("");

         System.Net.Http.HttpResponseMessage httpMsg = await client.SendAsync(request);

         if (Util.Config.DEBUG)
            Debug.Log("Authentication Response status code: " + httpMsg.StatusCode);

         return await httpMsg.Content.ReadAsStringAsync();
      }
   }

   /// <summary>
   /// Generic event args
   /// </summary>
   /// <typeparam name="T">Any type T</typeparam>
   public class GenericEventArgs<T> : System.EventArgs
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="GenericEventArgs{T}" /> class.
      /// </summary>
      /// <param name="eventData">The event data.</param>
      public GenericEventArgs(T eventData)
      {
         EventData = eventData;
      }

      /// <summary>
      /// Gets the event data.
      /// </summary>
      public T EventData { get; private set; }
   }

   /// <summary>
   /// Voice output formats.
   /// </summary>
   public enum AudioOutputFormat
   {
      /// <summary>
      /// raw-8khz-8bit-mono-mulaw request output audio format type.
      /// </summary>
      Raw8Khz8BitMonoMULaw,

      /// <summary>
      /// raw-16khz-16bit-mono-pcm request output audio format type.
      /// </summary>
      Raw16Khz16BitMonoPcm,

      /// <summary>
      /// riff-8khz-8bit-mono-mulaw request output audio format type.
      /// </summary>
      Riff8Khz8BitMonoMULaw,

      /// <summary>
      /// riff-16khz-16bit-mono-pcm request output audio format type.
      /// </summary>
      Riff16Khz16BitMonoPcm,

      /// <summary>
      /// ssml-16khz-16bit-mono-silk request output audio format type.
      /// It is a SSML with audio segment, with audio compressed by SILK codec
      /// </summary>
      Ssml16Khz16BitMonoSilk,

      /// <summary>
      /// raw-16khz-16bit-mono-truesilk request output audio format type.
      /// Audio compressed by SILK codec
      /// </summary>
      Raw16Khz16BitMonoTrueSilk,

      /// <summary>
      /// ssml-16khz-16bit-mono-tts request output audio format type.
      /// It is a SSML with audio segment, and it needs tts engine to play out
      /// </summary>
      Ssml16Khz16BitMonoTts,

      /// <summary>
      /// audio-16khz-128kbitrate-mono-mp3 request output audio format type.
      /// </summary>
      Audio16Khz128KBitRateMonoMp3,

      /// <summary>
      /// audio-16khz-64kbitrate-mono-mp3 request output audio format type.
      /// </summary>
      Audio16Khz64KBitRateMonoMp3,

      /// <summary>
      /// audio-16khz-32kbitrate-mono-mp3 request output audio format type.
      /// </summary>
      Audio16Khz32KBitRateMonoMp3,

      /// <summary>
      /// audio-16khz-16kbps-mono-siren request output audio format type.
      /// </summary>
      Audio16Khz16KbpsMonoSiren,

      /// <summary>
      /// riff-16khz-16kbps-mono-siren request output audio format type.
      /// </summary>
      Riff16Khz16KbpsMonoSiren,

      /// <summary>
      /// raw-24khz-16bit-mono-truesilk request output audio format type.
      /// </summary>
      Raw24Khz16BitMonoTrueSilk,

      /// <summary>
      /// raw-24khz-16bit-mono-pcm request output audio format type.
      /// </summary>
      Raw24Khz16BitMonoPcm,

      /// <summary>
      /// riff-24khz-16bit-mono-pcm request output audio format type.
      /// </summary>
      Riff24Khz16BitMonoPcm,

      /// <summary>
      /// audio-24khz-48kbitrate-mono-mp3 request output audio format type.
      /// </summary>
      Audio24Khz48KBitRateMonoMp3,

      /// <summary>
      /// audio-24khz-96kbitrate-mono-mp3 request output audio format type.
      /// </summary>
      Audio24Khz96KBitRateMonoMp3,

      /// <summary>
      /// audio-24khz-160kbitrate-mono-mp3 request output audio format type.
      /// </summary>
      Audio24Khz160KBitRateMonoMp3
   }


   /// <summary>
   /// Sample synthesize request
   /// </summary>
   public class Synthesize
   {
      /// <summary>
      /// Generates SSML.
      /// </summary>
      /// <param name="locale">The locale.</param>
      /// <param name="gender">The gender.</param>
      /// <param name="voicename">The voice name.</param>
      /// <param name="text">The text input.</param>
      private static string generateSSML(string locale, string gender, string voicename, string text)
      {
         System.Text.StringBuilder sbXML = new System.Text.StringBuilder();
         //SSML
         sbXML.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
         sbXML.Append("<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" ");

         if (text.CTContains("<mstts:"))
            sbXML.Append("xmlns:mstts=\"https://www.w3.org/2001/mstts\" ");

         sbXML.Append("xml:lang=\"");
         sbXML.Append(locale);
         sbXML.Append("\"");
         sbXML.Append(">");

         if (text.CTContains("<voice "))
         {
            sbXML.Append(text);
         }
         else
         {
            sbXML.Append("<voice ");
            sbXML.Append("xml:lang=\"");
            sbXML.Append(locale);
            sbXML.Append("\" ");
            sbXML.Append("xml:gender=\"");
            sbXML.Append(gender);
            sbXML.Append("\" ");
            sbXML.Append("name=\"");
            sbXML.Append(voicename);
            sbXML.Append("\"");
            sbXML.Append(">");
            sbXML.Append(text);
            sbXML.Append("</voice>");
         }

         sbXML.Append("</speak>");

         return sbXML.ToString();
      }

      private readonly System.Net.Http.HttpClient client;
      private readonly System.Net.Http.HttpClientHandler handler;

      /// <summary>
      /// Initializes a new instance of the <see cref="Synthesize"/> class.
      /// </summary>
      public Synthesize()
      {
         //var cookieContainer = new CookieContainer();
         handler = new System.Net.Http.HttpClientHandler { CookieContainer = new System.Net.CookieContainer(), UseProxy = false };
         client = new System.Net.Http.HttpClient(handler);
      }

      ~Synthesize()
      {
         client.Dispose();
         handler.Dispose();
      }

      /// <summary>
      /// Sends the specified text to be spoken to the TTS service and saves the response audio to a file.
      /// </summary>
      /// <param name="cancellationToken">The cancellation token.</param>
      /// <param name="inputOptions">Input options for the speech.</param>
      /// <returns>A Task</returns>
      public async System.Threading.Tasks.Task<System.IO.Stream> Speak(System.Threading.CancellationToken cancellationToken, InputOptions inputOptions)
      {
         client.DefaultRequestHeaders.Clear();
         foreach (System.Collections.Generic.KeyValuePair<string, string> header in inputOptions.Headers)
         {
            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
         }

         string genderValue;
         switch (inputOptions.VoiceType)
         {
            case Model.Enum.Gender.MALE:
               genderValue = "Male";
               break;

            default:
               genderValue = "Female";
               break;
         }

         System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, inputOptions.RequestUri)
         {
            Content = new System.Net.Http.StringContent(generateSSML(inputOptions.Locale, genderValue, inputOptions.VoiceName, inputOptions.Text))
         };

         System.Net.Http.HttpResponseMessage httpMsg = await client.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseContentRead, cancellationToken);

         if (Util.Config.DEBUG)
            Debug.Log("Response status code: " + httpMsg.StatusCode);

         System.IO.Stream httpStream = await httpMsg.Content.ReadAsStreamAsync();

         return httpStream;
      }

      /// <summary>
      /// Inputs Options for the TTS Service.
      /// </summary>
      public class InputOptions
      {
         /// <summary>
         /// Initializes a new instance of the <see cref="Input"/> class.
         /// </summary>
         public InputOptions()
         {
            Locale = "en-us";
            VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, JessaRUS)";
            // Default to Riff24Khz16BitMonoPcm output format.
            OutputFormat = AudioOutputFormat.Riff24Khz16BitMonoPcm;
         }

         /// <summary>
         /// Gets or sets the request URI.
         /// </summary>
         public System.Uri RequestUri { get; set; }

         /// <summary>
         /// Gets or sets the audio output format.
         /// </summary>
         public AudioOutputFormat OutputFormat { get; set; }

         /// <summary>
         /// Gets or sets the headers.
         /// </summary>
         public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> Headers
         {
            get
            {
               System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>> toReturn = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>> { new System.Collections.Generic.KeyValuePair<string, string>("Content-Type", "application/ssml+xml") };

               string outputFormat;

               switch (OutputFormat)
               {
                  case AudioOutputFormat.Raw16Khz16BitMonoPcm:
                     outputFormat = "raw-16khz-16bit-mono-pcm";
                     break;

                  case AudioOutputFormat.Raw8Khz8BitMonoMULaw:
                     outputFormat = "raw-8khz-8bit-mono-mulaw";
                     break;

                  case AudioOutputFormat.Riff16Khz16BitMonoPcm:
                     outputFormat = "riff-16khz-16bit-mono-pcm";
                     break;

                  case AudioOutputFormat.Riff8Khz8BitMonoMULaw:
                     outputFormat = "riff-8khz-8bit-mono-mulaw";
                     break;

                  case AudioOutputFormat.Ssml16Khz16BitMonoSilk:
                     outputFormat = "ssml-16khz-16bit-mono-silk";
                     break;

                  case AudioOutputFormat.Raw16Khz16BitMonoTrueSilk:
                     outputFormat = "raw-16khz-16bit-mono-truesilk";
                     break;

                  case AudioOutputFormat.Ssml16Khz16BitMonoTts:
                     outputFormat = "ssml-16khz-16bit-mono-tts";
                     break;

                  case AudioOutputFormat.Audio16Khz128KBitRateMonoMp3:
                     outputFormat = "audio-16khz-128kbitrate-mono-mp3";
                     break;

                  case AudioOutputFormat.Audio16Khz64KBitRateMonoMp3:
                     outputFormat = "audio-16khz-64kbitrate-mono-mp3";
                     break;

                  case AudioOutputFormat.Audio16Khz32KBitRateMonoMp3:
                     outputFormat = "audio-16khz-32kbitrate-mono-mp3";
                     break;

                  case AudioOutputFormat.Audio16Khz16KbpsMonoSiren:
                     outputFormat = "audio-16khz-16kbps-mono-siren";
                     break;

                  case AudioOutputFormat.Riff16Khz16KbpsMonoSiren:
                     outputFormat = "riff-16khz-16kbps-mono-siren";
                     break;
                  case AudioOutputFormat.Raw24Khz16BitMonoPcm:
                     outputFormat = "raw-24khz-16bit-mono-pcm";
                     break;
                  case AudioOutputFormat.Riff24Khz16BitMonoPcm:
                     outputFormat = "riff-24khz-16bit-mono-pcm";
                     break;
                  case AudioOutputFormat.Audio24Khz48KBitRateMonoMp3:
                     outputFormat = "audio-24khz-48kbitrate-mono-mp3";
                     break;
                  case AudioOutputFormat.Audio24Khz96KBitRateMonoMp3:
                     outputFormat = "audio-24khz-96kbitrate-mono-mp3";
                     break;
                  case AudioOutputFormat.Audio24Khz160KBitRateMonoMp3:
                     outputFormat = "audio-24khz-160kbitrate-mono-mp3";
                     break;
                  default:
                     outputFormat = "riff-16khz-16bit-mono-pcm";
                     break;
               }

               toReturn.Add(new System.Collections.Generic.KeyValuePair<string, string>("X-Microsoft-OutputFormat", outputFormat));
               // authorization Header
               toReturn.Add(new System.Collections.Generic.KeyValuePair<string, string>("Authorization", AuthorizationToken));
               // Refer to the doc
               toReturn.Add(new System.Collections.Generic.KeyValuePair<string, string>("X-Search-AppId", "07D3234E49CE426DAA29772419F436CA"));
               // Refer to the doc
               toReturn.Add(new System.Collections.Generic.KeyValuePair<string, string>("X-Search-ClientID", "1ECFAE91408841A480F00935DC390960"));
               // The software originating the request
               toReturn.Add(new System.Collections.Generic.KeyValuePair<string, string>("User-Agent", "UnityTTSClient"));

               return toReturn;
            }

            /*
            private set
            {
                Headers = value;
            }
            */
         }

         /// <summary>
         /// Gets or sets the locale.
         /// </summary>
         public string Locale { get; set; }

         /// <summary>
         /// Gets or sets the type of the voice; male/female.
         /// </summary>
         public Model.Enum.Gender VoiceType { get; set; }

         /// <summary>
         /// Gets or sets the name of the voice.
         /// </summary>
         public string VoiceName { get; set; }

         /// <summary>
         /// Authorization Token.
         /// </summary>
         public string AuthorizationToken { get; set; }

         /// <summary>
         /// Gets or sets the text.
         /// </summary>
         public string Text { get; set; }
      }
   }
}
#endif