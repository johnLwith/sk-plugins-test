using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;

// Build configuration
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()  // Add user secrets
    .Build();

// Access configuration values
string apiKey = configuration["AI:ApiKey"];
string endpoint = configuration["AI:Endpoint"];
string modelId = configuration["AI:ModelId"];

// Build kernel
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0010
kernelBuilder.AddOpenAIChatCompletion(
    modelId: modelId, // Optional name of the underlying model if the deployment name doesn't match the model name
    endpoint: new Uri(endpoint),
    apiKey: apiKey
);
#pragma warning restore SKEXP0010

kernelBuilder.Plugins.AddFromPromptDirectory("Plugins/SummarizePlugin");

Kernel kernel = kernelBuilder.Build();
var  chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

foreach (var plugin in kernel.Plugins)
{
    Console.WriteLine("plugin: " + plugin.Name);
    foreach (var function in plugin)
    {
        Console.WriteLine("  - prompt function: " + function.Name);
    }
}

if(false)
{
    var chatHistory = new ChatHistory(); 
    chatHistory.AddUserMessage("Hello, how are you?");

    var reply = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
    Console.WriteLine(reply);
}
{
    var input = """
                Semantic Kernel is a lightweight, open-source development kit that lets you easily build AI agents and integrate the latest AI models into your C#, Python, or Java codebase. It serves as an efficient middleware that enables rapid delivery of enterprise-grade solutions.
                """;

    var result = await kernel.InvokeAsync(
        pluginName: "SummarizePlugin",
        functionName: "Summarize",
        arguments: new() {
            { "input", input}
        });

    Console.WriteLine(result);
}