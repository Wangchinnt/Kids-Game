using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class ClassManagementTest
{
    private ClassManager classManager;

    [SetUp]
    public void Setup()
    {
        classManager = new ClassManager();
    }

    [Test]
    public void CreateClass_ShouldAddToSystem()
    {
        // Arrange
        string className = "Test Class";
        string teacherId = "test_teacher";

        // Act
        bool success = classManager.CreateClass(className, teacherId);

        // Assert
        Assert.IsTrue(success, "Tạo lớp học thất bại");
        ClassInfo classInfo = classManager.GetClassInfo(className);
        Assert.IsNotNull(classInfo, "Không tìm thấy thông tin lớp học");
        Assert.AreEqual(teacherId, classInfo.TeacherId, "ID giáo viên không khớp");
    }

    [Test]
    public void AddRemoveStudent_ShouldUpdateClassList()
    {
        // Arrange
        string className = "Test Class";
        string studentId = "test_student";
        classManager.CreateClass(className, "test_teacher");

        // Act
        bool addSuccess = classManager.AddStudent(className, studentId);
        bool removeSuccess = classManager.RemoveStudent(className, studentId);

        // Assert
        Assert.IsTrue(addSuccess, "Thêm học sinh thất bại");
        Assert.IsTrue(removeSuccess, "Xóa học sinh thất bại");
        Assert.IsFalse(classManager.IsStudentInClass(className, studentId), "Học sinh vẫn còn trong lớp");
    }
}