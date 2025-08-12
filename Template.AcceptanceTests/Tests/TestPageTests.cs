using Template.AcceptanceTests.PageObjects;

namespace Template.AcceptanceTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
internal sealed class TestPageTests : BaseTest
{
    private TestPage _testPage = null!;

    [SetUp]
    public new async Task SetUp()
    {
        await base.SetUp();
        _testPage = new TestPage(Page, BaseUrl);
    }

    [Test]
    public async Task TestPageTests_TEXT()
    {
        await Page.GotoAsync(BaseUrl + "/test");

        bool result = await _testPage.HasExpectedTextAsync();

        Assert.That(result, Is.True);
    }
}
