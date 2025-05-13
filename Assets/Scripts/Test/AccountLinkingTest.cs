using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class AccountLinkingTest
{
    private AccountLinkManager accountLinkManager;

    [SetUp]
    public void Setup()
    {
        accountLinkManager = new AccountLinkManager();
    }

    [Test]
    public void LinkAccount_WithValidEmail_ShouldSucceed()
    {
        // Arrange
        string parentId = "test_parent";
        string studentEmail = "student@example.com";

        // Act
        bool success = accountLinkManager.LinkAccount(parentId, studentEmail);

        // Assert
        Assert.IsTrue(success, "Liên kết tài khoản thất bại");
        Assert.IsTrue(accountLinkManager.IsLinked(parentId, studentEmail), "Tài khoản không được liên kết");
    }

    [Test]
    public void UnlinkAccount_ShouldRemoveAccess()
    {
        // Arrange
        string parentId = "test_parent";
        string studentEmail = "student@example.com";
        accountLinkManager.LinkAccount(parentId, studentEmail);

        // Act
        bool success = accountLinkManager.UnlinkAccount(parentId, studentEmail);

        // Assert
        Assert.IsTrue(success, "Hủy liên kết thất bại");
        Assert.IsFalse(accountLinkManager.IsLinked(parentId, studentEmail), "Tài khoản vẫn còn liên kết");
    }
}