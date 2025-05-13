using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SignUpTest : MonoBehaviour
{   
    private void Start() {
        TestSignUp();
    }
    private async void TestSignUp()
    {
        // Đảm bảo AuthManager đã được khởi tạo
        if (AuthManager.Instance == null)
        {
            Debug.LogError("AuthManager instance not found!");
            return;
        }

        string email = $"yeunguoitronggio@gmail.com";
        string password = "123456@";
        string userName = "Yeunguoitronggio";
        string fullName = "Tạ Quang Chiến";
        DateTime dob = DateTime.Now;
        string phone = "0123456789";
        string address = "Cầu Giấy, Hà Nội";
        string gender = "Nam";
        int role = 1;

        bool result = await AuthManager.Instance.SignUp(email, password, userName, fullName, dob, phone, address, gender, role);

        if (result)
        {
            Debug.Log("SignUp test passed!");
        }
        else
        {
            Debug.LogError("SignUp test failed!");
        }
    }
}
