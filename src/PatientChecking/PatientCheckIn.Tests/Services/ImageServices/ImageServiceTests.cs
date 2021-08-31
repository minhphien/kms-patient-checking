﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using PatientChecking.Services.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PatientCheckIn.Tests.Services.ImageServices
{
    public class ImageServiceTests
    {
        public static bool IsGuid(string value)
        {
            Guid x;
            return Guid.TryParse(value, out x);
        }

        [Fact]
        public void IsImageFile_Ok()
        {
            // Arrange.
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "This is mock of formfile";
            var fileName = "avatar.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);

            var formFile = fileMock.Object;

            var mockEnvironment = new Mock<IHostingEnvironment>();

            var expected = true;

            //Act
            var imageService = new ImageService(mockEnvironment.Object);
            var actual = imageService.IsImageFile(formFile);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsImageFile_NotOk()
        {
            // Arrange.
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "This is mock of formfile";
            var fileName = "doc.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);

            var formFile = fileMock.Object;

            var mockEnvironment = new Mock<IHostingEnvironment>();

            var expected = false;

            //Act
            var imageService = new ImageService(mockEnvironment.Object);
            var actual = imageService.IsImageFile(formFile);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void SaveImage_Ok()
        {
            // Arrange.
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "This is mock of formfile";
            var fileName = "avatar.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<FileStream>(), CancellationToken.None)).Returns(Task.CompletedTask);

            var formFile = fileMock.Object;

            var mockEnvironment = new Mock<IHostingEnvironment>();
            //...Setup the mock as needed
            mockEnvironment
                .Setup(m => m.WebRootPath)
                .Returns("");

            //Act
            var imageService = new ImageService(mockEnvironment.Object);
            var actual = await imageService.SaveImage(formFile);
            var guid = actual.Substring(7, 36);

            //Assert
            Assert.Contains(formFile.FileName, actual);
            Assert.Contains("/Image/", actual);
            Assert.True(IsGuid(guid));
        }

        [Fact]
        public async void SaveImage_NotOk()
        {
            // Arrange.
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "This is mock of formfile";
            var fileName = "avatar.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);

            var formFile = fileMock.Object;

            var mockEnvironment = new Mock<IHostingEnvironment>();
            //...Setup the mock as needed
            mockEnvironment
                .Setup(m => m.WebRootPath)
                .Returns("");

            //Act
            var imageService = new ImageService(mockEnvironment.Object);

            //Assert
            IOException exception = await Assert.ThrowsAsync<DirectoryNotFoundException>(() => imageService.SaveImage(formFile));
        }
    }
}
