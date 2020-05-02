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

        OBS obs;
        //受信待受開始
        public void Start(int port, OBS obs)
        {
            this.obs = obs;

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
            oscReceiver.Close();
            //Thread終了を待機
            thread.Join();
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
                Console.WriteLine("# ReceiveThread : " + e);
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
        int old_calibration_state = -1;
        private void ProcessMessage(OscMessage message)
        {
            //Console.WriteLine("ProcessMessage : " + message);
            if (message.Address == "/VMC/Ext/OK") {
                if (message.Count >= 3 && message[1] is int)
                {
                    int calibration_state = (int)message[1];
                    if (old_calibration_state != calibration_state) {
                        Console.WriteLine("# calibration_state changed " + old_calibration_state + "->" + calibration_state);
                        old_calibration_state = calibration_state;
                        if (calibration_state == 3)
                        {
                            Console.WriteLine("Scene1");
                            obs.SetScene("Scene1");
                        }
                        else
                        {
                            Console.WriteLine("Scene2");
                            obs.SetScene("Scene2");
                        }
                    }
                }
            }

        }

    }
}
