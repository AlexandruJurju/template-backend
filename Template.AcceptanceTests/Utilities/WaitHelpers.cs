using Microsoft.Playwright;

namespace Template.AcceptanceTests.Utilities;

public static class WaitHelpers
{
    public static async Task WaitForAngularAsync(IPage page)
    {
        await page.WaitForFunctionAsync(@"
            () => {
                if (window.getAllAngularTestabilities) {
                    return window.getAllAngularTestabilities()
                        .every(t => t.isStable());
                }
                return true;
            }
        ");
    }

    public static async Task<bool> WaitForElementAsync(IPage page, string selector, int timeoutMs = 5000)
    {
        try
        {
            await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeoutMs
            });
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public static async Task RetryAsync(Func<Task> action, int maxAttempts = 3, int delayMs = 1000)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            try
            {
                await action();
                return;
            }
            catch when (i < maxAttempts - 1)
            {
                await Task.Delay(delayMs);
            }
        }
    }
}
