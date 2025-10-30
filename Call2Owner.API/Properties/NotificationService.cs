using FirebaseAdmin.Messaging;
using static Call2Owner.Controllers.SocietyUserController;

namespace Call2Owner.Services
{
    public class NotificationService
    {
        public async Task<NotificationResult> SendNotificationAsync(NotificationRequest request)
        {
            try
            {
                // Build Firebase message
                var message = new Message()
                {
                    Token = request.ResidentVerificationCode, // This should be the FCM device token!
                    Notification = new Notification
                    {
                        Title = request.EntyType + " is waiting at the Main Gate",
                        Body = $"{request.Name} ({request.EntyType}) has arrived." // simple readable message
                    },
                    // 👇 Send all other properties in data payload
                    Data = new Dictionary<string, string>
                {
                    { "SocietyFlatId", request.SocietyFlatId.ToString() },
                    { "EntyType", request.EntyType ?? "" },
                    { "ProfilePicture", request.ProfilePicture ?? "" },
                    { "Name", request.Name ?? "" },
                    { "PhoneNumber", request.PhoneNumber ?? "" },
                    { "CompanyLogo", request.CompanyLogo ?? "" },
                    { "CompanyName", request.CompanyName ?? "" },
                    { "ResidentVerificationCode", request.ResidentVerificationCode ?? "" }
                }
                };

                // Send the notification via Firebase
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);

                // ✅ Return structured success result
                return new NotificationResult
                {
                    Success = true,
                    MessageId = response,
                    Message = "Notification sent successfully"
                };
            }
            catch (Exception ex)
            {
                // ❌ Return structured failure result
                return new NotificationResult
                {
                    Success = false,
                    Message = $"Error sending FCM message: {ex.Message}"
                };
            }
        }

        public class NotificationResult
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public string? MessageId { get; set; }
        }

    }
}
