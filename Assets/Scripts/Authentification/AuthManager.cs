using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase;
using System.Collections.Generic;
using System.Linq;

    public class AuthManager : MonoBehaviour
    {
        public static AuthManager Instance { get; private set; }
        
        private FirebaseAuth _auth;
        private Student _currentStudent;
        private FirebaseUser _user;
        private bool isFirebaseReady = false;
        private FirebaseFirestore _firestore;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeFirebase();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeFirebase()
        {
            Debug.Log("Initializing Firebase...");
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"Firebase initialization failed: {task.Exception}");
                    return;
                }

                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _auth = FirebaseAuth.DefaultInstance;
                    _firestore = FirebaseFirestore.DefaultInstance;
                    _auth.StateChanged += AuthStateChanged;
                    AuthStateChanged(this, null);
                    isFirebaseReady = true;
                    Debug.Log("Firebase initialized and ready.");
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }
        
        private async void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (_auth.CurrentUser != _user)
            {
                bool signedIn = _user != _auth.CurrentUser && _auth.CurrentUser != null
                    && _auth.CurrentUser.IsValid();
                if (!signedIn && _user != null)
                {
                    Debug.Log($"Signed out {_user.UserId}");
                }
                _user = _auth.CurrentUser;
                if (signedIn)
                {
                    Debug.Log($"Signed in {_user.UserId}");
                    // Load student data from Firebase
                    // Student student = await LoadStudentData(_user.UserId);
                    // if (student != null)
                    // {
                    //     _currentStudent = student;
                    //     Debug.Log($"Student data loaded: {JsonUtility.ToJson(_currentStudent)}");
                    // }
                    // else
                    // {
                    //     Debug.LogError("Student data not found");
                    // }
                }
            }
        }
        
        
        private void OnDestroy()
        {
            if (_auth != null)
            {
                _auth.StateChanged -= AuthStateChanged;
                _auth = null;
            }
        }
        
        // Đảm bảo Firebase đã sẵn sàng trước khi thực hiện các thao tác
        private async Task WaitForFirebaseReady()
        {
            while (!isFirebaseReady)
            {
                await Task.Delay(100);
            }
        }
        
        // Authentication methods
        public async Task<bool> SignUp(string email, string password, string userName, string fullName, DateTime dob, string phone, string address, string gender, int role)
        {
            await WaitForFirebaseReady();

            try
            {
                if (_auth == null)
                {
                    Debug.LogError("Firebase Auth not initialized!");
                    return false;
                }
                
                Debug.Log($"Attempting to sign up user: {email}");
                var authResult = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
                
                if (authResult != null)
                {
                    Debug.Log("Sign up successful");
                    await authResult.User.SendEmailVerificationAsync();
                    // Save user data to Firebase
                    await SaveUserData(authResult.User.UserId, email, userName, fullName, dob, phone, address, gender, role);
                    return true;
                }
                
                Debug.LogError("Sign up failed: authResult is null");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"SignUp error: {e.Message}");
                return false;
            }
        }
        
        private async Task SaveUserData(string userId, string email, string userName, string fullName, DateTime dob, string phone, string address, string gender, int role)
        {
            try
            {
                var userData = new Dictionary<string, object>
                {
                    { "email", email },
                    { "user_name", userName },
                    { "full_name", fullName },
                    { "DOB", dob }, // Nếu muốn lưu timestamp Firestore, để nguyên DateTime
                    { "phone", phone },
                    { "address", address },
                    { "gender", gender },
                    { "role", role },
                    { "createdAt", DateTime.Now },
                    { "lastLogin", DateTime.Now },
                    { "notification_preferences", new Dictionary<string, object>
                        {
                            { "progress_updates", true },
                            { "achievements", true },
                            { "class_announcements", true }
                        }
                    },
                    { "game_settings", new Dictionary<string, object>
                        {
                            { "difficulty", "easy" },
                            { "sound_enabled", true },
                            { "music_enabled", true },
                            { "language", "vi" },
                            { "notifications_enabled", true }
                        }
                    },
                    { "student_role", new Dictionary<string, object>
                        {
                            { "parent_id", "" },
                            { "class_id", "" },
                            { "level", 1 },
                            { "exp_points", 0 },
                            { "diamonds", 0 },
                            { "total_learning_time", 0 },
                            { "total_practice_time", 0 },
                            { "badges", new List<Dictionary<string, object>>() },
                            { "toys", new List<Dictionary<string, object>>() },
                            { "learning_progress", new Dictionary<string, object>() },
                            { "activity_logs", new List<Dictionary<string, object>>() }
                        }
                    }
                };
                await _firestore.Collection("users").Document(userId).SetAsync(userData);
                Debug.Log($"User data saved for user {userId} to Firestore");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving user data to Firestore: {e.Message}");
            }
        }
        
        
        public async Task<bool> SignIn(string emailOrUsername, string password)

        {
            await WaitForFirebaseReady();
            AuthResult authResult = null;
            try
            {
                if (_auth == null)
                {
                    Debug.LogError("Firebase Auth not initialized!");
                    return false;
                }

                Debug.Log($"Attempting to sign in user: {emailOrUsername}");

                string emailToUse = emailOrUsername;

                // Nếu không phải email, tìm email theo username
                if (!emailOrUsername.Contains("@"))
                {
                    // Tìm user theo username trong Firestore
                    var query = await _firestore.Collection("users")
                        .WhereEqualTo("user_name", emailOrUsername)
                        .Limit(1)
                        .GetSnapshotAsync();

                    if (query.Count == 0)
                    {
                        Debug.LogError("Username not found.");
                        return false;
                    }

                    var userDoc = query.Documents.FirstOrDefault();
                    if (userDoc != null && userDoc.ContainsField("email"))
                    {
                        emailToUse = userDoc.GetValue<string>("email");
                    }
                    else
                    {
                        Debug.LogError("Email not found for this username.");
                        return false;
                    }
                }

                // Đăng nhập bằng email và password
                authResult = await _auth.SignInWithEmailAndPasswordAsync(emailToUse, password);
                    if (authResult != null)
                {
                    Debug.Log("Sign in successful");
                    Debug.Log($"User ID: {authResult.User.UserId}");
                    await LoadUserData(authResult.User.UserId);
                    return true;
                }
                else
                {
                    Debug.LogError("Sign in failed: authResult is null");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"SignIn error: {e.Message}");
                return false;
            }
        }
        public async Task<bool> SignInWithGoogle()
        {
            return false;
            // await WaitForFirebaseReady();
            // AuthResult authResult = null;
            // try
            // {
            //     if (_auth == null)
            //     {
            // //         Debug.LogError("Firebase Auth not initialized!");
            //         return false;
            //     }
            //     var provider = new GoogleAuthProvider();
            //     authResult = await _auth.SignInWithPopupAsync(provider);
            //     if (authResult != null)
            //     {
            //         Debug.Log("Sign in with Google successful");
            //         return true;
            //     }
            //     else
            //     {
            //         Debug.LogError("Sign in with Google failed: authResult is null");
            //         return false;
            //     }
            // }
            // catch (Exception e)
            // {
            //     Debug.LogError($"SignInWithGoogle error: {e.Message}");
            //     return false;
            // }
        }
        private async Task LoadUserData(string userId)
        {   
            Debug.Log("Bắt đầu load user data");
            Student student = new Student();
            // Load basic user data
            var userDoc = await _firestore.Collection("users").Document(userId).GetSnapshotAsync();
            if (userDoc.Exists)
            {   
                var userData = userDoc.ToDictionary();
                student.StudentID = userId;
                student.FullName = userData["full_name"].ToString();
                student.DOB = userData["DOB"].ToString();
                student.Gender = userData["gender"].ToString();
                LocalDataManager.Instance.SaveStudent(student);
                // Load game settings (it's a field in user data)
                var gameSettings = (Dictionary<string, object>)userData["game_settings"];
                var gameSettingsData = new GameSettings();
                gameSettingsData.Difficulty = gameSettings["difficulty"].ToString();
                gameSettingsData.SoundEnabled = bool.Parse(gameSettings["sound_enabled"].ToString());
                gameSettingsData.MusicEnabled = bool.Parse(gameSettings["music_enabled"].ToString());
                LocalDataManager.Instance.SaveGameSettings(gameSettingsData);
                // Print game settings data, class to string
                Debug.Log($"Game settings data: {JsonUtility.ToJson(gameSettingsData)}");
               
                // Lấy student_role
                if (userData.TryGetValue("student_role", out var studentRoleObj) && studentRoleObj is Dictionary<string, object> studentRoleDict)
                {
                    // Lấy badges
                    if (studentRoleDict.TryGetValue("badges", out var badgesObj) && badgesObj is List<object> badgesList)
                    {
                        foreach (var badgeObj in badgesList)
                        {
                            if (badgeObj is Dictionary<string, object> badgeDict)
                            {
                                // Xử lý badge dạng object
                                string badgeId = badgeDict.ContainsKey("BadgeID") ? badgeDict["BadgeID"].ToString() : "";
                                string badgeName = badgeDict.ContainsKey("Name") ? badgeDict["Name"].ToString() : "";
                                string badgeDescription = badgeDict.ContainsKey("Description") ? badgeDict["Description"].ToString() : "";
                                string obtainedAtStr = "";
                                // lấy obtained_at: nó là string dạng "05/05/2025"
                                if (badgeDict.ContainsKey("obtained_at"))
                                {
                                    obtainedAtStr = badgeDict["obtained_at"].ToString();
                                }
                                Debug.Log($"Badge: {badgeId} - {badgeName} - {badgeDescription} - {obtainedAtStr}");
                                Badge badge = new Badge { BadgeID = badgeId , StudentID = userId, Name = badgeName, Description = badgeDescription, ObtainedAt = obtainedAtStr };
                                LocalDataManager.Instance.SaveBadge(badge);
                            }
                        }
                    }

                    // Lấy toys
                    if (studentRoleDict.TryGetValue("toys", out var toysObj) && toysObj is List<object> toysList)
                    {
                        foreach (var toyObj in toysList)
                        {
                            if (toyObj is Dictionary<string, object> toyDict)
                            {
                                string toyId = toyDict.ContainsKey("toy_id") ? toyDict["toy_id"].ToString() : "";
                                string toyName = toyDict.ContainsKey("name") ? toyDict["name"].ToString() : "";
                                string toyDescription = toyDict.ContainsKey("description") ? toyDict["description"].ToString() : "";
                                DateTime obtainedAt =  toyDict.ContainsKey("obtained_at") ? DateTime.Parse(toyDict["obtained_at"].ToString()) : DateTime.MinValue;
                                Debug.Log($"Toy: {toyId} - {toyName} - {toyDescription} - {obtainedAt}");
                                //LocalDataManager.Instance.SaveToy(new Toy { ToyID = toyId, Name = toyName, Description = toyDescription, ObtainedAt = obtainedAt });
                            }
                        }
                    }

                    // Lấy activity_logs
                    if (studentRoleDict.TryGetValue("activity_logs", out var logsObj) && logsObj is List<object> logsList)
                    {
                        foreach (var logObj in logsList)
                        {
                            if (logObj is Dictionary<string, object> logDict)
                            {
                                string logId = logDict.ContainsKey("log_id") ? logDict["log_id"].ToString() : "";
                                string studentId = logDict.ContainsKey("student_id") ? logDict["student_id"].ToString() : "";
                                string activityName = logDict.ContainsKey("activity_name") ? logDict["activity_name"].ToString() : "";
                                string activityType = logDict.ContainsKey("activity_type") ? logDict["activity_type"].ToString() : "";
                                string date = logDict.ContainsKey("date") ? logDict["date"].ToString() : "";
                                string level = logDict.ContainsKey("level") ? logDict["level"].ToString() : "";
                                string correctProblems = logDict.ContainsKey("correct_problems") ? logDict["correct_problems"].ToString() : "";
                                string totalProblems = logDict.ContainsKey("total_problems") ? logDict["total_problems"].ToString() : "";
                                string timeTaken = logDict.ContainsKey("time_taken") ? logDict["time_taken"].ToString() : "";
                                Debug.Log($"Activity log: {logId} - {studentId} - {activityName} - {activityType} - {date} - {level} - {correctProblems} - {totalProblems} - {timeTaken}");
                                //LocalDataManager.Instance.SaveActivityLog(new ActivityLog { LogID = logId, StudentID = studentId, ActivityName = activityName, ActivityType = activityType, Date = date, Level = int.Parse(level), CorrectProblems = int.Parse(correctProblems), TotalProblems = int.Parse(totalProblems), TimeTaken = float.Parse(timeTaken) });
                            }
                        }
                    }

                    // Lấy learning_progress
                    if (studentRoleDict.TryGetValue("learning_progress", out var progressObj) && progressObj is Dictionary<string, object> progressDict)
                    {   
                        string progressId = progressDict.ContainsKey("progress_id") ? progressDict["progress_id"].ToString() : "";
                        string chapter = progressDict.ContainsKey("chapter") ? progressDict["chapter"].ToString() : "";
                        string lesson = progressDict.ContainsKey("lesson") ? progressDict["lesson"].ToString() : "";
                        string status = progressDict.ContainsKey("status") ? progressDict["status"].ToString() : "";
                        string errorDetail = progressDict.ContainsKey("error_detail") ? progressDict["error_detail"].ToString() : "";
                        Debug.Log($"Learning progress: {progressId} - {chapter} - {lesson} - {status} - {errorDetail}");
                        LocalDataManager.Instance.SaveLearningProgress(new LearningProgress { ProgressID = progressId, Chapter = chapter, Lesson = lesson, Status = int.Parse(status), ErrorDetail = errorDetail });
                    }
                }
            }
            else
            {
                Debug.LogError("User data not found");
            }
            // print student data, k dùng json vì chỉ để debug thôi
            Debug.Log($"Student data: {student.FullName} {student.DOB} {student.Gender} {student.StudentID}");
        }
  

        public void SignOut()
        {
            if (_auth != null)
            {
                _auth.SignOut();
                _currentStudent = null;
                Debug.Log("User signed out");
            }
        }
        public async Task<bool> ForgotPassword(string email)
        {
            try
            {
                await _auth.SendPasswordResetEmailAsync(email);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"ForgotPassword error: {e.Message}");
                return false;
            }
        }

        public Student GetCurrentStudent()
        {
            return _currentStudent;
        }

        public string GetCurrentUserId()
        {
            return _user?.UserId;
        }


    }