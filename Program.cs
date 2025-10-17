using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Spotted
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Cloudy - Series B (600M Round 3) Exam Platform");
            Console.WriteLine("Top level mock Azure questions written by Azure Professionals");
            while(true)
            {

                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Add User");
                Console.WriteLine("2. Delete User");
                Console.WriteLine("3. Purchase Exam");
                Console.WriteLine("4. List Exams");
                Console.WriteLine("5. Take Exam");
                Console.WriteLine("6. Exit");

                Console.WriteLine("Enter your choice: ");
                int choice = int.TryParse(Console.ReadLine(), out int result) ? result : 0;

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter email: ");
                        var email = Console.ReadLine();
                        AddUser(email);
                        break;
                    case 2:
                        Console.Write("Enter Name to delete: ");
                        var nameToDelete = Console.ReadLine();
                        if (nameToDelete != null)
                            DeleteUser(nameToDelete);
                        break;
                    case 3:
                        Console.Write("Enter Name to purchase exam for: ");
                        var nameToPurchase = Console.ReadLine();
                        Console.Write("Enter Exam Title to purchase: ");
                        var examTitle = Console.ReadLine();
                        if (nameToPurchase != null && examTitle != null)
                            PurchaseExam(nameToPurchase, examTitle);
                
                        break;
                    case 4:
                        Console.Write("Enter Name to list exams for: ");
                        var nameToList = Console.ReadLine();
                        if (nameToList != null)
                            ListExams(nameToList);
                        break;
                    case 5:
                        Console.Write("Enter Name to take exam for: ");
                        var nameToTake = Console.ReadLine();
                        Console.Write("Enter Exam Title to take: ");
                        var examTitleToTake = Console.ReadLine();

                        if (nameToTake != null && examTitleToTake != null)
                        {
                            TakeExam(nameToTake, examTitleToTake);
                        }

                        break;
                    case 6:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

        }

        public static void AddUser(string email)
        {
            using var context = new CloudyContext();
            var user = new User
            {
                Email = email,
                Profile = new Profile { DisplayName = "John Doe" },
            };
            context.Users.Add(user);
            context.SaveChanges();
        }

        public static void DeleteUser(string DisplayName)
        {
            using var context = new CloudyContext();
            var user = context.Users.FirstOrDefault(u => u.Profile.DisplayName == DisplayName);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        public static float TakeExam(string DisplayName, string examTitle)
        {
            using var context = new CloudyContext();

            var exam = context.Exams.FirstOrDefault(e => e.Title == examTitle);
            if (exam == null)
                return -1;

            var user = context.Users.FirstOrDefault(u => u.Profile.DisplayName == DisplayName);
            if (user == null)
                return -1;

            var userExam = context.UserExams.FirstOrDefault(ue =>
                ue.UserId == user.UserId && ue.ExamId == exam.ExamId
            );
            if (userExam == null)
                return -1;


            List<Question> questions = exam.Questions.ToList();

            int correctAnswers = 0;

            foreach (var q in questions)
            {
                Console.WriteLine(q.QuestionText);
                for (int i = 0; i < q.Options.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {q.Options[i]}");
                }

                Console.Write("Your answer (1-4): ");
                if (
                    int.TryParse(Console.ReadLine(), out int answer)
                    && answer - 1 == q.CorrectIndex
                )
                {
                    correctAnswers++;
                }
            }

            float score = (float)correctAnswers / questions.Count * 100;
            userExam.Passed = score >= 70;
            context.SaveChanges();
            return score;
        }

        public static void PurchaseExam(string DisplayName, string examTitle)
        {
            using var context = new CloudyContext();

            var exam = context.Exams.FirstOrDefault(e => e.Title == examTitle);
            if (exam == null)
                return;

            var user = context.Users.FirstOrDefault(u => u.Profile.DisplayName == DisplayName);
            if (user == null)
                return;

            var userExam = new UserExams
            {
                UserId = user.UserId,
                ExamId = exam.ExamId,
                Passed = false,
            };
            context.UserExams.Add(userExam);
            context.SaveChanges();
        }
    
        public static void ListExams(string DisplayName)
        {
            using var context = new CloudyContext();

            var user = context.Users.FirstOrDefault(u => u.Profile.DisplayName == DisplayName);
            if (user == null)
                return;

            var exams = context.UserExams.Include(ue => ue.Exam).ThenInclude(e => e.Topics)
                .Where(ue => ue.UserId == user.UserId)
                .ToList();

            foreach (var exam in exams)
            {
                Console.WriteLine($"Exam: {exam.Exam.Title}");
                Console.WriteLine($"Description: {exam.Exam.Description}");
                Console.WriteLine("Topics:");
                foreach (var topic in exam.Exam.Topics)
                {
                    Console.WriteLine($" - {topic.Name}");
                }

                if (exam.Passed)
                {
                    Console.WriteLine("Status: Passed");
                }
                else
                {
                    Console.WriteLine("Status: Failed");
                }

                Console.WriteLine();
            }
        }
    }
}
