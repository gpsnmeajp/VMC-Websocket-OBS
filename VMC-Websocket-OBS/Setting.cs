using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMC_Websocket_OBS
{
    class Setting
    {
        public int VMCProtocolPort { set; get; }
        public string OBSWebSocketURI { set; get; }
        public string OBSWebSocketPassword { set; get; }


        public string SceneOfCalibrationComplete { set; get; }
        public string SceneOfCalibrationInProgress { set; get; }
    }
}
