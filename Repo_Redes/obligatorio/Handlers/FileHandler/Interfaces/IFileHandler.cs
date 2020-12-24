namespace Util.FileHandler.Interfaces
{
    public interface IFileHandler
    {
        bool FileExists(string path);
        bool IsValidPath(string path, bool allowRelativePath = false);

        bool CheckFileType(string fileName);
        string GetFileName(string path);
        long GetFileSize(string path);
    }
}