using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ITStepFinalProject.Models
{
    public class SubscribtionModel
    {
        public string subscribedTo { get; set; }
        public WebSocket webSocket { get; set; }
        public List<int> Ids { get; set; }
        public bool IsRunning { get; set; }
        public TaskCompletionSource<object> taskCompletionSource { get; set; }

        public SubscribtionModel(string _subscribedTo, WebSocket _webSocket, List<int> _ids, 
            TaskCompletionSource<object> taskCompletionSource)
        {
            subscribedTo = _subscribedTo;
            webSocket = _webSocket;
            Ids = _ids;
            this.taskCompletionSource = taskCompletionSource;
        }


        // this can throw an exception when sending the data in case the client has closed the websocket.
        public async Task<bool> SendTextToClient(string text)
        {
            if (!webSocket.State.Equals(WebSocketState.Open))
            {
                return false;
            }
            //new byte[1024 * 4];
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            try
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(bytes, 0, bytes.Length),
                    WebSocketMessageType.Text, true,
                    CancellationToken.None);

                return true;

            }
            catch (Exception)
            {
                CloseWebSocket();
                return false;
            }

        }

        public async Task<string?> ReceiveTextFromClient()
        {
            if (!webSocket.State.Equals(WebSocketState.Open))
            {
                return null;
            }

            var bytes = new byte[1024 * 4];
            try
            {
                var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(bytes), CancellationToken.None);

                if (receiveResult.MessageType.Equals(WebSocketMessageType.Close))
                {
                    Console.WriteLine("Client Closes WebSocket connection");
                }

                return Encoding.UTF8.GetString(bytes, 0, receiveResult.Count);
            }
            catch (Exception)
            {
                CloseWebSocket();
                return null;
            }
        }

        public async void CloseWebSocket()
        {
            if (!webSocket.State.Equals(WebSocketState.Open))
            {
                return;
            }
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    "Closing connection.", CancellationToken.None);
            webSocket.Dispose();
            IsRunning = false;
            taskCompletionSource.TrySetResult(webSocket);
            Console.WriteLine("Closing WebSocket connection.");
        }

        public async void WhileOpen(Func<WebSocket, Task> action)
        {
            IsRunning = true;
            while (webSocket.State.Equals(WebSocketState.Open))
            {
                //string? text = await ReceiveTextFromClient();
                //Console.WriteLine("\nWebSocket message from client: " + text + "\n");
                await action(webSocket);
            }
            Console.WriteLine(webSocket.State);
            webSocket.Dispose();
            IsRunning = false;
            taskCompletionSource.TrySetResult(webSocket);
            Console.WriteLine("WhileOpen Closing WebSocket connection.");
        }
    }
}
