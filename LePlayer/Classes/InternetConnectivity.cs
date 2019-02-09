﻿using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

namespace System
{
    /// <summary>
    /// Allows Truffle to test the internet connection
    /// </summary>
    public static class InternetConnectivity
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connDescription, int ReservedValue);
        public static bool IsConnectionAvailable() { int dummy; return InternetGetConnectedState(out dummy, 0); }

        public static bool strongInternetConnectionTest() {
            try {
                Ping testGoogle = new Ping();
                //PingReply reply = testGoogle.Send("qut.edu.au", 1000, new byte[32]); //Ping QUT
                PingReply reply = testGoogle.Send("google.com", 1000, new byte[32]); //Ping QUT
                return reply.Status == IPStatus.Success;
            }
            catch {
                return false;
            }
        }
    }
}
