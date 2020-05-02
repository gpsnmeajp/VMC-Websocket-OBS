using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OBSWebsocketDotNet;

namespace VMC_Websocket_OBS
{
    class OBS
    {
        OBSWebsocket obsWebsocket = null;

        //Websocket接続
        public void Start(string adr,string pass)
        {
            obsWebsocket = new OBSWebsocket();

            //コールバックをセット
            obsWebsocket.Connected += Connected;
            obsWebsocket.Disconnected += Connected;
            obsWebsocket.OBSExit += OBSExit;

            //タイムアウトをセット
            obsWebsocket.WSTimeout = new TimeSpan(0, 0, 3);

            //接続を実施
            obsWebsocket.Connect(adr, pass);
            //例外は上位に打ち上げる
        }
        //Websocket切断
        public void Stop()
        {
            obsWebsocket.Disconnect();
        }

        public bool IsConnected()
        {
            if (obsWebsocket != null)
            {
                if (obsWebsocket.WSConnection != null)
                {
                    return obsWebsocket.IsConnected;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        public void SetScene(string Scene) {
            obsWebsocket.SetCurrentScene(Scene);
        }

        private void Connected(object obj, EventArgs arg)
        {
            //当てにならない
            //Console.WriteLine("Connected");
        }
        private void Disonnected(object obj, EventArgs arg)
        {
            Console.WriteLine("Disonnected");
        }
        private void OBSExit(object obj, EventArgs arg)
        {
            Console.WriteLine("OBSExit");
        }
    }
}
