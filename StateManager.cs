using System.Text.Json;
using System.Text.RegularExpressions;

public class StateManager {
    private readonly string _stateFilePath;
    private readonly string _logsFilePath;
    public StateManager(string logsFilePath)
    {
        _stateFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Demir", "state.txt");
        _logsFilePath = logsFilePath;
    }

    public State[] GetStates(){
        if(!File.Exists(_stateFilePath)) return NormalizeStates(null);

        string stateContent = File.ReadAllText(_stateFilePath);
        if(string.IsNullOrEmpty(stateContent)) return NormalizeStates(null);

        var states = JsonSerializer.Deserialize<State[]>(stateContent);
        return NormalizeStates(states);
    }
    public void SaveStates(State[] states)
    {
        states = states.OrderBy(s => s.Type).ToArray();
        string stateContent = JsonSerializer.Serialize(states);
        File.WriteAllText(_stateFilePath, stateContent);
    }
    private State[] NormalizeStates(State[]? states)
    {
        var logFiles = Directory.GetFiles(_logsFilePath, "*.log");
        List<Log> logs = [];
        foreach(var log in logFiles)
        {
            string fileName = Path.GetFileName(log);
            var match = Regex.Match(fileName, @"^([a-zA-Z]+)-(\d{4}-\d{2}-\d{2})\.log$");
            if(match.Success)
            {
                logs.Add(new Log {
                    Type = match.Groups[1].Value,
                    FileName = fileName,
                    Date = DateTime.Parse(match.Groups[2].Value)
                });
            }
        }

        logs = [.. logs.OrderBy(l => l.Type)];
        
        if(states == null || states.Length == 0)
        {
            return logs.Select(l => new State {
                Type = l.Type,
                FileName = $"{l.Type}-{l.Date:yyyy-MM-dd}-0001.log",
                Date = l.Date,
                ResourceFileName = l.FileName
            }).ToArray();
        }

        states = states.OrderBy(s => s.Type).ToArray();
        logs = logs.Where(l => !states.Any(a => a.Date == l.Date && a.Type == l.Type)).ToList();

        foreach(var log in logs)
        {
            states = states.Concat([
                new State {
                    Type = log.Type,
                    FileName = $"{log.Type}-{log.Date:yyyy-MM-dd}-0001.log",
                    Date = log.Date,
                    ResourceFileName = log.FileName
                }
            ]).ToArray();
        }

        return states;
    }
}