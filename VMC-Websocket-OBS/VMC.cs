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
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rug.Osc;

namespace VMC_Websocket_OBS
{
    class VMC
    {
        //イベント打ち上げ用
        public Action<OscMessage> OnMessage = null;

        //受信処理用
        OscReceiver oscReceiver = null;
        Thread thread = null;

        //受信待受開始
        public void Start(int port)
        {
            //受信待受
            oscReceiver = new OscReceiver(port);
            oscReceiver.Connect();

            //受信処理スレッド
            thread = new Thread(new ThreadStart(ReceiveThread));
            thread.Start();

            //例外は上位に打ち上げる
        }

        //受信待受停止
        public void Stop()
        {
            //待受停止
            oscReceiver?.Close();
            //Thread終了を待機
            thread?.Join();
        }

        //受信Thread
        private void ReceiveThread()
        {
            try
            {
                //ソケットが閉じていない間受信を続ける
                while (oscReceiver.State != OscSocketState.Closed) {
                    //接続状態なら処理を行う
                    if (oscReceiver.State == OscSocketState.Connected)
                    {
                        //受信を行う
                        var packet = oscReceiver.Receive();
                        ProcessPacket(packet);
                    }
                    else {
                        //接続されていない場合は待つ
                        Thread.Sleep(16);
                    }
                }

            }
            catch (Exception e) {
                //ソケットが閉じられた例外のときは無視する
                if (e.Message != "The receiver socket has been disconnected")
                {
                    Console.WriteLine("# ReceiveThread : " + e);
                }
            }
        }
        //パケットを処理して、bundleとMessageに振り分け
        private void ProcessPacket(OscPacket packet)
        {
            switch (packet)
            {
                //bundleを受信した
                case OscBundle bundle:
                    ProcessBundle(bundle);
                    break;
                //Messageを受信した
                case OscMessage msg:
                    ProcessMessage(msg);
                    break;
                default:
                    //Do noting
                    break;
            }
        }
        //bundleを分解してMessageに
        private void ProcessBundle (OscBundle bundle)
        {
            //bundleを分解してMessageにする
            for (int i = 0; i < bundle.Count; i++)
            {
                //Messageかチェック
                switch (bundle[i])
                {
                    case OscMessage msg:
                        ProcessMessage(msg);
                        break;
                    default:
                        //Do noting
                        break;
                }
            }
        }
        //Messageを処理
        private void ProcessMessage(OscMessage message)
        {
            //Console.WriteLine("ProcessMessage : " + message);
            try
            {
                OnMessage?.Invoke(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("# Callback : " + e);
            }
        }
    }
}
