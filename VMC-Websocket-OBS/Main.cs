using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rug.Osc;
using Newtonsoft.Json;

namespace VMC_Websocket_OBS
{
    class Main
    {
        Setting setting = null; //設定

        OBS obs = new OBS();
        VMC vmc = new VMC();

        int old_calibration_state = -1;

        //起動～終了処理
        public void Process() {

            Console.WriteLine("### VMC-Websocket-OBS v0.01");

            try
            {
                LoadJson();

                Connect();

                //ユーザーの終了操作を待機
                Console.WriteLine("### Press ENTER key to stop program");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("# MainThread : "+e);
            }

            Console.WriteLine("### EXIT");
            Disconnect();

            Console.WriteLine("### Press ENTER key to close window");
            Console.ReadLine();
        }

        //JSON設定ファイルを読み込む
        private void LoadJson()
        {
            string json = File.ReadAllText("setting.json", new UTF8Encoding(false));
            setting = JsonConvert.DeserializeObject<Setting>(json);

            Console.WriteLine("# Setting loaded");
        }

        //接続開始処理
        private void Connect()
        {
            Console.WriteLine("# Connecting to OBS Websocket...");
            //OBS-Websocket切断時コールバック(接続失敗・異常切断)を設定
            obs.OnDisconnected = Shutdown;
            obs.OnOBSExit = Shutdown;
            //OBS-Websocketに接続
            obs.Start(setting.OBSWebSocketURI, setting.OBSWebSocketPassword);

            Console.WriteLine("# Start standby for VMC Protocol");
            //メッセージ受信コールバックを設定
            vmc.OnMessage = OnMessage;
            //VMCの通信を待ち受け
            vmc.Start(setting.VMCProtocolPort);
        }
        //切断処理
        private void Disconnect()
        {
            //OBS-Websocket切断時コールバック(接続失敗・異常切断)を解除
            obs.OnDisconnected = null;
            obs.OnOBSExit = null;

            vmc.Stop();
            obs.Stop();
        }

        //通信失敗、OBSの終了などの通知を受けたとき
        private void Shutdown()
        {
            Console.WriteLine("### Couldn't connect to OBS");
            try
            {
                //強制的に切断する
                Disconnect();
            }
            catch (Exception e) {
                Console.WriteLine("# Callback : " + e);
            }
            System.Environment.Exit(-1);

            Console.WriteLine("### Press ENTER key to close window");
            Console.ReadLine();
        }

        //VMCからのMessageを処理する
        private void OnMessage(OscMessage message)
        {
            // VMC Protocolを参照
            // https://sh-akira.github.io/VirtualMotionCaptureProtocol/
            if (message.Address == "/VMC/Ext/OK")
            {
                if (message.Count >= 3 && message[1] is int)
                {
                    //キャリブレーション状態を取り出し
                    int calibration_state = (int)message[1];
                    if (old_calibration_state != calibration_state)
                    {
                        Console.WriteLine("# calibration_state changed " + old_calibration_state + "->" + calibration_state);
                        old_calibration_state = calibration_state;
                        if (calibration_state == 3)
                        {
                            obs.SetScene(setting.SceneOfCalibrationComplete);
                        }
                        else
                        {
                            obs.SetScene(setting.SceneOfCalibrationInProgress);
                        }
                    }
                }
            }
        }


    }
}
