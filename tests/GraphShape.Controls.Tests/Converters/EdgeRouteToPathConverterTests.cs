using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GraphShape.Controls.Converters;
using JetBrains.Annotations;
using NUnit.Framework;

namespace GraphShape.Controls.Tests.Converters
{
    /// <summary>
    /// Tests for <see cref="EdgeRouteToPathConverter"/>.
    /// </summary>
    [TestFixture]
    internal class EdgeRouteToPathConverterTests
    {
        [NotNull]
        private static readonly CultureInfo TestCulture = new CultureInfo("en-US");

        [Test]
        public void Convert()
        {
            var converter = new EdgeRouteToPathConverter();
            Assert.IsNotNull(converter.Convert(null, typeof(PathFigureCollection), null, TestCulture));
            Assert.IsNotNull(
                converter.Convert(
                    new[] { 1.0, 2.0, DependencyProperty.UnsetValue, 12.0, -12.0, 50.0, DependencyProperty.UnsetValue, DependencyProperty.UnsetValue, null }, 
                    typeof(PathFigureCollection),
                    null,
                    TestCulture));
            Assert.IsNotNull(
                converter.Convert(
                    new[]
                    {
                        1.0, 2.0,
                        DependencyProperty.UnsetValue, 12.0,
                        -12.0, 50.0,
                        DependencyProperty.UnsetValue, DependencyProperty.UnsetValue,
                        new[] { new System.Windows.Point(25.0, 25.0), new System.Windows.Point(35.0, 35.0) }
                    },
                    typeof(PathFigureCollection),
                    null,
                    TestCulture));
        }

        [Test]
        public void Convert_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => new EdgeRouteToPathConverter().Convert(new object[] {}, typeof(PathFigureCollection), null, TestCulture));
        }

        [Test]
        public void ConvertBack()
        {
            Assert.Throws<NotSupportedException>(() => new EdgeRouteToPathConverter().ConvertBack(null, null, null, null));
        }
    }
}