using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using a = DocumentFormat.OpenXml.Drawing;
using pic = DocumentFormat.OpenXml.Drawing.Pictures;

namespace BatissWebOA
{
 
    public class WordChanger
    {
        private static byte[] EmptyPicture =
        {
            0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a, 0x00, 0x00, 0x00, 0x0d, 0x49, 0x48, 0x44, 0x52,
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x08, 0x06, 0x00, 0x00, 0x00, 0x1f, 0x15, 0xc4,
            0x89, 0x00, 0x00, 0x00, 0x01, 0x73, 0x52, 0x47, 0x42, 0x00, 0xae, 0xce, 0x1c, 0xe9, 0x00, 0x00,
            0x00, 0x04, 0x67, 0x41, 0x4d, 0x41, 0x00, 0x00, 0xb1, 0x8f, 0x0b, 0xfc, 0x61, 0x05, 0x00, 0x00,
            0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0e, 0xc3, 0x00, 0x00, 0x0e, 0xc3, 0x01, 0xc7,
            0x6f, 0xa8, 0x64, 0x00, 0x00, 0x00, 0x1a, 0x74, 0x45, 0x58, 0x74, 0x53, 0x6f, 0x66, 0x74, 0x77,
            0x61, 0x72, 0x65, 0x00, 0x50, 0x61, 0x69, 0x6e, 0x74, 0x2e, 0x4e, 0x45, 0x54, 0x20, 0x76, 0x33,
            0x2e, 0x35, 0x2e, 0x31, 0x30, 0x30, 0xf4, 0x72, 0xa1, 0x00, 0x00, 0x00, 0x0d, 0x49, 0x44, 0x41,
            0x54, 0x18, 0x57, 0x63, 0xf8, 0xff, 0xff, 0x3f, 0x03, 0x00, 0x08, 0xfc, 0x02, 0xfe, 0x88, 0x5f,
            0x06, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4e, 0x44, 0xae, 0x42, 0x60, 0x82
        };
        public byte[] SetBookmarks(byte[] file, IDictionary<string, object> bookmarks)
        {
            byte[] result = null;
            using (var stream = new MemoryStream())
            {
                stream.Write(file, 0, (int)file.Length);
                stream.Seek(0, SeekOrigin.Begin);
                SetBookmarks(stream, bookmarks);
                result = stream.ToArray();
            }
            return result;
        }

        private void SetBookmarks(Stream stream, IDictionary<string, object> bookmarks)
        {
            try
            {
                using (WordprocessingDocument document = WordprocessingDocument.Open(stream, true))
                {
                    foreach (var bookmark in bookmarks)
                    {
                        try
                        {
                            if (bookmark.Value is string)
                                SetBookmark(document, bookmark.Key, bookmark.Value.ToString());
                            else if (bookmark.Value is IList<IList<string>>)
                                SetBookmark(document, bookmark.Key, bookmark.Value as IList<IList<string>>);
                            else
                                SetBookmark(document, bookmark.Key, bookmark.Value as byte[]);
                        }
                        catch (Exception ex)
                        { }
                    }
                    document.MainDocumentPart.Document.Save();
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void SetBookmark(WordprocessingDocument document, string bookmarkName, string bookmarkValue)
        {
            var bookmarkStarts = FindBookmark(document, bookmarkName);

            foreach (BookmarkStart bookmarkStart in bookmarkStarts)
            {
                Run bookmark = GetBookmarkedRun(bookmarkStart);
                if (bookmark == null)
                    return;
                bookmark.RemoveAllChildren<Text>();
                bookmark.RemoveAllChildren<Break>();
                var rightToLeftFlag = new RightToLeftText() { Val = OnOffValue.FromBoolean(false) };
                if (bookmark.RunProperties == null)
                    bookmark.RunProperties = new RunProperties();
                bookmark.RunProperties.RightToLeftText = rightToLeftFlag;
                while (!(bookmark.NextSibling() is BookmarkEnd) && bookmark.NextSibling() != null)
                {
                    bookmark.Parent.RemoveChild(bookmark.NextSibling());
                }
                var bookmarkValues = bookmarkValue.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
                if (bookmarkValues.Any())
                {
                    bookmark.AppendChild(new Text(bookmarkValues.First()));
                    for (int i = 1; i < bookmarkValues.Count(); i++)
                    {
                        bookmark.AppendChild(new Break());
                        bookmark.AppendChild(new Text(bookmarkValues[i]));
                    }
                }
            }
        }

        private Run GetBookmarkedRun(BookmarkStart bookmarkStart)
        {
            var element = bookmarkStart.NextSibling();
            while (NotReachedCorrespondingBookmarkEnd(bookmarkStart, element))
            {
                if (element is Run)
                    return (Run)element;
                element = element.NextSibling();
            }
            return null;
        }

        private bool NotReachedCorrespondingBookmarkEnd(BookmarkStart bookmarkStart, OpenXmlElement element)
        {
            var bookmarkEnd = element as BookmarkEnd;
            if (bookmarkEnd == null)
                return true;
            if (bookmarkEnd.Id.HasValue && bookmarkStart.Id.HasValue && bookmarkEnd.Id.Value == bookmarkStart.Id.Value)
                return false;
            return true;
        }

        private static IEnumerable<BookmarkStart> FindBookmark(WordprocessingDocument document, string bookmarkName)
        {
            var result = new List<BookmarkStart>();
            var bookmark = document.MainDocumentPart.RootElement.Descendants().OfType<BookmarkStart>().Where(x => x.Name == bookmarkName).ToList();
            if (bookmark != null)
                result = result.Union(bookmark).ToList();

            foreach (var header in document.MainDocumentPart.HeaderParts)
            {
                bookmark = header.RootElement.Descendants().OfType<BookmarkStart>().Where(x => x.Name == bookmarkName).ToList();
                if (bookmark != null)
                    result = result.Union(bookmark).ToList();
            }

            foreach (var footerPart in document.MainDocumentPart.FooterParts)
            {
                bookmark = footerPart.RootElement.Descendants().OfType<BookmarkStart>().Where(x => x.Name == bookmarkName).ToList();
                if (bookmark != null)
                    result = result.Union(bookmark).ToList();
            }

            return result;
        }

        private void SetBookmark(WordprocessingDocument document, string bookmarkName, IList<IList<string>> bookmarkValue)
        {
            var bookmarkStarts = FindBookmark(document, bookmarkName);
            if (bookmarkStarts == null)
                return;
            if (bookmarkValue == null)
                bookmarkValue = new List<IList<string>>();
            foreach (BookmarkStart bookmarkStart in bookmarkStarts)
            {
                OpenXmlElement element = bookmarkStart;
                do
                {
                    element = element.Parent;
                } while ((element is Table) == false);

                var lastRow = element.Descendants<TableRow>().Last();
                var newRowPattern = lastRow.CloneNode(true);
                foreach (var ele in element.ChildElements.ToList())
                {
                    if (ele is TableRow && ele != element.GetFirstChild<TableRow>())
                    {
                        element.RemoveChild(ele);
                    }
                }
                foreach (var row in bookmarkValue)
                {
                    var newRow = newRowPattern.CloneNode(true);
                    var cells = newRow.Descendants<TableCell>().ToList();
                    for (int i = 0; i < row.Count; i++)
                    {
                        var cell = cells[i];
                        var text = cell.GetFirstChild<Paragraph>().GetFirstChild<Run>().GetFirstChild<Text>();
                        text.Text = row[i];
                    }
                    element.AppendChild(newRow);
                }
            }
        }
        private void SetBookmark(WordprocessingDocument document, string bookmarkName, byte[] bookmarkValue)
        {
            var bookmarkStarts = FindBookmark(document, bookmarkName);
            if (bookmarkStarts == null)
                return;
            if (bookmarkValue == null || bookmarkValue.Length == 0)
            {
                bookmarkValue = EmptyPicture;
            }
            foreach (BookmarkStart bookmarkStart in bookmarkStarts)
            {

                a.Graphic graphics = GetRelatedGraphics(bookmarkStart);

                string pictureId = AddImagePartForBookmark(bookmarkStart, bookmarkValue);


                var blib = graphics.GetFirstChild<a.GraphicData>().GetFirstChild<pic.Picture>().GetFirstChild<pic.BlipFill>().GetFirstChild<a.Blip>();
                blib.Embed = pictureId;
            }
        }

        private a.Graphic GetRelatedGraphics(BookmarkStart start)
        {
            var element = start.NextSibling();
            while (NotReachedCorrespondingBookmarkEnd(start, element))
            {
                if (element is Run)
                {
                    var run = (Run)element;
                    var graphics = run.GetFirstChild<Drawing>();
                    if (graphics != null)
                        return GetRelatedGraphics(graphics);
                }
                element = element.NextSibling();
            }
            return null;
        }

        private a.Graphic GetRelatedGraphics(Drawing d)
        {
            if (d.GetFirstChild<wp.Anchor>() != null)
                return d.GetFirstChild<wp.Anchor>().GetFirstChild<a.Graphic>();
            else
                return d.GetFirstChild<wp.Inline>().GetFirstChild<a.Graphic>();
        }
        private string AddImagePartForBookmark(BookmarkStart bookmarkStart, byte[] bookmarkValue)
        {

            OpenXmlElement element = bookmarkStart;
            while ((element is Document || element is Footer) == false)
            {
                element = element.Parent;
            }

            //MainDocumentPart mainPart = document.MainDocumentPart;
            string pictureId = "";
            if (element is Document)
            {
                var documentPart = ((Document)element).MainDocumentPart;
                ImagePart imagePart = documentPart.AddImagePart(ImagePartType.Jpeg);

                imagePart.GetStream().Write(bookmarkValue, 0, (int)bookmarkValue.Length);
                pictureId = documentPart.GetIdOfPart(imagePart);
            }
            else if (element is Footer)
            {
                var footerPart = ((Footer)element).FooterPart;
                ImagePart imagePart = footerPart.AddImagePart(ImagePartType.Jpeg);

                imagePart.GetStream().Write(bookmarkValue, 0, (int)bookmarkValue.Length);
                pictureId = footerPart.GetIdOfPart(imagePart);
            }

            return pictureId;
        }
    }

}