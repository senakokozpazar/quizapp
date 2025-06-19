using System;
using System.Collections.Generic;

namespace quizapp.data.Models;

public partial class Answer
{
    public int AnswerId { get; set; }

    public string? Text { get; set; }

    public bool? IsCorrect { get; set; }

    public int? QuestionId { get; set; }

    public virtual Question? Question { get; set; }

    public virtual ICollection<UsersAnswer> UsersAnswers { get; } = new List<UsersAnswer>();
}
