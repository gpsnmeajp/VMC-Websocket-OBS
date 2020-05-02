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
        OscReceiver oscReceiver = null;
        Thread thread = null;
        public void Start(int port)
        {
            //受信待受
            oscReceiver = new OscReceiver(port);
            oscReceiver.Connect();

            //受信処理スレッド
            thread = new Thread(new ThreadStart(ReceiveThread));
            thread.Start();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

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
                Console.WriteLine("# ReceiveThread : " + e);
            }
        }
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
        private void ProcessMessage(OscMessage message)
        {
            Console.WriteLine("ProcessMessage : " + message);
        }

    }
}
