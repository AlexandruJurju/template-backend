using Microsoft.Playwright;
using Template.AcceptanceTests.Utilities;

namespace Template.AcceptanceTests.PageObjects;

public class SignalRTestPage(IPage page, string pageUrl) : BasePage(page, pageUrl)
{
    // Selectors
    private const string StatusSelector = ".status span";
    private const string ConnectButton = "button:has-text('Connect')";
    private const string DisconnectButton = "button:has-text('Disconnect')";
    private const string RequestNumbersButton = "button:has-text('Request Numbers')";
    private const string JoinGroupButton = "button:has-text('Join Test Group')";
    private const string LeaveGroupButton = "button:has-text('Leave Test Group')";
    private const string LatestNumbersSection = ".latest-numbers";
    private const string NumberElements = ".number";
    private const string ConnectedStatus = "span.connected";
    private const string DisconnectedStatus = "span.disconnected";

    // Connection Management
    public async Task<bool> ConnectAsync()
    {
        await Page.ClickAsync(ConnectButton);
        return await WaitForConnectionAsync();
    }

    public async Task<bool> DisconnectAsync()
    {
        await Page.ClickAsync(DisconnectButton);
        return await WaitForDisconnectionAsync();
    }

    public async Task<bool> WaitForConnectionAsync(int timeout = 10000)
    {
        try
        {
            await Page.WaitForSelectorAsync(ConnectedStatus, new PageWaitForSelectorOptions { Timeout = timeout });
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task<bool> WaitForDisconnectionAsync(int timeout = 5000)
    {
        try
        {
            await Page.WaitForSelectorAsync(DisconnectedStatus, new PageWaitForSelectorOptions { Timeout = timeout });
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    // Status Checks
    public async Task<string> GetConnectionStatusAsync()
    {
        return await Page.TextContentAsync(StatusSelector) ?? "Unknown";
    }

    public async Task<bool> IsConnectedAsync()
    {
        string status = await GetConnectionStatusAsync();
        return status == "Connected";
    }

    public async Task<bool> IsDisconnectedAsync()
    {
        string status = await GetConnectionStatusAsync();
        return status == "Disconnected";
    }

    // Actions
    public async Task RequestRandomNumbersAsync()
    {
        await Page.ClickAsync(RequestNumbersButton);
    }

    public async Task JoinTestGroupAsync()
    {
        await Page.ClickAsync(JoinGroupButton);
    }

    public async Task LeaveTestGroupAsync()
    {
        await Page.ClickAsync(LeaveGroupButton);
    }

    // Data Retrieval
    public async Task<List<string>> GetRandomNumbersAsync()
    {
        var numbers = new List<string>();
        IReadOnlyList<IElementHandle> numberElements = await Page.QuerySelectorAllAsync(NumberElements);

        foreach (IElementHandle element in numberElements)
        {
            string? text = await element.TextContentAsync();
            if (!string.IsNullOrEmpty(text))
            {
                numbers.Add(text);
            }
        }

        return numbers;
    }

    public async Task<bool> HasRandomNumbersAsync()
    {
        return await IsElementVisibleAsync(LatestNumbersSection);
    }


    // Button State Checks
    public async Task<bool> IsConnectButtonEnabledAsync()
    {
        return !await Page.IsDisabledAsync(ConnectButton);
    }

    public async Task<bool> IsDisconnectButtonEnabledAsync()
    {
        return !await Page.IsDisabledAsync(DisconnectButton);
    }

    public async Task<bool> IsRequestNumbersButtonEnabledAsync()
    {
        return !await Page.IsDisabledAsync(RequestNumbersButton);
    }

    // Wait Helpers
    public async Task WaitForLogEntryAsync(string logType, string containsText, int timeout = 5000)
    {
        string selector = $".log-{logType}:has-text('{containsText}')";
        await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = timeout });
    }

    public async Task WaitForNumbersAsync(int timeout = 5000)
    {
        await Page.WaitForSelectorAsync(LatestNumbersSection, new PageWaitForSelectorOptions { Timeout = timeout });
    }

    // Network Monitoring Helpers
    public async Task<List<IRequest>> CaptureSignalRRequestsAsync(Func<Task> action)
    {
        var requests = new List<IRequest>();

        void RequestHandler(object? sender, IRequest request)
        {
            if (request.Url.Contains("hub") || request.Url.Contains("signalr"))
            {
                requests.Add(request);
            }
        }

        Page.Request += RequestHandler;

        try
        {
            await action();
            await Page.WaitForTimeoutAsync(1000); // Give time for requests to complete
        }
        finally
        {
            Page.Request -= RequestHandler;
        }

        return requests;
    }

    public async Task<List<IWebSocket>> MonitorWebSocketsAsync(Func<Task> action)
    {
        var webSockets = new List<IWebSocket>();

        void WebSocketHandler(object? sender, IWebSocket ws) => webSockets.Add(ws);

        Page.WebSocket += WebSocketHandler;

        try
        {
            await action();
            await Page.WaitForTimeoutAsync(1000); // Give time for WebSocket to establish
        }
        finally
        {
            Page.WebSocket -= WebSocketHandler;
        }

        return webSockets;
    }
}
