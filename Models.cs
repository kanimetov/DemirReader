
public class Data
{
    public Method Method { get; set; }
    public Process Process { get; set; }
    public string Layer { get; set; }
    public Creation Creation { get; set; }
    public string Type { get; set; }
}

public class Method
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Assembly { get; set; }
}

public class Process
{
    public string Name { get; set; }
    public int Id { get; set; }
    public Start Start { get; set; }
}

public class Start
{
    public long Epoch { get; set; }
    public DateTime Date { get; set; }
}

public class Creation
{
    public long Epoch { get; set; }
    public DateTime Date { get; set; }
}
