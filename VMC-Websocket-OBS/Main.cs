using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rug.Osc;

namespace VMC_Websocket_OBS
{
    class Main
    {
        const int port = 39539;
        const string adr = "ws://127.0.0.1:4444";
        const string pass = "";

        OBS obs = new OBS();
        VMC vmc = new VMC();

        bool exit = false;
        int old_calibration_state = -1;

        //起動～終了処理
        public void Process() {

            Console.WriteLine("### VMC-Websocket-OBS v0.00");

            try
            {
                Console.WriteLine("# Connecting to OBS Websocket...");
                //OBS-Websocket切断時コールバックを設定
                obs.OnDisconnected = Shutdown;
                obs.OnOBSExit = Shutdown;
                //OBS-Websocketに接続
                obs.Start(adr, pass);

                Console.WriteLine("# Start standby for VMC Protocol");
                //メッセージ受信コールバックを設定
                vmc.OnMessage = OnMessage;
                //VMCの通信を待ち受け
                vmc.Start(port);

                //ユーザーの終了操作を待機
                Console.WriteLine("### Press ENTER key to EXIT");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("# MainThread : "+e);
            }

            Console.WriteLine("### EXIT...");

            //OBS-Websocket切断時コールバックを解除
            obs.OnDisconnected = null;
            obs.OnOBSExit = null;

            vmc.Stop();
            obs.Stop();
            Console.WriteLine("### OK");
        }

        //通信失敗、OBSの終了などの通知を受けたとき
        private void Shutdown()
        {
            Console.WriteLine("### Couldn't connect to OBS");
            //強制的に終了する
            try
            {
                vmc.Stop();
                obs.Stop();
            }
            catch (Exception e) {
                Console.WriteLine("# Callback : " + e);
            }
            System.Environment.Exit(-1);
        }

        //VMCからのMessageイベント
        private void OnMessage(OscMessage message)
        {
            if (message.Address == "/VMC/Ext/OK")
            {
                if (message.Count >= 3 && message[1] is int)
                {
                    int calibration_state = (int)message[1];
                    if (old_calibration_state != calibration_state)
                    {
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
