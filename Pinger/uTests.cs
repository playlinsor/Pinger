using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Pinger
{
    [TestFixture]
    class MainProgamTests
    {

        [Test]
        public void Assertss()
        {
            Assert.AreEqual(10, 10);
        }


    }


    // Для пинга
    [TestFixture] class PingInformationTests
    {
        [Test] public void Create()
        {
            PingInformation pi = new PingInformation();
            Assert.AreEqual(pi.Delay, 0);
        }

        [Test] public void ErrorTest()
        {
            PingInformation pi = new PingInformation("Error");
            Assert.AreEqual(pi.getError(), "Error");
            Assert.AreEqual(pi.checkError(), true);
            PingInformation pis = new PingInformation();
            pis.setError("ERROR");
            Assert.AreEqual(pis.getError(), "ERROR");
            Assert.AreEqual(pis.checkError(), true);
        }

        [Test] public void CreateFull()
        {
            PingInformation pi = new PingInformation("10.1.1.1", 20, 30, 40);
            Assert.AreEqual(pi.IP, "10.1.1.1");
            Assert.AreEqual(pi.Delay, 20);
            Assert.AreEqual(pi.SendByte, 30);
            Assert.AreEqual(pi.TTL, 40);
        }

        [Test] public void CreateFullIP()
        {
            System.Net.IPAddress iPAddress = new System.Net.IPAddress(new byte[] { 10, 1, 2, 3 });
            PingInformation pi = new PingInformation(iPAddress, 20, 30, 40);
            Assert.AreEqual(pi.IP, "10.1.2.3");
            Assert.AreEqual(pi.Delay, 20);
            Assert.AreEqual(pi.SendByte, 30);
            Assert.AreEqual(pi.TTL, 40);
        }

    }
}
