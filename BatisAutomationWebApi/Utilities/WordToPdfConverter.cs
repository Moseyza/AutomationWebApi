using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using Aspose.Words;
using Aspose.Words.Saving;
using BatissWebOA;

namespace BatisAutomationWebApi.Utilities
{


    public class WordToPdfConverter
    {
        public void ExportAsPdf(string fileName, string destination)
        {
            var document = new Document(fileName);
            var saveOptions = new PdfSaveOptions();

            document.Save(destination, SaveFormat.Xps);
            //var wordInstance = new Application();
            //wordInstance.Documents.Add(fileName);
            //var doc = wordInstance.ActiveDocument;
            //doc.SaveAs(destination, WdSaveFormat.wdFormatXPS);
            //wordInstance.Quit();
        }

        public byte[] GetPdfFromWordBytes(byte[] bytes,Dictionary<string, object> bookmarks)
        {
            var wordWithBookmark = (new WordChanger()).SetBookmarks(bytes, bookmarks);
            using (var wordStream = new MemoryStream(wordWithBookmark))
            {
                var doc = new Document(wordStream);
                var pdfStream = new MemoryStream();
                doc.Save(pdfStream, Aspose.Words.SaveFormat.Pdf);
                pdfStream.Seek(0, SeekOrigin.Begin);
                //var xmlDoc = new XmlDocument();
                //xmlDoc.Load(pdfStream);
                var pdfDoc = new Aspose.Pdf.Document(pdfStream);
                var stream = new MemoryStream();
                pdfDoc.Save(stream);
                return stream.GetBuffer();

            }


        }
    }
}

