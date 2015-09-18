using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinger
{
    class PingInformation
    {
        public string IP;
        public long Delay;
        public int SendByte;
        public int TTL;

        bool isError;
        string ErrorInformation = "";

        public PingInformation(string cIP,long cDelay,int cByte,int cTTL)
        {
            IP = cIP; Delay = cDelay; SendByte = cByte; TTL = cTTL; 
        }

        public PingInformation(System.Net.IPAddress iPAddress, long delay, int cByte, int cTTL)
        {
            IP = iPAddress.ToString();
            Delay = delay;
            SendByte = cByte;
            TTL = cTTL; 
        }

        public PingInformation(string ErrorInformationPing)
        {
            setError(ErrorInformationPing);
        }

        public PingInformation()
        {
            IP = "0.0.0.0";
            Delay = 0;
            SendByte = 0;
            TTL = 0;
            isError = false;
        }


        public string getError()
        {
            if (isError) return ErrorInformation;
            return "";
        }

        public void setError(string ErrorString)
        {
            isError = true;
            ErrorInformation = ErrorString;
        }

        public bool checkError()
        {
            return isError;
        }
    }
}
