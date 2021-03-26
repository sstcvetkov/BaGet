using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NuGet.Versioning;
using Xunit;

namespace BaGet.Core.Tests.Metadata
{
    public class RegistrationBuilderTests
    {
        private readonly Mock<IUrlGenerator> _urlGenerator;

        public RegistrationBuilderTests()
        {
            _urlGenerator = new Mock<IUrlGenerator>();
        }

        [Fact]
        public void TheRegistrationIndexResponseIsSortedByVersion()
        {
            // Arrange
            var packageId = "BaGet.Test";
            var authors = new string[] { "test" };

            var packages = new List<Package>
            {
                GetTestPackage(packageId, "3.1.0", DateTime.Now),
                GetTestPackage(packageId, "10.0.5", DateTime.Now),
                GetTestPackage(packageId, "3.2.0", DateTime.Now),
                GetTestPackage(packageId, "3.1.0-pre", DateTime.Now),
                GetTestPackage(packageId, "1.0.0-beta1", DateTime.Now),
                GetTestPackage(packageId, "1.0.0", DateTime.Now),
            };

            var registration = new PackageRegistration(packageId, packages);

            var registrationBuilder = new RegistrationBuilder(_urlGenerator.Object);

            // Act
            var response = registrationBuilder.BuildIndex(registration);

            // Assert
            Assert.Equal(packages.Count, response.Pages[0].ItemsOrNull.Count);
            var index = 0;
            foreach (var package in packages.OrderBy(p => p.Version))
            {
                Assert.Equal(package.Version.ToFullString(), response.Pages[0].ItemsOrNull[index++].PackageMetadata.Version);
            }
        }

        /// <summary>
        /// Create a fake <see cref="Package"></see> with the minimum metadata needed by the <see cref="RegistrationBuilder"></see>.
        /// </summary>
        private Package GetTestPackage(string packageId, string version, DateTime published = default)
        {
            return new Package
            {
                Id = packageId,
                Authors = new string[] { "test" },
                PackageTypes = new List<PackageType> { new PackageType { Name = "test" } },
                Dependencies = new List<PackageDependency> { },
                Version = new NuGetVersion(version),
                Published = published
            };
        }
    }
}
