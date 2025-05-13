using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.ComponentModel;
using System;
public class AuthenticationScene : MonoBehaviour
{
    // Start is called before the first frame update
    // sign in button area
    [Header("Sign in")]
    [SerializeField] private GameObject signInPanel;
    [SerializeField] private TextMeshProUGUI signInStatusText;
    [SerializeField] private TMP_InputField emailOrUsernameInput;
    [SerializeField] private TMP_InputField passwordSignInInput;
    [SerializeField] private Button showPasswordSignInButton;
    [SerializeField] private Button forgotPasswordButton;
    [SerializeField] private Button signInButton;
    [SerializeField] private Button signUpButton1;
    [SerializeField] private Button signInByGoogleButton;

    [Header("Forgot password")]

    [SerializeField] private GameObject forgotPasswordPanel;
    [SerializeField] private TextMeshProUGUI forgotPasswordStatusText;
    [SerializeField] private TMP_InputField forgotPasswordEmailInput;
    [SerializeField] private Button sendResetPasswordButton;
    [SerializeField] private Button backButton1; 
    // add signup area here

    // sign up button area
    [Header("Sign up")]
    [SerializeField] private GameObject signUpPanel;
    [SerializeField] private TextMeshProUGUI signUpStatusText;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField fullNameInput;
    [SerializeField] private TMP_InputField dobInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordSignUpInput;
    [SerializeField] private Button femaleButton;
    [SerializeField] private Button maleButton;
    [SerializeField] private Button showPasswordSignUpButton;
    [SerializeField] private Button haveAccountButton;
    [SerializeField] private Button signUpButton2;
    [SerializeField] private Button backButton2;
    private string _gender = "";
    // end here
    void Start()
    {
        signInButton.onClick.AddListener(SignIn);
        showPasswordSignInButton.onClick.AddListener(ShowPasswordSignIn);
        forgotPasswordButton.onClick.AddListener(ShowForgotPasswordPanel);
        signUpButton1.onClick.AddListener(ShowSignUpPanel);
        signUpButton2.onClick.AddListener(SignUp);
        signInByGoogleButton.onClick.AddListener(SignInWithGoogle);

        backButton1.onClick.AddListener(ShowSignInPanel);
        backButton2.onClick.AddListener(ShowSignInPanel);
        sendResetPasswordButton.onClick.AddListener(SendResetPassword);

        femaleButton.onClick.AddListener(FemaleButton);
        maleButton.onClick.AddListener(MaleButton);
        haveAccountButton.onClick.AddListener(ShowSignInPanel);
    }

    private async void SignIn()
    {
        string emailOrUsername = emailOrUsernameInput.text.Trim();
        string password = passwordSignInInput.text;
        if (string.IsNullOrEmpty(emailOrUsername) || string.IsNullOrEmpty(password))
        {
            signInStatusText.text = "Vui lòng nhập tên đăng nhập hoặc email và mật khẩu.";
            return;
        }
        else {
            bool success = await AuthManager.Instance.SignIn(emailOrUsername, password);
            signInStatusText.text = "Đang đăng nhập...";
            if (success)
            {
                signInStatusText.text = "Đăng nhập thành công!";
                SceneManager.Instance.LoadScene("MainMenuScene");
            }
            else
            {
                signInStatusText.text = "Tên đăng nhập hoặc email và mật khẩu không chính xác.";
            }
        }
    }
    private void ShowPasswordSignIn()
    {
        passwordSignInInput.contentType = TMP_InputField.ContentType.Standard;
    }
    private void ShowForgotPasswordPanel()
    {
        // show a panel to input email
        signInPanel.SetActive(false);
        forgotPasswordPanel.SetActive(true);
    }
    private async void SendResetPassword()
    {
        string email = forgotPasswordEmailInput.text;
        if (string.IsNullOrEmpty(email))
        {
            forgotPasswordStatusText.text = "Vui lòng nhập email.";
            return;
        }
        bool success = await AuthManager.Instance.ForgotPassword(email);
        if (success)
        {
            forgotPasswordStatusText.text = "Email đã được gửi đến email của bạn.";
        }
        else
        {
            forgotPasswordStatusText.text = "Email không tồn tại.";
        }
    }
    private void ShowSignUpPanel()
    {
        signInPanel.SetActive(false);
        signUpPanel.SetActive(true);
    }
    private void ShowSignInPanel()
    {   
        signUpPanel.SetActive(false);
        forgotPasswordPanel.SetActive(false);
        signInPanel.SetActive(true);
    }
    // Sign up
    private async void SignUp()
    {
        // when click sign up button, check if the input is valid, print the error đầu tiên
        string username = usernameInput.text;
        string fullName = fullNameInput.text;
        // date of birth is in the format dd/mm/yyyy, read the date of birth from the input field and convert it to DateTime
        DateTime dob;
        if (!DateTime.TryParseExact(dobInput.text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dob))
        {
            signUpStatusText.text = "Ngày sinh không hợp lệ, hãy nhập lại theo định dạng dd/MM/yyyy.";
            return;
        }
        string phone = "";
        string address = "";
        string email = emailInput.text;
        string password = passwordSignUpInput.text;
        string gender = _gender;
        if (CheckUsername(username) != "")
        {
            signUpStatusText.text = CheckUsername(username);
            return;
        }
        if (CheckFullName(fullName) != "")
        {
            signUpStatusText.text = CheckFullName(fullName);
            return;
        }
        if (CheckDob(dob) != "")
        {
            signUpStatusText.text = CheckDob(dob);
            return;
        }
        if (CheckEmail(email) != "")
        {
            signUpStatusText.text = CheckEmail(email);
            return;
        }
        if (CheckPassword(password) != "")
        {
            signUpStatusText.text = CheckPassword(password);
            return;
        }
        if (CheckGender(gender) != "")
        {
            signUpStatusText.text = CheckGender(gender);
            return;
        }
        signUpStatusText.text = "Đang đăng ký...";
        // sign up
        bool success = await AuthManager.Instance.SignUp(email, password, username, fullName, dob, phone, address, gender, 1);
        if (success)
        {
            signUpStatusText.text = "Đăng ký thành công!";
        }
        else
        {
            signUpStatusText.text = "Đăng ký thất bại!";
        }
    }
    private void SignInWithGoogle()
    {
        throw new NotImplementedException();
    }
    private string CheckUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return "Vui lòng nhập tên người dùng";
        return "";
        
    }
    private string CheckFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return "Vui lòng nhập họ và tên.";
        return "";
    }
    private string CheckDob(DateTime dob)
    {
        if (dob == null)
            return "Vui lòng nhập ngày sinh.";
        // hãy kiểm tra ngày sinh có phải là ngày hiện tại không
        if (dob > DateTime.Now)
            return "Ngày sinh quá hiện tại, hãy nhập lại.";
        return "";
    }
    private string CheckGender(string gender)
    {
        if (string.IsNullOrWhiteSpace(gender))
            return "Vui lòng chọn giới tính.";
        return "";
    }
    private void FemaleButton()
    {
        _gender = "Nữ";
        // ấn vào nút này thì nút nam đổi sang màu trắng
        femaleButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
        maleButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
    private void MaleButton()
    {
        _gender = "Nam";
        // ấn vào nút này thì nút nữ đổi sang màu trắng
        femaleButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        maleButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
    }
    private string CheckEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Vui lòng nhập email.";

        if (email.Contains("@"))
        {
            // Kiểm tra định dạng email đơn giản
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "Email không hợp lệ.";
        }
        else
        {
            return "Email không hợp lệ.";
        }
        return ""; // Hợp lệ
    }
    private string CheckPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return "Vui lòng nhập mật khẩu.";
        if (password.Length < 6)
            return "Mật khẩu phải có ít nhất 6 ký tự.";
        if (password.Length > 20)
            return "Mật khẩu phải có tối đa 20 ký tự."; 
        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"^[a-zA-Z0-9]+$"))
            return "Mật khẩu chỉ được chứa chữ và số.";
        return "";
    }

    
}
