using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMC_Websocket_OBS
{
    class Main
    {
        const int port = 39539;
        const string adr = "ws://127.0.0.1:4444";
        const string pass = "";

        OBS obs = new OBS();
        VMC vmc = new VMC();

        public void Start() {

            Console.WriteLine("### VMC-Websocket-OBS v0.00");

            try
            {
                Console.WriteLine("# Connecting to OBS Websocket...");
                obs.Start(adr, pass);

                Console.WriteLine("# Start standby for VMC Protocol");
                vmc.Start(port,obs);

                Console.WriteLine("### Press ENTER key to EXIT");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("# MainThread : "+e);
            }

            Console.WriteLine("### EXIT...");
            vmc.Stop();
            obs.Stop();
            Console.WriteLine("### OK");
        }
    }
}
