using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FilesDDL;
using System.IO;
using System.Xml.Serialization;


namespace UnitFile
{
    [TestClass]
    public class FileUnit
    {
        private string csv = "test.csv";
        private string json = "test.json";
        private string xml = "test.xml";
        private string yaml = "test.yaml";
        private List<Course> Courses = new List<Course>
        {
            new Course { Id = 1, Name = "Мобильная разработка", Teacher = "Юлия Сергеевна", CountStudent = 24 },
            new Course { Id = 2, Name = "Математическое моделирование", Teacher = "Людмила Васильевна", CountStudent = 25 }
        };

        [TestInitialize]
        public void Setup()
        {
            Files<Course>.WriteFile(Courses, csv);
            Files<Course>.WriteFile(Courses, json);
            Files<Course>.WriteFile(Courses, xml);
            Files<Course>.WriteFile(Courses, yaml);
        }

        [TestCleanup]
        public void Clean()
        {
            if (File.Exists(csv)) File.Delete(csv);
            if (File.Exists(json)) File.Delete(json);
            if (File.Exists(xml)) File.Delete(xml);
            if (File.Exists(yaml)) File.Delete(yaml);
        }

        [TestMethod]
        public void ReadFromCSV()
        {
            var courses = Files<Course>.ReadFromFile(csv);
            Assert.IsNotNull(courses);
            Assert.AreEqual(Courses.Count, courses.Count);
            Assert.AreEqual(Courses[0].Name, courses[0].Name);
        }

        [TestMethod]
        public void ReadFromJSON()
        {
            var courses = Files<Course>.ReadFromFile(json);
            Assert.IsNotNull(courses);
            Assert.AreEqual(Courses.Count, courses.Count);
            Assert.AreEqual(Courses[0].Name, courses[0].Name);
        }

        [TestMethod]
        public void ReadFromYAML()
        {
            var courses = Files<Course>.ReadFromFile(yaml);
            Assert.IsNotNull(courses);
            Assert.AreEqual(Courses.Count, courses.Count);
            Assert.AreEqual(Courses[0].Name, courses[0].Name);
        }


        [TestMethod]
        public void WriteToCSV()
        {
            string newFile = "new.csv";
            Files<Course>.WriteFile(Courses, newFile);
            Assert.IsTrue(File.Exists(newFile));
            File.Delete(newFile);
        }

        [TestMethod]
        public void WriteToJSON()
        {
            string newFile = "new.json";
            Files<Course>.WriteFile(Courses, newFile);
            Assert.IsTrue(File.Exists(newFile));
            File.Delete(newFile);
        }

        [TestMethod]
        public void WriteToXML()
        {
            string newFile = "new.xml";
            Files<Course>.WriteFile(Courses, newFile);
            Assert.IsTrue(File.Exists(newFile));
            File.Delete(newFile);
        }

        [TestMethod]
        public void WriteToYAML()
        {
            string newFile = "new.yaml";
            Files<Course>.WriteFile(Courses, newFile);
            Assert.IsTrue(File.Exists(newFile));
            File.Delete(newFile);
        }

    }
}

