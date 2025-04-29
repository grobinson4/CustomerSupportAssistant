namespace CustomerSupportAssistant.Domain.Dtos
{
   public class InquiryAnalysisResult
{
    public List<RootCause> RootCauses { get; set; } = new();
    public List<string> AzureCommands { get; set; } = new();
    public string Eli5Explanation { get; set; } = string.Empty;
}

public class RootCause
{
    public string Hypothesis { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public List<string> Actions { get; set; } = new();
    public List<Documentation> Docs { get; set; } = new();
}

public class Documentation
{
    public string Title { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}
}
