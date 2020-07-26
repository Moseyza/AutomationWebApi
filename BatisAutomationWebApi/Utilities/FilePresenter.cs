using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DataTransferObjects;

namespace BatisAutomationWebApi.Utilities
{

    public interface IFilePresenter
    {
        void Save(FileDto file);
        string Show(FileDto file);
        void OpenFile(string filePath);
        string SaveToTempFolder(FileDto file);
    }

    public class FilePresenter : IFilePresenter
        {
            public void Save(FileDto file)
            {
                var extension = Path.GetExtension(file.Extension);
                //var savePath = ChooseSavePath(extension);
                //if (UserWantsToDownloadFile(savePath))
                //    SaveFile(savePath.ChoosedPath, file);
            }

            public string SaveToTempFolder(FileDto file)
            {
                var extension = file.Extension;
                string filePath = GetFileFullPath(extension);
                SaveFile(filePath, file);
                return filePath;
            }

            public string SaveToTempFolder(byte[] file, string extension)
            {
                return SaveToTempFolder(new FileDto { Content = file, Extension = extension });
            }

            public string Show(FileDto file)
            {
                var extension = file.Extension;
                string filePath = GetFileFullPath(extension);
                SaveFile(filePath, file);
                OpenFile(filePath);
                return filePath;
            }

            public string GetFreePath(string extension)
            {
                return GetFileFullPath(extension);
            }

            private string GetFileFullPath(string extension)
            {
                var batisTempFolder = GetBatisTempFolder();
                var fileName = CreateTemporaryFileName(extension);
                var fileFullPath = GetTemporaryFileFullPath(batisTempFolder, fileName);
                return fileFullPath;
            }

            private static string GetTemporaryFileFullPath(string batisTempFolder, string fileName)
            {
                return Path.Combine(batisTempFolder, fileName);
            }

            private string CreateTemporaryFileName(string extension)
            {
                extension = Path.GetExtension(extension);
                return string.Format("{0}{1}", Guid.NewGuid().ToString(), extension);
            }

            private static string GetBatisTempFolder()
            {
                var tempFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var batisTempFolder = Path.Combine(tempFolder, @"BatisErp");
                if (IsTempFolderDoesNotExists(batisTempFolder))
                {
                    Directory.CreateDirectory(batisTempFolder);
                }

                return batisTempFolder;
            }

            private static bool IsTempFolderDoesNotExists(string folderAddress)
            {
                bool isNotExist = !Directory.Exists(folderAddress);
                return isNotExist;

            }

            public void OpenFile(string filePath)
            {
                var process = System.Diagnostics.Process.Start(filePath);

            }

            //private static bool UserWantsToDownloadFile(ChooseSavePath savePath)
            //{
            //    return savePath.IsAnyFileChoosed;
            //}

            private static void SaveFile(string savePath, FileDto fileData)
            {
                //var fileSaver = new FileSaver();
                //fileSaver.Save(savePath, fileData.Content);
            }

            //private ChooseSavePath ChooseSavePath(string extension)
            //{
            //    var saveFileDialog = new SaveFileDialog();
            //    var savePath = saveFileDialog.SaveFile(extension);
            //    return savePath;
            //}
        }
    }

