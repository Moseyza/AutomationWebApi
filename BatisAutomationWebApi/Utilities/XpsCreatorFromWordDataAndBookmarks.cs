using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BatissWebOA;

namespace BatisAutomationWebApi.Utilities
{
    
        public  class XpsCreatorFromWordDataAndBookmarks
        {
            public IList<string> CreatePdfs(byte[] word, IList<Dictionary<string, object>> bookmarks)
            {
                var result = new List<string>();
                foreach (var bookmark in bookmarks)
                {
                    if (bookmark.ContainsKey("LetterNo"))
                        bookmark.Remove("LetterNo");

                    var path = CreatePdf(word, bookmark);
                    if (!string.IsNullOrEmpty(path))
                        result.Add(path);
                }
                if (bookmarks.Count == 0)
                {
                    var path = CreatePdf(word, new Dictionary<string, object>());
                    if (!string.IsNullOrEmpty(path))
                        result.Add(path);
                }
                return result;
            }

            public string CreatePdf(byte[] word, Dictionary<string, object> bookmarks)
            {
                var wordWithBookmark = (new WordChanger()).SetBookmarks(word, bookmarks);
                var filePresenter = new FilePresenter();
                var wordAddress = filePresenter.SaveToTempFolder(wordWithBookmark, ".docx");
                var pdfPath = filePresenter.GetFreePath(".xps");
                try
                {
                    (new WordToPdfConverter()).ExportAsPdf(wordAddress, pdfPath);
                    return pdfPath;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }
    }

