/*
MIT License

Copyright (c) 2020 gpsnmeajp

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
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
