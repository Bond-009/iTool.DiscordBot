using System.Collections.Generic;
using Xunit;

namespace iTool.DiscordBot.Modules.Tests
{
    public class DevModuleTests
    {
        [Theory]
        [MemberData(nameof(TryGetCode_TestData))]
        public void TryGetCodeTests(string input, bool success, string expected)
        {
            Assert.Equal(success, DevModule.TryGetCode(input, out string code));
            Assert.Equal(expected, code.ToString());
        }

        public static IEnumerable<object[]> TryGetCode_TestData()
        {
            yield return new object[]
            {
                @"```cs
return true
```",
                true,
                "return true",
            };
            yield return new object[]
            {
                "`return true`",
                true,
                "return true",
            };
            yield return new object[]
            {
                "return true",
                true,
                "return true",
            };
            yield return new object[]
            {
                @"```cs
return true
",
                false,
                string.Empty,
            };
            yield return new object[]
            {
                "`return true",
                false,
                string.Empty,
            };
            yield return new object[]
            {
                @"```cs return true
```
",
                false,
                string.Empty,
            };
        }
    }
}
