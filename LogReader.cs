using System.Globalization;
using System.Text.RegularExpressions;
public class LogReader
{
    private readonly string _logsFolderPath;
    private readonly string _outputDirectory;
    private const int MAX_ITEM_PER_FILE = 10;
    private readonly StateManager _stateManager;

    public LogReader(string logsFolderPath, string outputDirectory)
    {
        _logsFolderPath = logsFolderPath;
        _outputDirectory = outputDirectory;
        _stateManager = new StateManager(_logsFolderPath);
    }
    

    public void ProcessLogFile()
    {
        var states = _stateManager.GetStates();
        for (int i = 0; i < states.Length; i++)
        {
                var newState = WriteToBatchFile(states[i]);
                states[i] = newState;
        }

        _stateManager.SaveStates(states);
    }

    private State WriteToBatchFile(State state, int batchSize = 10)
    {
         try
        {
            
            string filePath = Path.Combine(_logsFolderPath, state.ResourceFileName);
            string batchFile = Path.Combine(_outputDirectory, state.FileName);
            var tempFilePath = $"{batchFile}.tmp";

            string fileContent = File.ReadAllText(filePath).Trim();
            var jsonObjects = fileContent.Split(["\n"], StringSplitOptions.None);

            if (jsonObjects.Length <= state.Count + 1) return state;

            var isFilled = false;
            var isModified = false;
            using var writer = new StreamWriter(tempFilePath);
            for(int i = state.TotalCount + 1; i < jsonObjects.Length; i++)
            {
                if(state.Count >= MAX_ITEM_PER_FILE) {
                    state.FileName = IncrementBatchFile(state.FileName);
                    state.Count = 0;
                    isFilled = true;
                    break;
                }
                 writer.WriteLine(jsonObjects[i]);
                 state.Count++;
                 state.TotalCount++;
                 isModified = true;
            }
            writer.Close();


            if(isModified){
                File.Delete(batchFile);
                File.Move(tempFilePath, batchFile);
            }

            if(isFilled){
                WriteToBatchFile(state);
            }

            return state;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private string IncrementBatchFile(string fileName)
    {
        var newFileName = Regex.Replace(fileName, @"(\d{4})(?=\.log$)", match => {
            int number = int.Parse(match.Value);
            number++;
            
            return number.ToString("D4");
        });

        return newFileName;
    }
}



public class State {
    public string Type { get; set; }
    public string FileName { get; set; }
    public string ResourceFileName { get; set; }
    public int Count { get; set; }
    public int TotalCount { get; set; }
    public DateTime Date { get; set; }
}

class Log {
    public string Type { get; set; }
    public string FileName { get; set; }
    public DateTime Date { get; set; }
}