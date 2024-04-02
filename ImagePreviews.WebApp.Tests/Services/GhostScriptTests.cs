using NUnit.Framework;
using Moq;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ImagePreviews.WebApp.Services;
using ImagePreviews.WebApp.Settings;
using ImagePreviews.WebApp.Wrappers;

namespace ImagePreviews.WebApp.Tests.Services
{
    internal class GhostScriptTests
    {
        private IGhostScript _ghostScript;
        private Mock<IProcessWrapper> _processMock;

        [SetUp]
        public void Setup()
        {
            var loggerMock = new Mock<ILogger<GhostScript>>();
            _processMock = new Mock<IProcessWrapper>();
            var gsSettings = new GhostScriptSettings()
            {
                PathExecutable = "folder/gsProgram.exe",
                TimeoutMs = 1234
            };
            var imgSettings = new ImagePathSettings()
            {
                DataShareBasePath = "dataShare",
                TempPath = "temp"
            };
            _ghostScript = new GhostScript(loggerMock.Object, _processMock.Object, gsSettings, imgSettings);
        }

        [Test]
        public void ConvertPageToImage_png_success()
        {
            // Arrange
            string path = "/test/100/document.pdf";
            string page = "10";
            string format = "png";
            var expectedArgument = " -sDEVICE=png16m -dFirstPage=10 -dLastPage=10 -sOutputFile=\"temp\\document_10.png\" dataShare\\test/100/document.pdf";
            _processMock.Setup(p => p.Run(It.IsAny<ProcessStartInfo>(), It.IsAny<int>())).Returns(true);

            // Act
            var (filename, outputPath) = _ghostScript.ConvertPageToImage(path, page, format);

            // Assert
            Assert.That(filename, Is.EqualTo("document_10.png"));
            Assert.That(outputPath, Is.EqualTo("temp\\document_10.png"));
            _processMock.Verify(p => p.Run(It.Is<ProcessStartInfo>(p => p.Arguments == expectedArgument), 1234));
        }

        [Test]
        public void ConvertPageToImage_jpg_success()
        {
            // Arrange
            string path = "/test/100/document.pdf";
            string page = "10";
            string format = "jpg";
            var expectedArgument = " -sDEVICE=jpeg -dFirstPage=10 -dLastPage=10 -sOutputFile=\"temp\\document_10.jpg\" dataShare\\test/100/document.pdf";
            _processMock.Setup(p => p.Run(It.IsAny<ProcessStartInfo>(), It.IsAny<int>())).Returns(true);

            // Act
            var (filename, outputPath) = _ghostScript.ConvertPageToImage(path, page, format);

            // Assert
            Assert.That(filename, Is.EqualTo("document_10.jpg"));
            Assert.That(outputPath, Is.EqualTo("temp\\document_10.jpg"));
            _processMock.Verify(p => p.Run(It.Is<ProcessStartInfo>(p => p.Arguments == expectedArgument), 1234));
        }

        [Test]
        public void ConvertPageToImage_jpeg_success()
        {
            // Arrange
            string path = "/test/100/document.pdf";
            string page = "10";
            string format = "jpeg";
            var expectedArgument = " -sDEVICE=jpeg -dFirstPage=10 -dLastPage=10 -sOutputFile=\"temp\\document_10.jpeg\" dataShare\\test/100/document.pdf";
            _processMock.Setup(p => p.Run(It.IsAny<ProcessStartInfo>(), It.IsAny<int>())).Returns(true);

            // Act
            var (filename, outputPath) = _ghostScript.ConvertPageToImage(path, page, format);

            // Assert
            Assert.That(filename, Is.EqualTo("document_10.jpeg"));
            Assert.That(outputPath, Is.EqualTo("temp\\document_10.jpeg"));
            _processMock.Verify(p => p.Run(It.Is<ProcessStartInfo>(p => p.Arguments == expectedArgument), 1234));
        }

        [Test]
        public void ConvertPageToImage_invalid_format()
        {
            // Arrange
            string path = "/test/100/document.pdf";
            string page = "10";
            string format = "tiff";
            var expectedArgument = " tiff -dFirstPage=10 -dLastPage=10 -sOutputFile=\"temp\\document_10.tiff\" dataShare\\test/100/document.pdf";
            _processMock.Setup(p => p.Run(It.IsAny<ProcessStartInfo>(), It.IsAny<int>())).Returns(true);

            // Act
            var (filename, outputPath) = _ghostScript.ConvertPageToImage(path, page, format);

            // Assert
            Assert.That(filename, Is.EqualTo("document_10.tiff"));
            Assert.That(outputPath, Is.EqualTo("temp\\document_10.tiff"));
            _processMock.Verify(p => p.Run(It.Is<ProcessStartInfo>(p => p.Arguments == expectedArgument), 1234));
        }

        [Test]
        public void ConvertPageToImage_ghostscript_fail()
        {
            // Arrange
            string path = "/test/100/document.pdf";
            string page = "10";
            string format = "png";
            var expectedArgument = " -sDEVICE=png16m -dFirstPage=10 -dLastPage=10 -sOutputFile=\"temp\\document_10.png\" dataShare\\test/100/document.pdf";
            _processMock.Setup(p => p.Run(It.IsAny<ProcessStartInfo>(), It.IsAny<int>())).Returns(false);

            // Act
            var (filename, outputPath) = _ghostScript.ConvertPageToImage(path, page, format);

            // Assert
            Assert.That(filename, Is.Empty);
            Assert.That(outputPath, Is.Empty);
            _processMock.Verify(p => p.Run(It.Is<ProcessStartInfo>(p => p.Arguments == expectedArgument), 1234));
        }

        [Test]
        public void ConvertPageToImage_path_empty()
        {
            // Arrange
            string page = "10";
            string format = "png";
            var expectedArgument = " -sDEVICE=png16m -dFirstPage=10 -dLastPage=10 -sOutputFile=\"temp\\_10.png\" dataShare";
            _processMock.Setup(p => p.Run(It.IsAny<ProcessStartInfo>(), It.IsAny<int>())).Returns(true);

            // Act
            var (filename, outputPath) = _ghostScript.ConvertPageToImage(string.Empty, page, format);

            // Assert
            Assert.That(filename, Is.EqualTo("_10.png"));
            Assert.That(outputPath, Is.EqualTo("temp\\_10.png"));
            _processMock.Verify(p => p.Run(It.Is<ProcessStartInfo>(p => p.Arguments == expectedArgument), 1234));
        }

        [Test]
        public void ConvertPageToImage_page_empty()
        {
            // Arrange
            string path = "/test/100/document.pdf";
            string format = "png";
            var expectedArgument = " -sDEVICE=png16m -dFirstPage= -dLastPage= -sOutputFile=\"temp\\document_.png\" dataShare\\test/100/document.pdf";
            _processMock.Setup(p => p.Run(It.IsAny<ProcessStartInfo>(), It.IsAny<int>())).Returns(true);

            // Act
            var (filename, outputPath) = _ghostScript.ConvertPageToImage(path, string.Empty, format);

            // Assert
            Assert.That(filename, Is.EqualTo("document_.png"));
            Assert.That(outputPath, Is.EqualTo("temp\\document_.png"));
            _processMock.Verify(p => p.Run(It.Is<ProcessStartInfo>(p => p.Arguments == expectedArgument), 1234));
        }

        [Test]
        public void ConvertPageToImage_format_empty()
        {
            // Arrange
            string path = "/test/100/document.pdf";
            string page = "10";
            var expectedArgument = "  -dFirstPage=10 -dLastPage=10 -sOutputFile=\"temp\\document_10.\" dataShare\\test/100/document.pdf";
            _processMock.Setup(p => p.Run(It.IsAny<ProcessStartInfo>(), It.IsAny<int>())).Returns(true);

            // Act
            var (filename, outputPath) = _ghostScript.ConvertPageToImage(path, page, string.Empty);

            // Assert
            Assert.That(filename, Is.EqualTo("document_10."));
            Assert.That(outputPath, Is.EqualTo("temp\\document_10."));
            _processMock.Verify(p => p.Run(It.Is<ProcessStartInfo>(p => p.Arguments == expectedArgument), 1234));
        }
    }
}
