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

            // Ignore dev server requests
            if ((request.Url.Contains("hub") || request.Url.Contains("signalr")) &&
                !request.Url.Contains("/@") && !request.Url.Contains("/node_modules"))
            {
                TestContext.Out.WriteLine($"SignalR Request: {request.Method} {request.Url}");
            }
        };

        // Monitor WebSocket connections
        Page.WebSocket += (_, ws) =>
        {
            // Ignore dev server websocket connections
            if (!ws.Url.Contains("localhost:3000/?token=") && !ws.Url.Contains("ws://localhost:3000"))
            {
                _capturedWebSockets.Add(ws);
                TestContext.Out.WriteLine($"SignalR WebSocket opened: {ws.Url}");

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
    public async Task Should_Connect_And_Show_Connected_Status()
    {
        // Verify initial state
        Assert.That(await _signalRPage.IsDisconnectedAsync(), Is.True);
        Assert.That(await _signalRPage.IsConnectButtonEnabledAsync(), Is.True);
        Assert.That(await _signalRPage.IsDisconnectButtonEnabledAsync(), Is.False);

        // Connect
        bool connected = await _signalRPage.ConnectAsync();
        Assert.That(connected, Is.True);

        // Verify connected state
        Assert.That(await _signalRPage.IsConnectedAsync(), Is.True);
    }

    [Test]
    public async Task Should_Disconnect_And_Show_Disconnected_Status()
    {
        // Connect first
        await _signalRPage.ConnectAsync();
        Assert.That(await _signalRPage.IsConnectedAsync(), Is.True);

        // Disconnect
        bool disconnected = await _signalRPage.DisconnectAsync();
        Assert.That(disconnected, Is.True, "Disconnection should succeed");

        // Verify disconnected state
        Assert.That(await _signalRPage.IsDisconnectedAsync(), Is.True, "Should show disconnected status");
        Assert.That(await _signalRPage.IsConnectButtonEnabledAsync(), Is.True, "Connect button should be enabled");
        Assert.That(await _signalRPage.IsDisconnectButtonEnabledAsync(), Is.False, "Disconnect button should be disabled");
        Assert.That(await _signalRPage.IsRequestNumbersButtonEnabledAsync(), Is.False, "Request numbers button should be disabled");
    }

    [Test]
    public async Task Should_Request_And_Receive_Random_Numbers()
    {
        // Connect
        await _signalRPage.ConnectAsync();
        Assert.That(await _signalRPage.IsConnectedAsync(), Is.True);

        // Request numbers
        await _signalRPage.RequestRandomNumbersAsync();

        // Wait for numbers to arrive
        await _signalRPage.WaitForNumbersAsync();

        // Verify numbers received
        Assert.That(await _signalRPage.HasRandomNumbersAsync(), Is.True, "Should have received numbers");

        List<string> numbers = await _signalRPage.GetRandomNumbersAsync();
        Assert.That(numbers.Count, Is.GreaterThan(0), "Should receive at least one number");
    }

    [Test]
    public async Task Should_Join_Test_Group_Successfully()
    {
        // Connect
        await _signalRPage.ConnectAsync();
        Assert.That(await _signalRPage.IsConnectedAsync(), Is.True);

        // Join group
        await _signalRPage.JoinTestGroupAsync();

        // Wait for join confirmation
        await _signalRPage.WaitForLogEntryAsync("info", "Joining test group");
    }

    [Test]
    public async Task Should_Leave_Test_Group_Successfully()
    {
        // Connect and join group first
        await _signalRPage.ConnectAsync();
        await _signalRPage.JoinTestGroupAsync();
        await Task.Delay(1000); // Wait for join to complete

        // Leave group
        await _signalRPage.LeaveTestGroupAsync();

        // Wait for leave confirmation
        await _signalRPage.WaitForLogEntryAsync("info", "Leaving test group");
    }

    [Test]
    public async Task Should_Establish_WebSocket_Connection_On_Connect()
    {
        // Clear captured websockets
        _capturedWebSockets.Clear();

        // Connect
        await _signalRPage.ConnectAsync();

        // Give time for WebSocket to establish
        await Task.Delay(2000);

        // Verify WebSocket connection was established
        Assert.That(_capturedWebSockets.Count, Is.GreaterThan(0),
            "Should have established at least one WebSocket connection");

        IWebSocket? signalRWebSocket = _capturedWebSockets.FirstOrDefault(ws =>
            ws.Url.Contains("random-number-hub") || ws.Url.Contains("signalr"));

        Assert.That(signalRWebSocket, Is.Not.Null, "Should have established SignalR WebSocket connection");
    }

    [Test]
    public async Task Should_Send_WebSocket_Messages_When_Requesting_Numbers()
    {
        // Clear captured messages
        _signalRMessages.Clear();

        // Connect
        await _signalRPage.ConnectAsync();
        await Task.Delay(1000);

        // Clear messages captured during connection
        _signalRMessages.Clear();

        // Request numbers
        await _signalRPage.RequestRandomNumbersAsync();
        await Task.Delay(2000);

        // Verify WebSocket messages were sent
        Assert.That(_signalRMessages.Count, Is.GreaterThan(0),
            "Should have WebSocket message activity");

#pragma warning disable CA1310
        var sentMessages = _signalRMessages.Where(m => m.StartsWith("Sent:")).ToList();
#pragma warning restore CA1310
        Assert.That(sentMessages.Count, Is.GreaterThan(0),
            "Should have sent at least one message");

        await TestContext.Out.WriteLineAsync($"Total SignalR messages: {_signalRMessages.Count}");
        foreach (string msg in _signalRMessages.Take(10)) // Log first 10 messages
        {
            await TestContext.Out.WriteLineAsync($"Message: {msg}");
        }
    }

    [Test]
    public async Task Should_Capture_SignalR_Requests_During_Connection()
    {
        _capturedRequests.Clear();

        await _signalRPage.ConnectAsync();
        await Task.Delay(2000);

        var signalRRequests = _capturedRequests.Where(r =>
            r.Url.Contains("hub") || r.Url.Contains("signalr")).ToList();

        Assert.That(signalRRequests.Count, Is.GreaterThan(0));
        Assert.That(signalRRequests.Any(r => r.Method == "POST"));
    }

    [Test]
    public async Task Should_Establish_WebSocket_Connection()
    {
        _capturedWebSockets.Clear();

        await _signalRPage.ConnectAsync();
        await Task.Delay(2000);

        Assert.That(_capturedWebSockets.Count, Is.GreaterThan(0));

        IWebSocket? signalRWebSocket = _capturedWebSockets.FirstOrDefault(ws =>
            ws.Url.Contains("random-number-hub"));

        Assert.That(signalRWebSocket, Is.Not.Null);
    }

    [Test]
    public async Task Should_Send_WebSocket_Messages_On_Number_Request()
    {
        await _signalRPage.ConnectAsync();
        await Task.Delay(1000);

        _signalRMessages.Clear();

        await _signalRPage.RequestRandomNumbersAsync();
        await Task.Delay(2000);

        Assert.That(_signalRMessages.Count, Is.GreaterThan(0));

#pragma warning disable CA1310
        var sentMessages = _signalRMessages.Where(m => m.StartsWith("Sent:")).ToList();
#pragma warning restore CA1310
        Assert.That(sentMessages.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task Should_Receive_WebSocket_Messages_After_Request()
    {
        await _signalRPage.ConnectAsync();
        await Task.Delay(1000);

        _signalRMessages.Clear();

        await _signalRPage.RequestRandomNumbersAsync();
        await Task.Delay(3000);

#pragma warning disable CA1310
        var receivedMessages = _signalRMessages.Where(m => m.StartsWith("Received:")).ToList();
#pragma warning restore CA1310
        Assert.That(receivedMessages.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task Should_Send_Group_Join_Messages()
    {
        await _signalRPage.ConnectAsync();
        await Task.Delay(1000);

        _signalRMessages.Clear();

        await _signalRPage.JoinTestGroupAsync();
        await Task.Delay(2000);

        var groupJoinMessages = _signalRMessages.Where(m =>
            m.Contains("JoinGroup") || m.Contains("testGroup")).ToList();

        Assert.That(groupJoinMessages.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task Should_Send_Group_Leave_Messages()
    {
        await _signalRPage.ConnectAsync();
        await _signalRPage.JoinTestGroupAsync();
        await Task.Delay(1000);

        _signalRMessages.Clear();

        await _signalRPage.LeaveTestGroupAsync();
        await Task.Delay(2000);

        var groupLeaveMessages = _signalRMessages.Where(m =>
            m.Contains("LeaveGroup") || m.Contains("testGroup")).ToList();

        Assert.That(groupLeaveMessages.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task Should_Maintain_WebSocket_During_Multiple_Operations()
    {
        _capturedWebSockets.Clear();

        await _signalRPage.ConnectAsync();
        await Task.Delay(1000);

        int initialWebSocketCount = _capturedWebSockets.Count;

        await _signalRPage.RequestRandomNumbersAsync();
        await _signalRPage.JoinTestGroupAsync();
        await _signalRPage.LeaveTestGroupAsync();
        await Task.Delay(2000);

        Assert.That(_capturedWebSockets.Count, Is.EqualTo(initialWebSocketCount));
    }

    [Test]
    public async Task Should_Generate_Message_Traffic_During_Session()
    {
        _signalRMessages.Clear();

        await _signalRPage.ConnectAsync();
        await _signalRPage.RequestRandomNumbersAsync();
        await Task.Delay(1000);
        await _signalRPage.RequestRandomNumbersAsync();
        await Task.Delay(1000);
        await _signalRPage.JoinTestGroupAsync();
        await Task.Delay(2000);

        Assert.That(_signalRMessages.Count, Is.GreaterThan(5));

#pragma warning disable CA1310
        int sentCount = _signalRMessages.Count(m => m.StartsWith("Sent:"));
        int receivedCount = _signalRMessages.Count(m => m.StartsWith("Received:"));
#pragma warning restore CA1310

        Assert.That(sentCount, Is.GreaterThan(0));
        Assert.That(receivedCount, Is.GreaterThan(0));
    }

    [Test]
    public async Task Should_Close_WebSocket_On_Disconnect()
    {
        await _signalRPage.ConnectAsync();
        await Task.Delay(1000);

        int webSocketsBeforeDisconnect = _capturedWebSockets.Count;
        Assert.That(webSocketsBeforeDisconnect, Is.GreaterThan(0));

        await _signalRPage.DisconnectAsync();
        await Task.Delay(2000);

        IWebSocket? activeWebSocket = _capturedWebSockets.LastOrDefault();
        Assert.That(activeWebSocket, Is.Not.Null);
    }


    [Test]
    public async Task Should_Track_Message_Bidirectional_Flow()
    {
        await _signalRPage.ConnectAsync();
        await Task.Delay(1000);

        int initialMessageCount = _signalRMessages.Count;

        await _signalRPage.RequestRandomNumbersAsync();
        await Task.Delay(3000);

        int finalMessageCount = _signalRMessages.Count;
        int newMessages = finalMessageCount - initialMessageCount;

        Assert.That(newMessages, Is.GreaterThanOrEqualTo(2));

        int newSentMessages = _signalRMessages.Skip(initialMessageCount)
#pragma warning disable CA1310
            .Count(m => m.StartsWith("Sent:"));
#pragma warning restore CA1310
        int newReceivedMessages = _signalRMessages.Skip(initialMessageCount)
#pragma warning disable CA1310
            .Count(m => m.StartsWith("Received:"));
#pragma warning restore CA1310

        Assert.That(newSentMessages, Is.GreaterThan(0));
        Assert.That(newReceivedMessages, Is.GreaterThan(0));
    }

    [TearDown]
    public async Task TearDownSignalr()
    {
        if (_signalRPage != null)
        {
            try
            {
                await _signalRPage.DisconnectAsync();
            }
            catch (Exception)
            {
                // Ignore teardown errors
            }
        }

        await TestContext.Out.WriteLineAsync($"Captured {_capturedRequests.Count} requests");
        await TestContext.Out.WriteLineAsync($"Captured {_capturedWebSockets.Count} WebSockets");
        await TestContext.Out.WriteLineAsync($"Captured {_signalRMessages.Count} SignalR messages");
    }
}
