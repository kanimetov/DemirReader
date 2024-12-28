string logsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Demir", "Logs");
string outputDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Demir", "Batches");

if (!Directory.Exists(outputDirectory))
{
    Directory.CreateDirectory(outputDirectory);
}

Console.WriteLine("Service started. Press [Enter] to stop.");
CancellationTokenSource cts = new CancellationTokenSource();

        // Start the service loop
var serviceTask = RunServiceAsync(cts.Token, logsFolderPath, outputDirectory);
await serviceTask;





static async Task RunServiceAsync(CancellationToken token, string logsFolderPath, string outputDirectory)
{
    const int WAIT_MINUTES = 10;
    var logReader = new LogReader(logsFolderPath, outputDirectory); 
    while (!token.IsCancellationRequested)
    {
        Console.WriteLine($"Task executed at: {DateTime.Now}");
        logReader.ProcessLogFile();

        try
        {
            await Task.Delay(TimeSpan.FromMinutes(WAIT_MINUTES), token);
        }
        catch (TaskCanceledException)
        {
                break;
        }
    }

    Console.WriteLine("Service stopped.");
}