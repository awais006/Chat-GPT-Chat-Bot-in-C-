using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Chat_GPT_Chat_Bot
{
    public class Program
    {
        const string key = "YOUR API KEY";
        const string url = "https://api.openai.com/v1/chat/completions";

        static void Main(string[] args)
        {

            Program program = new Program();

            program.CallChatGPTAPI();


        }


        public async void CallChatGPTAPI()
        {
            try
            {
                var messages = new List<dynamic>
                {
                    new {role = "system",
                        content = "You are ChatGPT, a large language " +
                                                    "model trained by OpenAI. " +
                                                    "Answer as concisely as possible.  " +
                                                    "Make a joke every few lines just to spice things up."},
                    new {role = "assistant",
                        content = "How can I help you?"}
                };

                while (true)
                {
                    // Capture the users messages and add to
                    // messages list for submitting to the chat API
                    Console.WriteLine("Type your Prompt: ");
                    var userMessage = Console.ReadLine();
                    messages.Add(new { role = "user", content = userMessage });

                    // Create the request for the API sending the
                    // latest collection of chat messages
                    var request = new
                    {
                        messages,
                        model = "gpt-3.5-turbo",
                        max_tokens = 300,
                    };

                    // Send the request and capture the response
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
                    var requestJson = JsonConvert.SerializeObject(request);
                    var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
                    var httpResponseMessage = await httpClient.PostAsync(url, requestContent);
                    var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeAnonymousType(jsonString, new
                    {
                        choices = new[] { new { message = new { role = string.Empty, content = string.Empty } } },
                        error = new { message = string.Empty }
                    });


                    if (!string.IsNullOrEmpty(responseObject?.error?.message))  // Check for errors
                    {
                        Console.WriteLine("Error Message ==> " + responseObject?.error.message);
                    }
                    else  // Add the message object to the message collection
                    {
                        var messageObject = responseObject?.choices[0].message;
                        messages.Add(messageObject);
                        Console.WriteLine("Reply From Chat GPT: ==> " + messageObject.content);
                    }
                }

            }

            catch (Exception exp)
            { 
                Console.WriteLine(exp.Message); 
            }

            

        }


    }
}
