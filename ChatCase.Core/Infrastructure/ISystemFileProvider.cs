using Microsoft.Extensions.FileProviders;
using System.Text;

namespace ChatCase.Core.Infrastructure
{
    /// <summary>
    /// A file provider abstraction
    /// </summary>
    public interface ISystemFileProvider : IFileProvider
    {
        /// <summary>
        /// Combines an array of strings into a path
        /// </summary>
        /// <param name="paths">An array of parts of the path</param>
        /// <returns>The combined paths</returns>
        string Combine(params string[] paths);

        /// <summary>
        /// Creates all directories and subdirectories in the specified path unless they already exist
        /// </summary>
        /// <param name="path">The directory to create</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Creates or overwrites a file in the specified path
        /// </summary>
        /// <param name="path">The path and name of the file to create</param>
        void CreateFile(string path);

        /// <summary>
        /// Depth-first recursive delete, with handling for descendant directories open in Windows Explorer.
        /// </summary>
        /// <param name="path">Directory path</param>
        void DeleteDirectory(string path);

        /// <summary>
        /// Deletes the specified file
        /// </summary>
        /// <param name="filePath">The name of the file to be deleted. Wildcard characters are not supported</param>
        void DeleteFile(string filePath);

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="path">The path to test</param>
        /// <returns>
        /// true if path refers to an existing directory; false if the directory does not exist or an error occurs when
        /// trying to determine if the specified file exists
        /// </returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Determines whether the specified file exists
        /// </summary>
        /// <param name="filePath">The file to check</param>
        /// <returns>
        /// True if the caller has the required permissions and path contains the name of an existing file; otherwise,
        /// false.
        /// </returns>
        bool FileExists(string filePath);

        /// <summary>
        /// Returns the names of files (including their paths) that match the specified search
        /// pattern in the specified directory, using a value to determine whether to search subdirectories.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to search</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files in path. This parameter
        /// can contain a combination of valid literal path and wildcard (* and ?) characters
        /// , but doesn't support regular expressions.
        /// </param>
        /// <param name="topDirectoryOnly">
        /// Specifies whether to search the current directory, or the current directory and all
        /// subdirectories
        /// </param>
        /// <returns>
        /// An array of the full names (including paths) for the files in the specified directory
        /// that match the specified search pattern, or an empty array if no files are found.
        /// </returns>
        string[] GetFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true);

        /// <summary>
        /// Checks if the path is directory
        /// </summary>
        /// <param name="path">Path for check</param>
        /// <returns>True, if the path is a directory, otherwise false</returns>
        bool IsDirectory(string path);

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        string MapPath(string path);

        /// <summary>
        /// Opens a file, reads all lines of the file with the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="path">The file to open for reading</param>
        /// <param name="encoding">The encoding applied to the contents of the file</param>
        /// <returns>A string containing all lines of the file</returns>
        string ReadAllText(string path, Encoding encoding);

        /// <summary>
        /// Creates a new file, writes the specified string to the file using the specified encoding,
        /// and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="path">The file to write to</param>
        /// <param name="contents">The string to write to the file</param>
        /// <param name="encoding">The encoding to apply to the string</param>
        void WriteAllText(string path, string contents, Encoding encoding);
    }
}
