using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Net;

namespace Call2Owner.Services
{
    public static class FirebaseInitializer
    {
        private static bool _isInitialized;

        public static void InitializeFirebase()
        {
            if (_isInitialized)
                return;

            try
            {
                // ✅ Remote URL of your Firebase Admin SDK JSON
                var jsonUrl = "https://apisociety.call2owner.com/Images/firebase/wltpe-404512-firebase-adminsdk-fvoiz-26acbc4461.json";

                // ✅ Create a temp file path
                var tempPath = Path.Combine(Path.GetTempPath(), "firebase_admin_sdk.json");

                // ✅ Download the JSON file
                using (var client = new WebClient())
                {
                    client.DownloadFile(jsonUrl, tempPath);
                }

                // ✅ Initialize Firebase using the downloaded file
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(tempPath)
                });

                _isInitialized = true;
                Console.WriteLine("✅ Firebase initialized successfully from remote JSON file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Firebase initialization failed: " + ex.Message);
                throw; // Optionally rethrow if you want the app to stop when Firebase fails
            }
        }
    }
}
