using FluentAssertions;
using Microsoft.Playwright;
using Template.AcceptanceTests.PageObjects;

namespace Template.AcceptanceTests.Tests;

internal sealed class SignalrTests : BaseTest
{
    private SignalRTestPage _signalRPage = null!;
    private readonly List<IRequest> _capturedRequests = new();
    private readonly List<IWebSocket> _capturedWebSockets = new();
    private readonly List<string> _signalRMessages = new();

    [SetUp]
    public new async Task SetUp()
    {
        await base.SetUp();
        _signalRPage = new SignalRTestPage(Page, $"{BaseUrl}/test-signalr");

        SetupNetworkMonitoring();

        await _signalRPage.NavigateToAsync();
        await _signalRPage.WaitForPageLoadAsync();
    }

    private void SetupNetworkMonitoring()
    {
        // Capture all network requests
        Page.Request += (_, request) =>
        {
            _capturedRequests.Add(request);

            // Log SignalR-specific requests (ignore dev server requests)
            if ((request.Url.Contains("hub") || request.Url.Contains("signalr")) &&
                !request.Url.Contains("/@") && !request.Url.Contains("/node_modules"))
            {
                TestContext.Out.WriteLine($"SignalR Request: {request.Method} {request.Url}");
            }
        };

        // Monitor WebSocket connections
        Page.WebSocket += (_, ws) =>
        {
            // Only capture SignalR WebSockets, not dev server WebSockets
            if (!ws.Url.Contains("localhost:3000/?token=") && !ws.Url.Contains("ws://localhost:3000"))
            {
                _capturedWebSockets.Add(ws);
                TestContext.Out.WriteLine($"SignalR WebSocket opened: {ws.Url}");

                // Monitor WebSocket frames for SignalR connections only
                ws.FrameSent += (_, frame) =>
                {
                    string data = frame.Text ?? frame.Binary?.ToString() ?? "";
                    _signalRMessages.Add($"Sent: {data}");
                    TestContext.Out.WriteLine($"SignalR WS Frame Sent: {data}");
                };

                ws.FrameReceived += (_, frame) =>
                {
                    string data = frame.Text ?? frame.Binary?.ToString() ?? "";
                    _signalRMessages.Add($"Received: {data}");
                    TestContext.Out.WriteLine($"SignalR WS Frame Received: {data}");
                };

                ws.Close += (_, _) => TestContext.Out.WriteLine($"SignalR WebSocket closed: {ws.Url}");
            }
            else
            {
                TestContext.Out.WriteLine($"Dev Server WebSocket (ignored): {ws.Url}");
            }
        };

        // Monitor console for SignalR debug messages
        Page.Console += (_, msg) =>
        {
            if (msg.Text.Contains("SignalR") || msg.Text.Contains("Hub"))
            {
                TestContext.Out.WriteLine($"Console [{msg.Type}]: {msg.Text}");
            }
        };
    }

    [Test]
    public async Task SignalrTest()
    {
        await _signalRPage.ConnectAsync();

        await Task.Delay(20000);
    }
}
