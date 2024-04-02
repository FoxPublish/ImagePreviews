using NUnit.Framework;
using Serilog.Events;
using ImagePreviews.WebApp.Util;

namespace ImagePreviews.WebApp.Tests.Util
{
    internal class LogUtilTests
    {
        [TestCase("trace")]
        [TestCase("traCe")]
        [TestCase("verbose")]
        [TestCase("vErbose")]
        public void LevelFromString_verbose(string level)
        {
            // Act
            var result = LogUtil.LevelFromString(level);

            // Assert
            Assert.That(result, Is.EqualTo(LogEventLevel.Verbose));
        }

        [TestCase("debug")]
        [TestCase("debuG")]
        public void LevelFromString_debug(string level)
        {
            // Act
            var result = LogUtil.LevelFromString(level);

            // Assert
            Assert.That(result, Is.EqualTo(LogEventLevel.Debug));
        }

        [TestCase("warning")]
        [TestCase("waRning")]
        public void LevelFromString_warning(string level)
        {
            // Act
            var result = LogUtil.LevelFromString(level);

            // Assert
            Assert.That(result, Is.EqualTo(LogEventLevel.Warning));
        }

        [TestCase("error")]
        [TestCase("erRor")]
        public void LevelFromString_error(string level)
        {
            // Act
            var result = LogUtil.LevelFromString(level);

            // Assert
            Assert.That(result, Is.EqualTo(LogEventLevel.Error));
        }

        [TestCase("fatal")]
        [TestCase("fatAl")]
        [TestCase("critical")]
        [TestCase("critIcal")]
        public void LevelFromString_fatal(string level)
        {
            // Act
            var result = LogUtil.LevelFromString(level);

            // Assert
            Assert.That(result, Is.EqualTo(LogEventLevel.Fatal));
        }

        [TestCase("information")]
        [TestCase("informatioN")]
        [TestCase("")]
        [TestCase("warn")]
        public void LevelFromString_information(string level)
        {
            // Act
            var result = LogUtil.LevelFromString(level);

            // Assert
            Assert.That(result, Is.EqualTo(LogEventLevel.Information));
        }
    }
}
