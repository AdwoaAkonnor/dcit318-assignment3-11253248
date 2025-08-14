using System;
using System.Collections.Generic;
using System.Linq;

namespace DCIT318_Q2
{
    // Generic repository (simple)
    public class Repository<T> where T : class
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);
        public List<T> GetAll() => new List<T>(_items);
        public T? GetById(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);
        public bool Remove(Func<T, bool> predicate)
        {
            var item = _items.FirstOrDefault(predicate);
            if (item == null) return false;
            return _items.Remove(item);
        }
    }

    // Entities
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id; Name = name; Age = age; Gender = gender;
        }

        public override string ToString() => $"{Name} (ID: {Id}, Age: {Age}, Gender: {Gender})";
    }

    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id; PatientId = patientId; MedicationName = medicationName; DateIssued = dateIssued;
        }

        public override string ToString() => $"{MedicationName} (PrescID:{Id}) issued {DateIssued:d}";
    }

    // HealthSystemApp
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
            _patientRepo.Add(new Patient(2, "Bob Johnson", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Carol Adams", 27, "Female"));

            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Paracetamol", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Metformin", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Atorvastatin", DateTime.Now.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Amlodipine", DateTime.Now.AddDays(-3)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            foreach (var p in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(p.PatientId))
                    _prescriptionMap[p.PatientId] = new List<Prescription>();
                _prescriptionMap[p.PatientId].Add(p);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("=== Patients ===");
            foreach (var patient in _patientRepo.GetAll())
                Console.WriteLine(patient);
            Console.WriteLine("================");
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var list) ? new List<Prescription>(list) : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            Console.WriteLine($"Prescriptions for patient ID {id}:");
            var list = GetPrescriptionsByPatientId(id);
            if (list.Count == 0) Console.WriteLine("No prescriptions found.");
            else foreach (var p in list) Console.WriteLine(p);
        }
    }

    class Program
    {
        static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();
            Console.WriteLine();
            app.PrintPrescriptionsForPatient(1); // show patient 1 as example
        }
    }
}

