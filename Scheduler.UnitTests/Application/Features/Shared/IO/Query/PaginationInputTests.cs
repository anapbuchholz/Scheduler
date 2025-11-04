using AutoFixture;
using Scheduler.Application.Features.Shared.IO.Query;

namespace Scheduler.UnitTests.Application.Features.Shared.IO.Query
{
    [TestClass]
    public class PaginationInputTests
    {
        private readonly Fixture _fixture = new();

        [TestMethod]
        public void Create_ShouldAssignProperties_WhenValidValuesAreProvided()
        {
            // Arrange
            int pageNumber = 5;
            int pageSize = 20;
            string searchParam = _fixture.Create<string>();

            // Act
            var pagination = PaginationInput.Create(pageNumber, pageSize, searchParam);

            // Assert
            Assert.AreEqual(pageNumber, pagination.PageNumber);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(searchParam, pagination.SearchParam);
        }

        [TestMethod]
        public void Create_ShouldSetPageNumberToMin_WhenPageNumberIsLessThanMinimum()
        {
            // Arrange
            int invalidPageNumber = -10;
            int validPageSize = 20;

            // Act
            var pagination = PaginationInput.Create(invalidPageNumber, validPageSize, null);

            // Assert
            Assert.AreEqual(1, pagination.PageNumber);
        }

        [TestMethod]
        public void Create_ShouldSetPageSizeToMax_WhenPageSizeExceedsMaximum()
        {
            // Arrange
            int validPageNumber = 1;
            int tooLargePageSize = 500;

            // Act
            var pagination = PaginationInput.Create(validPageNumber, tooLargePageSize, null);

            // Assert
            Assert.AreEqual(100, pagination.PageSize);
        }

        [TestMethod]
        public void Create_ShouldSetPageSizeToMax_WhenPageSizeIsZero()
        {
            // Arrange
            int validPageNumber = 1;
            int zeroPageSize = 0;

            // Act
            var pagination = PaginationInput.Create(validPageNumber, zeroPageSize, null);

            // Assert
            Assert.AreEqual(100, pagination.PageSize);
        }

        [TestMethod]
        public void Create_ShouldAllowNullSearchParam()
        {
            // Arrange
            int pageNumber = 2;
            int pageSize = 10;

            // Act
            var pagination = PaginationInput.Create(pageNumber, pageSize, null);

            // Assert
            Assert.IsNull(pagination.SearchParam);
        }
    }
}
