using NUnit.Framework;
using Moq;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ImagePreviews.WebApp.Services;
using ImagePreviews.WebApp.Middleware;

namespace ImagePreviews.WebApp.Tests.Middleware
{
    internal class PdfMiddlewareTests
    {
        private RequestDelegate _nextMiddlewareMock;
        private DefaultHttpContext _httpContextMock;
        private Mock<ILogger<PdfMiddleware>> _loggerMock;
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<IGhostScript> _ghostScriptMock;
        
        private PdfMiddleware _middleware;

        [SetUp]
        public void Setup()
        {
            _nextMiddlewareMock = (HttpContext) => Task.CompletedTask;
            _httpContextMock = new DefaultHttpContext();
            _loggerMock = new Mock<ILogger<PdfMiddleware>>();
            _fileSystemMock = new Mock<IFileSystem>();
            _ghostScriptMock = new Mock<IGhostScript>();

            _middleware = new PdfMiddleware(_nextMiddlewareMock, _loggerMock.Object);
        }

        [Test]
        public async Task EmptyRequest()
        {
            // Act
            await _middleware.Invoke(_httpContextMock, _fileSystemMock.Object, _ghostScriptMock.Object);

            // Assert
            Assert.That(StatusCodes.Status200OK, Is.EqualTo(_httpContextMock.Response.StatusCode));
            _ghostScriptMock.Verify(g => g.ConvertPageToImage(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()), Times.Never);
        }

        [Test]
        public async Task Image()
        {
            // Arrange
            var path = "/test.png";
            _httpContextMock.Request.Path = new PathString(path);

            // Act
            await _middleware.Invoke(_httpContextMock, _fileSystemMock.Object, _ghostScriptMock.Object);

            // Assert
            Assert.That(StatusCodes.Status200OK, Is.EqualTo(_httpContextMock.Response.StatusCode));
            Assert.That(new PathString("/test.png"), Is.EqualTo(_httpContextMock.Request.Path));
            _ghostScriptMock.Verify(g => g.ConvertPageToImage(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()), Times.Never);
        }

        [Test]
        public async Task Pdf_DefaultPage()
        {
            // Arrange
            var path = "/test.pdf";
            var defaultPage = "1";
            var imageFormat = "png";
            _httpContextMock.Request.Path = new PathString(path);
            _httpContextMock.Request.QueryString = new QueryString("?format=png");
            _ghostScriptMock.Setup(g => g.ConvertPageToImage(path, defaultPage, imageFormat))
                .Returns(("test_1.png", "temp/test_1.png"));
            _fileSystemMock.Setup(f => f.File.Exists("temp/test_1.png")).Returns(true);
            _fileSystemMock.Setup(f => f.File.Delete("temp/test_1.png"));

            // Act
            await _middleware.Invoke(_httpContextMock, _fileSystemMock.Object, _ghostScriptMock.Object);

            // Assert
            Assert.That(StatusCodes.Status200OK, Is.EqualTo(_httpContextMock.Response.StatusCode));
            Assert.That(new PathString("/temp/test_1.png"), Is.EqualTo(_httpContextMock.Request.Path));
            _ghostScriptMock.Verify(g => g.ConvertPageToImage(path, defaultPage, imageFormat), Times.Once);
        }

        [Test]
        public async Task Pdf_WithPage()
        {
            // Arrange
            var path = "/test.pdf";
            var page = "13";
            var imageFormat = "png";
            _httpContextMock.Request.Path = new PathString(path);
            _httpContextMock.Request.QueryString = new QueryString("?format=png&page=13");
            _ghostScriptMock.Setup(g => g.ConvertPageToImage(path, page, imageFormat))
                .Returns(("test_13.png", "temp/test_13.png"));
            _fileSystemMock.Setup(f => f.File.Exists("temp/test_13.png")).Returns(true);
            _fileSystemMock.Setup(f => f.File.Delete("temp/test_13.png"));

            // Act
            await _middleware.Invoke(_httpContextMock, _fileSystemMock.Object, _ghostScriptMock.Object);

            // Assert
            Assert.That(StatusCodes.Status200OK, Is.EqualTo(_httpContextMock.Response.StatusCode));
            Assert.That(new PathString("/temp/test_13.png"), Is.EqualTo(_httpContextMock.Request.Path));
            _ghostScriptMock.Verify(g => g.ConvertPageToImage(path, page, imageFormat), Times.Once);
        }

        [Test]
        public async Task Pdf_NoQuery()
        {
            // Arrange
            var path = "/test.pdf";
            _httpContextMock.Request.Path = new PathString(path);

            // Act
            await _middleware.Invoke(_httpContextMock, _fileSystemMock.Object, _ghostScriptMock.Object);

            // Assert
            Assert.That(StatusCodes.Status200OK, Is.EqualTo(_httpContextMock.Response.StatusCode));
            _ghostScriptMock.Verify(g => g.ConvertPageToImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Pdf_NoFormatInQuery()
        {
            // Arrange
            var path = "/test.pdf";
            _httpContextMock.Request.Path = new PathString(path);
            _httpContextMock.Request.QueryString = new QueryString("?page=13&width=200");

            // Act
            await _middleware.Invoke(_httpContextMock, _fileSystemMock.Object, _ghostScriptMock.Object);

            // Assert
            Assert.That(StatusCodes.Status200OK, Is.EqualTo(_httpContextMock.Response.StatusCode));
            _ghostScriptMock.Verify(g => g.ConvertPageToImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Pdf_GhostScriptReturnEmpty()
        {
            // Arrange
            var path = "/test.pdf";
            var page = "13";
            var imageFormat = "png";
            _httpContextMock.Request.Path = new PathString(path);
            _httpContextMock.Request.QueryString = new QueryString("?format=png&page=13");
            _ghostScriptMock.Setup(g => g.ConvertPageToImage(path, page, imageFormat))
                .Returns((String.Empty, String.Empty));

            // Act
            await _middleware.Invoke(_httpContextMock, _fileSystemMock.Object, _ghostScriptMock.Object);

            // Assert
            Assert.That(StatusCodes.Status200OK, Is.EqualTo(_httpContextMock.Response.StatusCode));
            Assert.That(new PathString("/temp/"), Is.EqualTo(_httpContextMock.Request.Path));
            _ghostScriptMock.Verify(g => g.ConvertPageToImage(path, page, imageFormat), Times.Once);
        }
    }
}
