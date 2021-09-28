namespace UnityUITest.Example
{
    public class MockNetworkClient : NetworkClient
    {
        public string mockRequest;
        public string mockResponse;

        public override string SendServerRequest(string request)
        {
            mockRequest = request;
            return mockResponse;
        }
    }


}