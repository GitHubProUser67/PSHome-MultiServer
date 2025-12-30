using SSFWServer.Helpers.FileHelper;

namespace SSFWServer.Services
{
    public class AvatarService
    {
        public byte[]? HandleAvatarService(string absolutepath, string? key)
        {
            return FileHelper.ReadAllBytes(absolutepath, key);
        }
    }
}