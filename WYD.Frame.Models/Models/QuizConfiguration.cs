namespace WYD.Frame.Models.Models;

public class QuizConfiguration
{
    public List<QuizQuestionResponse> QuestionResponses { get; set; } = new();
}

public class QuizQuestionResponse
{
    public string Id { get; set; }
    public string Question { get; set; }
    public string? Answer { get; set; }
}