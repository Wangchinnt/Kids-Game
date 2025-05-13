using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginTest : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Start()
    {
            loginButton.onClick.AddListener(TestLogin);
    }

    private async void TestLogin()
    {
        statusText.text = "Đang đăng nhập...";
        bool success = await AuthManager.Instance.SignIn(emailInput.text, passwordInput.text);
        if (success)
        {
            statusText.text = "Đăng nhập thành công!";
            Student student = LocalDataManager.Instance.LoadStudent(AuthManager.Instance.GetCurrentUserId());
            Debug.Log($"Load From Local: Student data: {student.FullName} {student.DOB} {student.Gender} {student.StudentID}");
        }
        else
        {
            statusText.text = "Đăng nhập thất bại!";
        }
        bool success2 = await AuthManager.Instance.ForgotPassword("yeunguoitronggio@gmail.com");
        if (success2)
        {
            statusText.text = "Gửi email khôi phục mật khẩu thành công!";
        }
        else
        {
            statusText.text = "Gửi email khôi phục mật khẩu thất bại!";
        }
        // print student data
       
    }
} 