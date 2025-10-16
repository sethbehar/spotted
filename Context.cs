using Microsoft.EntityFrameworkCore;

public class CloudyContext : DbContext
{
    // ✅ Add this ctor so the factory can pass options
    public CloudyContext(DbContextOptions<CloudyContext> options) : base(options) { }

    // (Optional) keep a parameterless ctor if you want
    public CloudyContext() { }

    public DbSet<Profile> Profiles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserExams> UserExams { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Question> Questions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // ✅ Only apply if no options were provided (e.g., by the factory/DI)
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=cloudy;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Relationships ----------
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<User>(u => u.ProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserExams>()
            .HasOne(ue => ue.User)
            .WithMany(u => u.UserExams)
            .HasForeignKey(ue => ue.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserExams>()
            .HasOne(ue => ue.Exam)
            .WithMany()
            .HasForeignKey(ue => ue.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Exam)
            .WithMany(e => e.Questions)
            .HasForeignKey(q => q.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Topic>()
            .HasOne(t => t.Exam)
            .WithMany(e => e.Topics)
            .HasForeignKey(t => t.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- Seeds (order: parents before children) ----------

        // Profiles first (parent of User)
        modelBuilder.Entity<Profile>().HasData(
            new Profile { ProfileId = 1, DisplayName = "Seth Behar" },
            new Profile { ProfileId = 2, DisplayName = "Mike Scared" }
        );

        // Users (FK -> Profiles)
        modelBuilder.Entity<User>().HasData(
            new User { UserId = 1, ProfileId = 1, Email = "sethbehar@gmail.com" },
            new User { UserId = 2, ProfileId = 2, Email = "mike@scaredofthedark.com" }
        );

        // Exams (parent of Topics/Questions)
        modelBuilder.Entity<Exam>().HasData(
            new Exam { ExamId = 1, Title = "AZ-900", Description = "Foundational knowledge of Azure concepts, services, cloud concepts, security, privacy, pricing, and support." },
            new Exam { ExamId = 2, Title = "AZ-204", Description = "Developing solutions for Microsoft Azure." }
        );

        // Topics (FK -> Exams)
        modelBuilder.Entity<Topic>().HasData(
            new Topic { TopicId = 1, ExamId = 1, Name = "Cloud", Description = "Understand cloud concepts (15-20%)" },
            new Topic { TopicId = 2, ExamId = 1, Name = "AI", Description = "Understand core Azure services (30-35%)" },
            new Topic { TopicId = 3, ExamId = 1, Name = "Security", Description = "Understand security, privacy, compliance, and trust (25-30%)" },
            new Topic { TopicId = 4, ExamId = 1, Name = "Data Governance", Description = "Understand Azure pricing and support (20-25%)" }
        );

        modelBuilder.Entity<Question>().HasData(
    new
    {
        QuestionId = 1,
        QuestionText = "Which Azure service should you use to store unstructured data such as images and videos?",
        Options = new[] { "Azure SQL Database", "Azure Blob Storage", "Azure Table Storage", "Azure Queue Storage" },
        CorrectIndex = 1,
        ExamId = 1
    },
    new
    {
        QuestionId = 2,
        QuestionText = "Which Azure service allows you to run virtualized Windows or Linux servers in the cloud?",
        Options = new[] { "Azure App Service", "Azure Virtual Machines", "Azure Kubernetes Service", "Azure Functions" },
        CorrectIndex = 1,
        ExamId = 1
    },
    new
    {
        QuestionId = 3,
        QuestionText = "What is the main benefit of using Azure Availability Zones?",
        Options = new[] { "They provide faster internet connections.", "They protect applications and data from datacenter failures.", "They reduce storage costs for data.", "They automatically scale applications based on demand." },
        CorrectIndex = 1,
        ExamId = 1
    },
    new
    {
        QuestionId = 4,
        QuestionText = "Which pricing model allows you to pay only for the exact amount of resources you use?",
        Options = new[] { "Reserved Instances", "Pay-as-you-go", "Enterprise Agreement", "Free Tier" },
        CorrectIndex = 1,
        ExamId = 1
    },
    new
    {
        QuestionId = 5,
        QuestionText = "Which Azure tool allows you to view the status of all Azure services globally?",
        Options = new[] { "Azure Service Health", "Azure Monitor", "Azure Advisor", "Azure Security Center" },
        CorrectIndex = 0,
        ExamId = 1
    }
);

    }
}