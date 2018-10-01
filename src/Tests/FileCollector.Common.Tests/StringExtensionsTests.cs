using Xunit;

namespace FileCollector.Common.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("price_*.xlsx")]
        [InlineData("pricE_*.xlsx")]
        [InlineData("*.xlsx")]
        [InlineData("price_*")]
        [InlineData("price_*.2018*")]
        [InlineData("price_12.08.2018.xlsx")]
        [InlineData("Price_12.08.2018.xlsx")]
        public void Test1(string pattern)
        {
            Assert.True("price_12.08.2018.xlsx".MatchPattern(pattern));
        }
    }
}
