using System;
using System.Diagnostics;
using System.Linq;

namespace ExaminationSystem
{
    // Answer Class
    public class Answer : ICloneable, IComparable<Answer>
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }

        public Answer(int id, string text)
        {
            AnswerId = id;
            AnswerText = text;
        }

        public object Clone() => new Answer(AnswerId, AnswerText);
        public int CompareTo(Answer other) => AnswerId.CompareTo(other.AnswerId);
        public override string ToString() => $"{AnswerId}- {AnswerText}";
    }

    // Base Question Class
    public abstract class Question : ICloneable, IComparable<Question>
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public int Mark { get; set; }
        public Answer[] AnswerList { get; set; }
        public int RightAnswerId { get; set; }
        public abstract string QuestionType { get; }

        protected Question(string header, string body, int mark, int answerCount)
        {
            Header = header;
            Body = body;
            Mark = mark;
            AnswerList = new Answer[answerCount];
        }

        public abstract object Clone();
        public int CompareTo(Question other) => Mark.CompareTo(other.Mark);

        public override string ToString()
        {
            // Exact match to screenshot: "Question 1: aaa" then "MCQ Question:    Mark 10"
            string answers = string.Join("\n", AnswerList.Select(a => a.ToString()));
            return $"{Header}\n{QuestionType}    Mark {Mark}\n{answers}";
        }
    }

    // True/False Question
    public class TrueFalseQuestion : Question
    {
        public override string QuestionType => "True/False Question:";

        public TrueFalseQuestion(string header, string body, int mark)
            : base(header, body, mark, 2)
        {
            AnswerList[0] = new Answer(1, "True");
            AnswerList[1] = new Answer(2, "False");
        }

        public override object Clone()
        {
            var q = new TrueFalseQuestion(Header, Body, Mark);
            q.RightAnswerId = RightAnswerId;
            return q;
        }
    }

    // MCQ Question
    public class MCQQuestion : Question
    {
        public override string QuestionType => "MCQ Question:";

        public MCQQuestion(string header, string body, int mark, int answerCount)
            : base(header, body, mark, answerCount) { }

        public override object Clone()
        {
            var q = new MCQQuestion(Header, Body, Mark, AnswerList.Length);
            Array.Copy(AnswerList, q.AnswerList, AnswerList.Length);
            q.RightAnswerId = RightAnswerId;
            return q;
        }
    }

    // Base Exam Class
    public abstract class Exam
    {
        public int Time { get; set; }
        public int NumberOfQuestions { get; set; }
        public Question[] Questions { get; set; }

        protected Exam(int time, int numberOfQuestions)
        {
            Time = time;
            NumberOfQuestions = numberOfQuestions;
            Questions = new Question[numberOfQuestions];
        }

        public abstract void ShowExam();
        public override string ToString() => $"Time: {Time} mins, Questions: {NumberOfQuestions}";
    }

    // Final Exam
    public class FinalExam : Exam
    {
        public FinalExam(int time, int numberOfQuestions) : base(time, numberOfQuestions) { }

        public override void ShowExam()
        {
            Console.WriteLine("Final Exam");
            int totalMarks = 0, obtainedMarks = 0;

            foreach (var q in Questions)
            {
                Console.WriteLine(q);
                Console.Write("Enter your answer ID: ");
                int ans = int.Parse(Console.ReadLine());

                totalMarks += q.Mark;
                if (ans == q.RightAnswerId)
                    obtainedMarks += q.Mark;

                Console.WriteLine();
            }

            Console.WriteLine($"Your Grade is {obtainedMarks} from {totalMarks}");
            Console.WriteLine("Thank you");
        }
    }

    // Practical Exam
    public class PracticalExam : Exam
    {
        public PracticalExam(int time, int numberOfQuestions) : base(time, numberOfQuestions) { }

        public override void ShowExam()
        {
            Console.WriteLine("Practical Exam");
            int totalMarks = 0, obtainedMarks = 0;

            foreach (var q in Questions)
            {
                Console.WriteLine(q);
                Console.Write("Enter your answer ID: ");
                int ans = int.Parse(Console.ReadLine());

                totalMarks += q.Mark;
                if (ans == q.RightAnswerId)
                    obtainedMarks += q.Mark;
                Console.WriteLine();
            }

            Console.WriteLine("Practical Exam Results:");
            foreach (var q in Questions)
            {
                Console.WriteLine($"{q.Header}");
                Console.WriteLine($"Your Answer => {q.AnswerList.First(a => a.AnswerId == q.RightAnswerId).AnswerText}");
                Console.WriteLine($"Correct Answer => {q.AnswerList.First(a => a.AnswerId == q.RightAnswerId).AnswerText}");
                Console.WriteLine();
            }

            Console.WriteLine($"Your Grade is {obtainedMarks} from {totalMarks}");
            Console.WriteLine($"Time = {TimeSpan.FromTicks(Stopwatch.GetTimestamp()).ToString()}");
            Console.WriteLine("Thank you");
        }
    }

    // Subject Class
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public Exam Exam { get; set; }

        public Subject(int id, string name)
        {
            SubjectId = id;
            SubjectName = name;
        }

        public void CreateExam(Exam exam) => Exam = exam;
        public override string ToString() => $"Subject: {SubjectName} (ID: {SubjectId})";
    }

    // Main Program
    class Program
    {
        static void Main()
        {
            Subject subject = new Subject(1, "C# Programming");

            Console.Write("Enter the type of exam (1 for Practical, 2 for Final): ");
            int type = int.Parse(Console.ReadLine());

            Console.Write("Please enter the time for the exam (30 to 180 minutes): ");
            int time = int.Parse(Console.ReadLine());

            Console.Write("Please enter the number of questions: ");    
            int num = int.Parse(Console.ReadLine());

            Exam exam = type == 1 ? new PracticalExam(time, num) : new FinalExam(time, num);

            for (int i = 0; i < num; i++)
            {
                Console.WriteLine($"Please enter the question body:");
                string body = Console.ReadLine();

                Console.WriteLine("Please enter the question mark:");
                int mark = int.Parse(Console.ReadLine());

                // Both Practical and Final (in your screenshot) use MCQ
                Console.WriteLine("Choices of Question:");
                string[] choices = new string[4];
                for (int j = 0; j < 4; j++)
                {
                    Console.WriteLine($"Please enter choice number {j + 1}:");
                    choices[j] = Console.ReadLine();
                }

                Question q = new MCQQuestion($"Question {i + 1}: {body}", body, mark, 4);
                for (int j = 0; j < 4; j++)
                    q.AnswerList[j] = new Answer(j + 1, choices[j]);

                Console.Write("Please enter the right answer ID: ");
                q.RightAnswerId = int.Parse(Console.ReadLine());

                exam.Questions[i] = q;
            }

            subject.CreateExam(exam);

            Console.Write("Do You Want To Start Exam (Y | N): ");
            string start = Console.ReadLine();

            if (start.ToUpper() == "Y")
            {
                Console.WriteLine();
                exam.ShowExam();
            }
        }
    }
}