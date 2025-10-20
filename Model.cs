using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// great use of attributes for mapping properties to database columns
public class Profile
{
    [Column("profile_id")]
    public int ProfileId { get; set; }

    [Column("display_name")]
    public string DisplayName { get; set; } = string.Empty;
    public User? User { get; set; }
}

public class User
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("profile_id")]
    [ForeignKey("Profile")]
    public int ProfileId { get; set; }

    public Profile Profile { get; set; } = null!;

    [Column("email")]
    public required string Email { get; set; }

    public List<UserExam> UserExam { get; set; } = new();
}

public class UserExam
{
    [Key, Column("user_exam_id")]
    public int UserExamId { get; set; }

    [Column("user_id")]
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Column("exam_id")]
    [ForeignKey("Exam")]
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    [Column("passed")]
    public bool Passed { get; set; }
}

public class Exam
{
    [Column("exam_id")]
    public int ExamId { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    public List<Topic> Topics { get; set; } = new();

    public List<Question> Questions { get; set; } = new();
}

public class Question
{
    [Column("question_id"), Key]
    public int QuestionId { get; set; }

    [Column("question_text")]
    public string QuestionText { get; set; } = string.Empty;

    [Column("options", TypeName = "jsonb")]
    public string OptionsJson { get; set; } = "[]";

    [NotMapped]
    public List<string> Options
    {
        get => JsonConvert.DeserializeObject<List<string>>(OptionsJson) ?? new List<string>();
        set => OptionsJson = JsonConvert.SerializeObject(value);
    }

    [Column("correct_index")]
    public int CorrectIndex { get; set; }

    [Column("exam_id")]
    [ForeignKey("Exam")]
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
}

public class Topic
{
    [Column("topic_id")]
    public int TopicId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    // Make it 1:N under Exam to avoid EF creating a join table
    [Column("exam_id")]
    [ForeignKey(nameof(Exam))]
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
}
