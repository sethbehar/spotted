using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Spotted
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("\n===============================================");
            Console.WriteLine("Welcome to Cloudy - (Series B Funded: 600M Round 3) Exam Platform");
            Console.WriteLine("Top-level mock Azure questions written by Azure Professionals");
            Console.WriteLine("===============================================\n");

            while (true)
            {
                Console.WriteLine("\n-------------------- MENU --------------------");
                Console.WriteLine("1. Add User");
                Console.WriteLine("2. Delete User");
                Console.WriteLine("3. Purchase Exam");
                Console.WriteLine("4. List Exams");
                Console.WriteLine("5. Take Exam");
                Console.WriteLine("6. Exit");
                Console.WriteLine("----------------------------------------------\n");

                Console.Write("Enter your choice: ");
                int choice = int.TryParse(Console.ReadLine(), out int result) ? result : 0;
                Console.WriteLine();

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter email: ");
                        var email = Console.ReadLine();
                        Console.Write("Enter display name: ");
                        var displayName = Console.ReadLine();
                        AddUser(email, displayName);
                        break;

                    case 2:
                        Console.Write("Enter Name to delete: ");
                        var nameToDelete = Console.ReadLine();
                        if (nameToDelete != null)
                        {
                            DeleteUser(nameToDelete);
                        }
                        break;

                    case 3:
                        Console.Write("Enter Name to purchase exam for: ");
                        var nameToPurchase = Console.ReadLine();
                        Console.Write("Enter Exam Title to purchase: ");
                        var examTitle = Console.ReadLine();
                        if (nameToPurchase != null && examTitle != null)
                        {
                            PurchaseExam(nameToPurchase, examTitle);
                        }
                        break;

                    case 4:
                        Console.Write("Enter Name to list exams for: ");
                        var nameToList = Console.ReadLine();
                        if (nameToList != null)
                        {
                            Console.WriteLine();
                            ListExams(nameToList);
                            Console.WriteLine("----------------------------------------------\n");
                        }
                        break;

                    case 5:
                        Console.Write("Enter Name to take exam for: ");
                        var nameToTake = Console.ReadLine();
                        Console.Write("Enter Exam Title to take: ");
                        var examTitleToTake = Console.ReadLine();
                        Console.WriteLine();

                        string res = "";

                        if (nameToTake != null && examTitleToTake != null)
                        {
                            res = TakeExam(nameToTake, examTitleToTake);
                        }
                        else
                        {
                            Console.WriteLine("Invalid input for exam.\n");
                        }

                        if (!string.IsNullOrEmpty(res))
                        {
                            Console.WriteLine($"\nResult: {res}\n");
                        }

                        break;

                    case 6:
                        Console.WriteLine("\nExiting Cloudy...\n");
                        return;

                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.\n");
                        break;
                }
            }
        }

        public static void AddUser(string email, string displayName)
        {
            using var context = new CloudyContext();
            var user = new User
            {
                Email = email,
                Profile = new Profile { DisplayName = displayName },
            };
            context.Users.Add(user);
            context.SaveChanges();

            Console.WriteLine($"\n✅ User '{displayName}' added successfully.\n");
        }

        public static void DeleteUser(string DisplayName)
        {
            using var context = new CloudyContext();
            var user = context.Users.FirstOrDefault(u => u.Profile.DisplayName == DisplayName);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();

                Console.WriteLine($"\nUser '{DisplayName}' deleted successfully.\n");
            }
            else
            {
                Console.WriteLine($"\nUser '{DisplayName}' not found.\n");
            }
        }

        public static string TakeExam(string DisplayName, string examTitle)
        {
            using var context = new CloudyContext();

            var exam = context.Exams
                .Include(e => e.Questions)
                .Include(e => e.Topics)
                .FirstOrDefault(e => e.Title == examTitle);

            if (exam == null)
                return "Exam not found.";

            var user = context.Users.FirstOrDefault(u => u.Profile.DisplayName == DisplayName);
            if (user == null)
                return "User not found.";

            var userExam = context.UserExams.FirstOrDefault(ue =>
                ue.UserId == user.UserId && ue.ExamId == exam.ExamId
            );
            if (userExam == null)
                return "User has not purchased this exam.";

            List<Question> questions = exam.Questions.ToList();
            int correctAnswers = 0;

            Console.WriteLine("===============================================");
            Console.WriteLine($"Starting Exam: {exam.Title}");
            Console.WriteLine("===============================================\n");

            foreach (var q in questions)
            {
                Console.WriteLine(q.QuestionText);
                for (int i = 0; i < q.Options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {q.Options[i]}");
                }

                Console.Write("\nYour answer (1-4): ");
                if (int.TryParse(Console.ReadLine(), out int answer) && answer - 1 == q.CorrectIndex)
                {
                    correctAnswers++;
                }

                Console.WriteLine();
            }

            float score = (float)correctAnswers / questions.Count * 100;
            userExam.Passed = score >= 70;
            context.SaveChanges();

            Console.WriteLine($"\nScore: {score}%");
            Console.WriteLine(score >= 70 ? "✅ Exam passed!" : "❌ Exam failed.");
            Console.WriteLine("-----------------------------------------------\n");

            return score >= 70 ? "Exam passed!" : "Exam failed.";
        }

        public static void PurchaseExam(string DisplayName, string examTitle)
        {
            using var context = new CloudyContext();

            var exam = context.Exams.FirstOrDefault(e => e.Title == examTitle);
            if (exam == null) 
            {
                Console.WriteLine("Exam not found.");
                return;
            }
                

            var user = context.Users.FirstOrDefault(u => u.Profile.DisplayName == DisplayName);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            var userExam = new UserExams
            {
                UserId = user.UserId,
                ExamId = exam.ExamId,
                Passed = false,
            };

            context.UserExams.Add(userExam);
            context.SaveChanges();

            Console.WriteLine($"\n💳 Exam '{examTitle}' purchased successfully for user '{DisplayName}'.\n");
        }

        public static void ListExams(string DisplayName)
        {
            using var context = new CloudyContext();

            var user = context.Users.FirstOrDefault(u => u.Profile.DisplayName == DisplayName);
            if (user == null) {
                Console.WriteLine("User not found.");
                return;
            }

            var exams = context.UserExams
                .Include(ue => ue.Exam)
                .ThenInclude(e => e.Topics)
                .Where(ue => ue.UserId == user.UserId)
                .ToList();

            if (exams.Count == 0)
            {
                Console.WriteLine("No exams found for this user.");
                return;
            }

            Console.WriteLine("===============================================");
            Console.WriteLine($"Exams for {DisplayName}");
            Console.WriteLine("===============================================\n");

            foreach (var exam in exams)
            {
                Console.WriteLine($"Exam: {exam.Exam.Title}");
                Console.WriteLine($"Description: {exam.Exam.Description}");
                Console.WriteLine("Topics:");
                foreach (var topic in exam.Exam.Topics)
                {
                    Console.WriteLine($" - {topic.Name}");
                }

                Console.WriteLine($"Status: {(exam.Passed ? "Passed ✅" : "Failed ❌")}");
                Console.WriteLine("-----------------------------------------------\n");
            }
        }
    }
}
