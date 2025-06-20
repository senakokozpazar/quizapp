using System;
using System.Collections.Generic;

namespace quizapp.data.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public string? Text { get; set; }

    public int? QuizId { get; set; }

    public virtual ICollection<Answer> Answers { get; } = new List<Answer>();

    public virtual Quiz? Quiz { get; set; }
}
