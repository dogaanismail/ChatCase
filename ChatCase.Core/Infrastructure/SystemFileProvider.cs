using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChatCase.Core.Infrastructure
{
    /// <summary>
    /// IO functions using the on-disk file system
    /// </summary>
    public class SystemFileProvider : PhysicalFileProvider, ISystemFileProvider
    {
        /// <summary>
        /// Initializes a file provider
        /// </summary>
        /// <param name="webHostEnvironment"></param>
        public SystemFileProvider(IWebHostEnvironment webHostEnvironment)
            : base(File.Exists(webHostEnvironment.ContentRootPath) ? Path.GetDirectoryName(webHostEnvironment.ContentRootPath) : webHostEnvironment.ContentRootPath)
        {
            WebRootPath = File.Exists(webHostEnvironment.WebRootPath)
                ? Path.GetDirectoryName(webHostEnvironment.WebRootPath)
                : webHostEnvironment.WebRootPath;
        }

        #region Utilities

        private static void DeleteDirectoryRecursive(string path)
        {
            Directory.Delete(path, true);
            const int maxIterationToWait = 10;
            var curIteration = 0;

            while (Directory.Exists(path))
            {
                curIteration += 1;
                if (curIteration > maxIterationToWait)
                    return;
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Determines if the string is a valid Universal Naming Convention (UNC)
        /// for a server and share path.
        /// </summary>
        /// <param name="path">The path to be tested.</param>
        /// <returns><see langword="true"/> if the path is a valid UNC path; 
        /// otherwise, <see langword="false"/>.</returns>
        protected static bool IsUncPath(string path)
        {
            return Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.IsUnc;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Combines an array of strings into a path
        /// </summary>
        /// <param name="paths">An array of parts of the path</param>
        /// <returns>The combined paths</returns>
        public virtual string Combine(params string[] paths)
        {
            var path = Path.Combine(paths.SelectMany(p => IsUncPath(p) ? new[] { p } : p.Split('\\', '/')).ToArray());

            if (Environment.OSVersion.Platform == PlatformID.Unix && !IsUncPath(path))
                path = "/" + path;

            return path;
        }

        /// <summary>
        /// Creates all directories and subdirectories in the specified path unless they already exist
        /// </summary>
        /// <param name="path">The directory to create</param>
        public virtual void CreateDirectory(string path)
        {
            if (!DirectoryExists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Creates or overwrites a file in the specified path
        /// </summary>
        /// <param name="path">The path and name of the file to create</param>
        public virtual void CreateFile(string path)
        {
            if (FileExists(path))
                return;

            var fileInfo = new FileInfo(path);
            CreateDirectory(fileInfo.DirectoryName);

            using (File.Create(path))
            {
            }
        }

        /// <summary>
        ///  Depth-first recursive delete, with handling for descendant directories open in Windows Explorer.
        /// </summary>
        /// <param name="path">Directory path</param>
        public void DeleteDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(path);

            foreach (var directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                DeleteDirectoryRecursive(path);
            }
            catch (IOException)
            {
                DeleteDirectoryRecursive(path);
            }
            catch (UnauthorizedAccessException)
            {
                DeleteDirectoryRecursive(path);
            }
        }

        /// <summary>
        /// Deletes the specified file
        /// </summary>
        /// <param name="filePath">The name of the file to be deleted. Wildcard characters are not supported</param>
        public virtual void DeleteFile(string filePath)
        {
            if (!FileExists(filePath))
                return;

            File.Delete(filePath);
        }

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Determines whether the specified file exists
        /// </summary>
        /// <param name="filePath">The file to check</param>
        /// <returns>
        /// True if the caller has the required permissions and path contains the name of an existing file; otherwise,
        /// false.
        /// </returns>
        public virtual bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// returns files
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="searchPattern"></param>
        /// <param name="topDirectoryOnly"></param>
        /// <returns></returns>
        public virtual string[] GetFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true)
        {
            if (string.IsNullOrEmpty(searchPattern))
                searchPattern = "*.*";

            return Directory.GetFiles(directoryPath, searchPattern,
                topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        /// <summary>
        /// Checks if the path is directory
        /// </summary>
        /// <param name="path">Path for check</param>
        /// <returns>True, if the path is a directory, otherwise false</returns>
        public virtual bool IsDirectory(string path)
        {
            return DirectoryExists(path);
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string MapPath(string path)
        {
            path = path.Replace("~/", string.Empty).TrimStart('/');

            var pathEnd = path.EndsWith('/') ? Path.DirectorySeparatorChar.ToString() : string.Empty;

            return Combine(Root ?? string.Empty, path) + pathEnd;
        }

        /// <summary>
        /// Opens a file, reads all lines of the file with the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="path">The file to open for reading</param>
        /// <param name="encoding">The encoding applied to the contents of the file</param>
        /// <returns>A string containing all lines of the file</returns>
        public virtual string ReadAllText(string path, Encoding encoding)
        {
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(fileStream, encoding);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Creates a new file, writes the specified string to the file using the specified encoding,
        /// and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="path">The file to write to</param>
        /// <param name="contents">The string to write to the file</param>
        /// <param name="encoding">The encoding to apply to the string</param>
        public virtual void WriteAllText(string path, string contents, Encoding encoding)
        {
            File.WriteAllText(path, contents, encoding);
        }

        #endregion

        protected string WebRootPath { get; }
    }
}
