using SSFWServer.Helpers.FileHelper;

namespace SSFWServer.Services
{
    public class AvatarService
    {
        public byte[]? HandleAvatarService(string filePath, string? key)
        {
            if (File.Exists(filePath))
            {
                return FileHelper.ReadAllBytes(filePath, key);
            } else
            {
                return null;
            }
        }
    }
}