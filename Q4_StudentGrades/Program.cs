using System;
using System.Collections.Generic;
using System.IO;

namespace DCIT318_Q4
{
    // Student class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id; FullName = fullName; Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70) return "B";
            if (Score >= 60) return "C";
            if (Score >= 50) return "D";
            return "F";
        }

        public override string ToString() => $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
    }

    // Custom exceptions
    public class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string msg): base(msg){} }
    public class MissingFieldException : Exception { public MissingFieldException(string msg): base(msg){} }

    // Processor class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();
            using var sr = new StreamReader(inputFilePath);
            int lineNo = 0;
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                lineNo++;
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length != 3) throw new MissingFieldException($"Line {lineNo} does not contain 3 fields.");
                if (!int.TryParse(parts[0].Trim(), out var id)) throw new InvalidScoreFormatException($"Invalid ID at line {lineNo}.");
                var name = parts[1].Trim();
                if (!int.TryParse(parts[2].Trim(), out var score)) throw new InvalidScoreFormatException($"Invalid score at line {lineNo}.");
                students.Add(new Student(id, name, score));
            }
            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using var sw = new StreamWriter(outputFilePath);
            foreach (var s in students)
            {
                sw.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            // For demo: create a sample input file in working directory.
            var input = "students_input.txt";
            var output = "students_report.txt";
            File.WriteAllLines(input, new string[]
            {
                "101, Alice Smith, 84",
                "102, Bob Johnson, 73",
                "103, Carol Adams, 49"
            });

            var proc = new StudentResultProcessor();
            try
            {
                var students = proc.ReadStudentsFromFile(input);
                proc.WriteReportToFile(students, output);
                Console.WriteLine("Report written to " + output);
                Console.WriteLine("Preview:");
                foreach (var line in File.ReadAllLines(output)) Console.WriteLine(line);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Input file not found.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine("Invalid score format: " + ex.Message);
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine("Missing field: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: " + ex.Message);
            }
        }
    }
}
