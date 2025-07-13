




private Controller<AUTD3Sharp.Link.TwinCAT>? _autd = null;

private const float OscillatorRadius = 0.00508f; // 振動子の半径 AUTD位置設定用
private const float width_x_right = 0.0004f;  // 向かって右側のAUTDと正面のAUTDのAUTD座標におけるx軸方向のずれ(正面のAUTDのtransducerの端に対する、右側のAUTDのtransducerの面の位置)(正は正面のAUTDに対してx軸正方向に移動している)
private const float width_y_right = 0.0005f;  // 向かって右側のAUTDと正面のAUTDのAUTD座標におけるy軸方向のずれ(正面のAUTDのtransducerの面に対する、右側のAUTDのtransducerの端の位置)(正は正面のAUTDに対してy軸正方向に移動している)
private const float width_x_left = -0.001f;  // 向かって左側のAUTDと正面のAUTDのAUTD座標におけるx軸方向のずれ(正面のAUTDのtransducerの端に対する、左側のAUTDのtransducerの面の位置)(正は正面のAUTDに対してx軸正方向に移動している)
private const float width_y_left = 0.0005f;  // 向かって左側のAUTDと正面のAUTDのAUTD座標におけるy軸方向のずれ(正面のAUTDのtransducerの面に対する、左側のAUTDのtransducerの端の位置)(正は正面のAUTDに対してy軸正方向に移動している)
private const float WidthOriginToOscillatorEdge = 0.1778f; //AUTDの原点位置から、width方向(x軸)に進んで、一番端のオシレーターの端までの長さ[m]

UnityEngine.Vector3[] tmp_pos = new UnityEngine.Vector3[]{
            //1-4
            new UnityEngine.Vector3(-AUTD3.DeviceWidth - OscillatorRadius + width_x_left,WidthOriginToOscillatorEdge+AUTD3.DeviceWidth + width_y_left,-AUTD3.DeviceHeight),
            new UnityEngine.Vector3(-AUTD3.DeviceWidth - OscillatorRadius + width_x_left,WidthOriginToOscillatorEdge+AUTD3.DeviceWidth + width_y_left,0),
            new UnityEngine.Vector3(-AUTD3.DeviceWidth - OscillatorRadius + width_x_left,WidthOriginToOscillatorEdge + width_y_left,0),
            new UnityEngine.Vector3(-AUTD3.DeviceWidth - OscillatorRadius + width_x_left,WidthOriginToOscillatorEdge + width_y_left,-AUTD3.DeviceHeight),
            //5-8
            new UnityEngine.Vector3(-AUTD3.DeviceWidth,0,-AUTD3.DeviceHeight),
            new UnityEngine.Vector3(-AUTD3.DeviceWidth,0,0),
            new UnityEngine.Vector3(0,0,0),
            new UnityEngine.Vector3(0,0,-AUTD3.DeviceHeight),
            //9-12
            new UnityEngine.Vector3(WidthOriginToOscillatorEdge + width_x_right,OscillatorRadius + width_y_right,-AUTD3.DeviceHeight),
            new UnityEngine.Vector3(WidthOriginToOscillatorEdge + width_x_right,OscillatorRadius + width_y_right,0),
            new UnityEngine.Vector3(WidthOriginToOscillatorEdge + width_x_right,OscillatorRadius+AUTD3.DeviceWidth + width_y_right,0),
            new UnityEngine.Vector3(WidthOriginToOscillatorEdge + width_x_right,OscillatorRadius+AUTD3.DeviceWidth + width_y_right,-AUTD3.DeviceHeight),
        };
UnityEngine.Quaternion[] tmp_rot = new UnityEngine.Quaternion[]{
            //1-4
            new UnityEngine.Quaternion(0,0,-MathF.Sin(MathF.PI / 4),MathF.Cos(MathF.PI / 4)) * new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            new UnityEngine.Quaternion(0,0,-MathF.Sin(MathF.PI / 4),MathF.Cos(MathF.PI / 4)) * new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            new UnityEngine.Quaternion(0,0,-MathF.Sin(MathF.PI / 4),MathF.Cos(MathF.PI / 4)) * new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            new UnityEngine.Quaternion(0,0,-MathF.Sin(MathF.PI / 4),MathF.Cos(MathF.PI / 4)) * new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            //5-8
            new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            //9-12
            new UnityEngine.Quaternion(0,0,MathF.Sin(MathF.PI / 4),MathF.Cos(MathF.PI / 4)) * new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            new UnityEngine.Quaternion(0,0,MathF.Sin(MathF.PI / 4),MathF.Cos(MathF.PI / 4)) * new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            new UnityEngine.Quaternion(0,0,MathF.Sin(MathF.PI / 4),MathF.Cos(MathF.PI / 4)) * new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
            new UnityEngine.Quaternion(0,0,MathF.Sin(MathF.PI / 4),MathF.Cos(MathF.PI / 4)) * new UnityEngine.Quaternion(-MathF.Sin(MathF.PI / 4),0,0, MathF.Cos(MathF.PI / 4)),
        };

_autd = Controller.Open(
        //FindObjectsByType<AUTD3Device>(FindObjectsSortMode.InstanceID).Select(obj => new AUTD3(pos: obj.transform.position, rot: obj.transform.rotation)),
        new[] {
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[0].x,tmp_pos[0].y,tmp_pos[0].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[0].w,tmp_rot[0].x,tmp_rot[0].y,tmp_rot[0].z)), // 1.
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[1].x,tmp_pos[1].y,tmp_pos[1].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[1].w,tmp_rot[1].x,tmp_rot[1].y,tmp_rot[1].z)),
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[2].x,tmp_pos[2].y,tmp_pos[2].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[2].w,tmp_rot[2].x,tmp_rot[2].y,tmp_rot[2].z)),
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[3].x,tmp_pos[3].y,tmp_pos[3].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[3].w,tmp_rot[3].x,tmp_rot[3].y,tmp_rot[3].z)),
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[4].x,tmp_pos[4].y,tmp_pos[4].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[4].w,tmp_rot[4].x,tmp_rot[4].y,tmp_rot[4].z)),
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[5].x,tmp_pos[5].y,tmp_pos[5].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[5].w,tmp_rot[5].x,tmp_rot[5].y,tmp_rot[5].z)),
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[6].x,tmp_pos[6].y,tmp_pos[6].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[6].w,tmp_rot[6].x,tmp_rot[6].y,tmp_rot[6].z)),
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[7].x,tmp_pos[7].y,tmp_pos[7].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[7].w,tmp_rot[7].x,tmp_rot[7].y,tmp_rot[7].z)),
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[8].x,tmp_pos[8].y,tmp_pos[8].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[8].w,tmp_rot[8].x,tmp_rot[8].y,tmp_rot[8].z)),
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[9].x,tmp_pos[9].y,tmp_pos[9].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[9].w,tmp_rot[9].x,tmp_rot[9].y,tmp_rot[9].z)),
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[10].x,tmp_pos[10].y,tmp_pos[10].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[10].w,tmp_rot[10].x,tmp_rot[10].y,tmp_rot[10].z)),
                        new AUTD3(pos : new AUTD3Sharp.Utils.Point3(tmp_pos[11].x,tmp_pos[11].y,tmp_pos[11].z),
                                  rot : new AUTD3Sharp.Utils.Quaternion(tmp_rot[11].w,tmp_rot[11].x,tmp_rot[11].y,tmp_rot[11].z)),
            },
        new AUTD3Sharp.Link.TwinCAT()
    //new AUTD3Sharp.Link.Simulator(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080))
    );