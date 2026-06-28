using SSFWServer.Helpers.FileHelper;

namespace SSFWServer.Services
{
    public class AvatarService
    {
        public static byte[]? HandleAvatarService(string filePath, string? key)
        {
            return File.Exists(filePath) ? FileHelper.ReadAllBytes(filePath, key) : null;
        }
    }
}
