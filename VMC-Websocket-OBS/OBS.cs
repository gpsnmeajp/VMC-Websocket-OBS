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
        //イベント打ち上げ用
        public Action OnConnected = null;
        public Action OnDisconnected = null;
        public Action OnOBSExit = null;

        OBSWebsocket obsWebsocket = null;

        //Websocket接続
        public void Start(string adr, string pass)
        {
            obsWebsocket = new OBSWebsocket();

            //コールバックをセット
            obsWebsocket.Connected += Connected;
            obsWebsocket.Disconnected += Disconnected;
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
            obsWebsocket?.Disconnect();
        }

        //シーンをセットする
        public void SetScene(string Scene)
        {
            obsWebsocket.SetCurrentScene(Scene);
        }

        //イベントは上位に打ち上げる
        private void Connected(object obj, EventArgs arg)
        {
            try
            {
                OnConnected?.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine("# Callback : " + e);
            }
        }
        private void Disconnected(object obj, EventArgs arg)
        {
            try
            {
                OnDisconnected?.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine("# Callback : " + e);
            }
        }
        private void OBSExit(object obj, EventArgs arg)
        {
            try
            {
                OnOBSExit?.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine("# Callback : " + e);
            }
        }
    }
}
