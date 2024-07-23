using System;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class FileAttachmentTests
    {
        [Test(Description = "Attachments must point to a UNC share or to an internet address")]
        public void ConstructorRequiresUncAndHttpPaths()
        {
            try
            {
                new FileAttachment(new Uri(@"c:\test.csv"));
                Assert.Fail("Exception should have eben thrown");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("filePath"));
            }

            new FileAttachment(new Uri(@"\\server\share\file.csv"));
            new FileAttachment(new Uri("http://server/path/file.csv"));
            new FileAttachment(new Uri("https://server/path/file.csv"));
            new FileAttachment(new Uri("file://server/share/file.csv"));
        }

        [Test(Description = "File Scheme URI should convert to UNC style path")]
        public void FileSchemeConvertsToUncFormat()
        {
            var subject = new FileAttachment(new Uri("file://server/share/file.csv"));
            Assert.That(subject.FilePath, Is.EqualTo(@"\\server\share\file.csv"));
        }

        [Test(Description = "HTTP and UNC Scheme URI should stay the same")]
        public void HttpAndUncSchemesShouldBeUnaltered()
        {
            var subject = new FileAttachment(new Uri("https://server/path/file.cs"));
            Assert.That(subject.FilePath, Is.EqualTo("https://server/path/file.cs"));

            subject = new FileAttachment(new Uri(@"\\server\share\file.csv"));
            Assert.That(subject.FilePath, Is.EqualTo(@"\\server\share\file.csv"));
        }
    }
}
