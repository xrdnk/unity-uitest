﻿using System;

namespace UnityUITest.Example
{
    public class NetworkClient
    {
        public virtual string SendServerRequest(string request)
        {
            // Seems like our example server is offline, but this method is still testable
            throw new Exception("Server unavailable");
        }
    }
}