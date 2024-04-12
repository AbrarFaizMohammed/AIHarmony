using AIHarmony.data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Mscc.GenerativeAI;
using OpenAI_API;
using HeyRed.MarkdownSharp;




namespace AIHarmony.Hubs
{
    public class AIConnect:Hub
    {
        private static Queue<UserQuestion> clientsDetails = new Queue<UserQuestion>();

        private readonly IConfiguration _configuration;
        private readonly Applicationdbcontext _db;

        public AIConnect(IConfiguration configuration, Applicationdbcontext db)
        {
            _configuration = configuration;
            _db = db;  
        }

        Markdown mark = new Markdown();

        public async Task SendMessage(string message, String AIName, String userInitial)
        {
            if ((IsUserPromptIsValid(message, userInitial))){
                await Clients.Caller.SendAsync("ReceiveerrorMessage", "Remove confidential information from your prompt");
                return;
            }

            clientsDetails.Enqueue(new UserQuestion {Question = message, AIName = AIName, ConnectionId = Context.ConnectionId });
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessagetocompareAI(string message, String AIName1, String AIName2, String userInitial)
        {
            if ((IsUserPromptIsValid(message, userInitial)))
            {
                await Clients.Caller.SendAsync("ReceiveerrorMessage", "Remove confidential information from your prompt");
                return;
            }

            clientsDetails.Enqueue(new UserQuestion { Question = message, AIName = $"{AIName1},{AIName2}", ConnectionId = Context.ConnectionId });
            await Clients.Caller.SendAsync("ReceiveMessage", message,  AIName1,  AIName2);
        }
        public async Task ReplyFromServer()
        {
            while(clientsDetails.Count > 0)
            {
                var userQueryDetails = clientsDetails.Dequeue();
                string[] aiNames = userQueryDetails.AIName.Split(',');
                var result = "";
                foreach (var aiName in aiNames)
                {
                    switch (aiName)
                    {
                        case "OpenAi":
                            result = await ConnectToOpenAi(userQueryDetails.Question);
                            break;
                        case "Gemini":
                            result = connectToGeminiAI(userQueryDetails.Question);
                            break;
                        case "Copilot":
                            result = connectToGeminiAI(userQueryDetails.Question);
                            break;
                    }
                    await Clients.Client(userQueryDetails.ConnectionId).SendAsync("ReceiveMessageFromServer", result, aiName);
                    result = "";
                }
            }
        }

       

        private string connectToGeminiAI(string userPrompt)
        {
            string apiKey = Environment.GetEnvironmentVariable("Gemini_Key");

            var model = new GenerativeModel(apiKey: apiKey, model: Mscc.GenerativeAI.Model.GeminiPro);
            var response = model.GenerateContent(userPrompt).Result;
            var result = response.Text;
            return mark.Transform(result);
        }

        private async Task<string> ConnectToOpenAi(string userPrompt)
        {
            string response = "";
            try
            {
                var Key = Environment.GetEnvironmentVariable("OpenAI_KEY");
                OpenAIAPI api = new OpenAIAPI(Key);
                var chat = api.Chat.CreateConversation();
                chat.AppendUserInput(userPrompt);

                // Await the response stream from the chatbot
                await chat.StreamResponseFromChatbotAsync(res =>
                {
                    response += res+" ";
                });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return mark.Transform(response);
        }


        private bool IsUserPromptIsValid(string userPrompt, String userDetail)
        {
            if (string.IsNullOrEmpty(userPrompt) || string.IsNullOrEmpty(userDetail))
            {
                return false; 
            }
           
            var wordsToCheck = _db.ConfidentialWords
                                  .Where(x => x.Users != null &&
                                              x.UserId ==Guid.Parse(userDetail))
                                  .Select(x => x.Word)
                                  .ToList();

            return wordsToCheck.Any(word => userPrompt.ToLower().Contains(word.ToLower()));
        }

    }
    }
