using SSFWServer.Helpers.FileHelper;

namespace SSFWServer.Services
{
    public class AvatarService
    {
        public byte[]? HandleAvatarService(string absolutepath, string filePath, string? key)
        {
            if (File.Exists(filePath))
            {
                return FileHelper.ReadAllBytes(absolutepath, key);
            } else
            {
                return null;
            }
        }
    }
}