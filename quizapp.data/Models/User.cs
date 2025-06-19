using System;
using System.Collections.Generic;

namespace quizapp.data.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<UsersAnswer> UsersAnswers { get; } = new List<UsersAnswer>();
}
