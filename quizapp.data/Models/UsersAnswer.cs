using System;
using System.Collections.Generic;

namespace quizapp.data.Models;

public partial class UsersAnswer
{
    public int UsersAnswerId { get; set; }

    public int? UserId { get; set; }

    public int? AnswerId { get; set; }

    public virtual Answer? Answer { get; set; }

    public virtual User? User { get; set; }
}
