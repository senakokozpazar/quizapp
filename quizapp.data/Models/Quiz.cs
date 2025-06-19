using System;
using System.Collections.Generic;

namespace quizapp.data.Models;

public partial class Quiz
{
    public int QuizId { get; set; }

    public string? QuizTitle { get; set; }

    public string? Description { get; set; }
}
