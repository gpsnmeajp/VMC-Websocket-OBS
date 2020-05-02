# VMC-Websocket-OBS
[VMCProtocol](https://sh-akira.github.io/VirtualMotionCaptureProtocol/)を用いてバーチャルモーションキャプチャーの状態を取得し、  
Websocketを用いてOBSを制御するソフトウェア

## 現在利用可能な機能
- キャリブレーションを開始した際と完了時にOBSのシーンを切り替える(Tポーズ写り対策)

## 必要物
- [バーチャルモーションキャプチャー](https://sh-akira.github.io/VirtualMotionCapture/)
- [OBS Studio](https://obsproject.com/ja)
- [obs-websocket](https://github.com/Palakis/obs-websocket)

## 設定
- obs-websocketを導入してください。
- バーチャルモーションキャプチャーのモーション送信を有効にしてください。
- setting.jsonを設定してください。(ポートは基本設定不要です。Sceneを自分で使いたいシーン名に変えてください。半角英数字のみにすることを推奨します。)
- OBS Studioを起動した状態で、VMC-Websocket-OBS.exeを起動してください。
- ファイアーウォール警告が出た場合は許可してください。
- OBS Studioの方で接続通知が出れば準備ができています。
- バーチャルモーションキャプチャーでキャリブレーション操作をして動作を確認してください。

```JSON
{
	"VMCProtocolPort": 39539,
	"OBSWebSocketURI": "ws://127.0.0.1:4444",
	"OBSWebSocketPassword": "",
	"SceneOfCalibrationComplete": "Scene1",
	"SceneOfCalibrationInProgress": "Scene2"
}
```