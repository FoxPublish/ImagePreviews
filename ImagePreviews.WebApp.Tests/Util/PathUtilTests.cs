using NUnit.Framework;
using ImagePreviews.WebApp.Util;

namespace ImagePreviews.WebApp.Tests.Util
{
    internal class PathUtilTests
    {
        [Test]
        public void Combine_dirNoTrailing_fileNoLeading()
        {
            // Arrange
            var dir = "c:\\test";
            var file = "100/img.png";

            // Act
            var result = PathUtil.Combine(dir, file);

            // Assert
            Assert.That(result, Is.EqualTo("c:\\test\\100/img.png"));
        }

        [Test]
        public void Combine_dirNoTrailing_fileLeadingForward()
        {
            // Arrange
            var dir = "c:\\test";
            var file = "/100/img.png";

            // Act
            var result = PathUtil.Combine(dir, file);

            // Assert
            Assert.That(result, Is.EqualTo("c:\\test\\100/img.png"));
        }

        [Test]
        public void Combine_dirNoTrailing_fileLeadingBackward()
        {
            // Arrange
            var dir = "c:\\test";
            var file = "\\100/img.png";

            // Act
            var result = PathUtil.Combine(dir, file);

            // Assert
            Assert.That(result, Is.EqualTo("c:\\test\\100/img.png"));
        }

        [Test]
        public void Combine_dirTrailingBackward_fileNoLeading()
        {
            // Arrange
            var dir = "c:\\test\\";
            var file = "100/img.png";

            // Act
            var result = PathUtil.Combine(dir, file);

            // Assert
            Assert.That(result, Is.EqualTo("c:\\test\\100/img.png"));
        }

        [Test]
        public void Combine_dirTrailingBackward_fileLeadingForward()
        {
            // Arrange
            var dir = "c:\\test\\";
            var file = "/100/img.png";

            // Act
            var result = PathUtil.Combine(dir, file);

            // Assert
            Assert.That(result, Is.EqualTo("c:\\test\\100/img.png"));
        }

        [Test]
        public void Combine_dirTrailingBackward_fileLeadingBackward()
        {
            // Arrange
            var dir = "c:\\test\\";
            var file = "\\100/img.png";

            // Act
            var result = PathUtil.Combine(dir, file);

            // Assert
            Assert.That(result, Is.EqualTo("c:\\test\\100/img.png"));
        }

        [Test]
        public void Combine_dirTrailingForward_fileNoLeading()
        {
            // Arrange
            var dir = "c:\\test/";
            var file = "100/img.png";

            // Act
            var result = PathUtil.Combine(dir, file);

            // Assert
            Assert.That(result, Is.EqualTo("c:\\test/100/img.png"));
        }

        [Test]
        public void Combine_dirTrailingForward_fileLeadingForward()
        {
            // Arrange
            var dir = "c:\\test/";
            var file = "/100/img.png";

            // Act
            var result = PathUtil.Combine(dir, file);

            // Assert
            Assert.That(result, Is.EqualTo("c:\\test/100/img.png"));
        }

        [Test]
        public void Combine_dirTrailingForward_fileLeadingBackward()
        {
            // Arrange
            var dir = "c:\\test/";
            var file = "\\100/img.png";

            // Act
            var result = PathUtil.Combine(dir, file);

            // Assert
            Assert.That(result, Is.EqualTo("c:\\test/100/img.png"));
        }
    }
}
