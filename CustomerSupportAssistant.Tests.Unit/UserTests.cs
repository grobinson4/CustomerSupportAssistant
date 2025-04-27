using Xunit;
using CustomerSupportAssistant.Domain.Entities;

namespace CustomerSupportAssistant.Tests.Unit;

public class UserTests
{
    [Fact]
    public void User_Should_Have_Projects_When_Created()
    {
        var user = new User();
        Assert.NotNull(user.Projects);
    }
}
