namespace Try {
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Net.Sockets;
  using System.Numerics;
  using System.Text;
  using System.Threading.Tasks;

  using TransformationSpace;

  public class PTZCtrl_SP1510 : SpaceObject, IDisposable {
    /// <summary>
    /// Int16转Bytex2,按书写顺序高位->高位，低位->低位
    /// </summary>
    /// <param name="Number"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void Int16ToByte2InByteArray(in ushort Number, in byte[] Dest, in int Offset) {
      Dest[Offset] = (byte)(Number & 0xFF);
      Dest[Offset + 1] = (byte)((Number >> 8) & 0xFF);
    }
    /// <summary>
    /// Int16转Bytex2,按书写顺序高位->低位，低位->高位
    /// </summary>
    /// <param name="Number"></param>
    /// <param name="Dest"></param>
    /// <param name="Offset"></param>
    public static void Int16ToByte2InByteArrayRev(in ushort Number, in byte[] Dest, in int Offset) {
      Dest[Offset + 1] = (byte)(Number & 0xFF);
      Dest[Offset] = (byte)((Number >> 8) & 0xFF);
    }
    /// <summary>
    /// Bytex2转Int16,按书写顺序高位->高位，低位->低位
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static ushort Byte2ToInt16FromByteArray(in byte[] Source, in int Offset)
      => (ushort)((Source[Offset] & 0xFF) | (Source[Offset + 1] & 0xFF) << 8);
    /// <summary>
    /// Bytex2转Int16,按书写顺序高位->低位，低位->高位
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static ushort Byte2ToInt16FromByteArrayRev(in byte[] Source, in int Offset)
      => (ushort)((Source[Offset + 1] & 0xFF) | (Source[Offset] & 0xFF) << 8);
    public PTZCtrl_SP1510() {
      _Communicator = new TcpClient();
      //_Node.SyncDevice = TurnToPosition;
    }
    public const byte MaxSpeed = 0x40, MinSpeed = 0x00;

    public SP1510Cfg Cfg { get; protected set; }
    public Vector3 SelfPosition { get; protected set; }
    public Vector3 SelfRotation { get; protected set; }
    public ushort CurrentSpeedV, CurrentSpeedH;

    private readonly TcpClient _Communicator;
    private readonly byte[] RecviedData = new byte[7];
    private readonly byte[] _RequestPackage = new byte[] { 255, 0, 0, 0, 0, 0, 0 };
    /// <summary>
    /// X:Vertical/Tilt Z:Zoom Y:Horizental/Pan
    /// </summary>
    public Vector3 PTZState {
      get => LocalRotationEuler;
    }

    /// <summary>
    /// 垂直Euler转数据
    /// </summary>
    /// <param name="Degree">[-45,45]</param>
    /// <returns></returns>
    public ushort TRS2PTZ_Vertical(in float Degree) =>
     Convert.ToUInt16((Degree >= 0 ? (Degree > 45 ? 45 : Degree) * 2 : (360 - (Degree <= -45 ? -45 : Degree) * -2)) * 100);
    /// <summary>
    /// 水平Euler转数据
    /// </summary>
    /// <param name="Degree">[-180,180]</param>
    /// <returns></returns>
    public ushort TRS2PTZ_Horizontal(in float Degree) =>
      Convert.ToUInt16(((Degree < 0 ? -Degree : 360 - Degree)) * 100);
    ///// <summary>
    ///// 数据转垂直Euler
    ///// </summary>
    ///// <param name="Degree"></param>
    ///// <returns></returns>
    //public float PTZ2TRS_Vertical(in ushort Degree) =>
    // Convert.ToSingle((Degree <= 9000 ? ((Degree >= 0 ? Degree : 0) - 9000) : Degree >= 27000 ? Degree - 27000 : Degree) / 100);
    ///// <summary>
    ///// 数据转水平Euler
    ///// </summary>
    ///// <param name="Degree"></param>
    ///// <returns></returns>
    //public float PTZ2TRS_Horizontal(in ushort Degree) =>
    //  Convert.ToSingle(Degree / 100);

    /// <summary>
    /// 复制默认数据包，写地址
    /// </summary>
    /// <param name="Address"></param>
    /// <returns></returns>
    protected byte[] WriteAddress(byte Address) {
      var Bs = new byte[_RequestPackage.Length];
      _RequestPackage.CopyTo(Bs, 0);
      Bs[1] = Address;
      return Bs;
    }
    /// <summary>
    /// 写命令
    /// </summary>
    /// <param name="Package"></param>
    /// <param name="Command"></param>
    /// <returns></returns>
    protected byte[] WriteCommand(in byte[] Package, PTZ_Pelco_D_Command Command) {
      Int16ToByte2InByteArrayRev((ushort)Command, Package, 2);
      return Package;
    }
    /// <summary>
    /// 写数据
    /// </summary>
    /// <param name="Package"></param>
    /// <param name="Data"></param>
    /// <returns></returns>
    protected byte[] WriteData(in byte[] Package, ushort Data) {
      Int16ToByte2InByteArrayRev(Data, Package, 4);
      return Package;
    }
    /// <summary>
    /// 写校验
    /// </summary>
    /// <param name="Package"></param>
    /// <returns></returns>
    protected byte[] WriteCheckSum(in byte[] Package) {
      Package[6] = (byte)((Package[1] + Package[2] + Package[3] + Package[4] + Package[5]) & 0xFF);
      return Package;
    }
    /// <summary>
    /// 配置绝对角度旋转速度
    /// </summary>
    /// <param name="VSpeed"></param>
    /// <param name="HSpeed"></param>
    public void SetSpeed(ushort VSpeed, ushort HSpeed) {
      var PV = WriteCheckSum(WriteData(WriteCommand(WriteAddress(Cfg.Address), PTZ_Pelco_D_Command.SetDegreeMoveSpeed), Convert.ToUInt16((HSpeed << 16) | VSpeed)));
      try {
        SendPackage(PV);
      }
      catch (Exception E) {
        Console.WriteLine(E);
        Console.WriteLine($"{Cfg.Name} M=SetSpeed IP={Cfg.IP} Data= {string.Join(',', PV)}");
      }
    }
    /// <summary>
    /// 转动至默认角度
    /// </summary>
    public void ResetPositionToDefault() {
      var P1 = WriteCheckSum(WriteData(WriteCommand(WriteAddress(Cfg.Address), PTZ_Pelco_D_Command.SetHorizentalDegree), TRS2PTZ_Horizontal(SelfRotation.Y)));
      var P2 = WriteCheckSum(WriteData(WriteCommand(WriteAddress(Cfg.Address), PTZ_Pelco_D_Command.SetVerticalDegree), TRS2PTZ_Vertical(SelfRotation.X)));
      try {
        Console.WriteLine($"{Cfg.Name} M=ResetPositionToDefault IP={Cfg.IP} Data= {string.Join(',', P1)}");
        SendPackage(P1);
      }
      catch (Exception E) {
        Console.WriteLine(E);
        Console.WriteLine($"{Cfg.Name} M=ResetPositionToDefault IP={Cfg.IP} Data= {string.Join(',', P1)}");
      }
      try {
        Console.WriteLine($"{Cfg.Name} M=ResetPositionToDefault IP={Cfg.IP} Data= {string.Join(',', P2)}");
        SendPackage(P2);
      }
      catch (Exception E) {
        Console.WriteLine(E);
        Console.WriteLine($"{Cfg.Name} M=ResetPositionToDefault IP={Cfg.IP} Data= {string.Join(',', P2)}");
      }
    }
    /// <summary>
    /// 转至角度
    /// </summary>
    /// <param name="HorDeg"></param>
    /// <param name="VerDeg"></param>
    /// <param name="VSpeed"></param>
    /// <param name="HSpeed"></param>
    public void TurnToPosition(in ushort HorDeg, in ushort VerDeg, in ushort VSpeed = 0, in ushort HSpeed = 0) {
      if (VSpeed > 0 || HSpeed > 0) {
        SetSpeed(VSpeed == 0 ? CurrentSpeedV : VSpeed, HSpeed == 0 ? CurrentSpeedH : HSpeed);
      }
      var P1 = WriteCheckSum(WriteData(WriteCommand(WriteAddress(Cfg.Address), PTZ_Pelco_D_Command.SetHorizentalDegree), HorDeg));
      var P2 = WriteCheckSum(WriteData(WriteCommand(WriteAddress(Cfg.Address), PTZ_Pelco_D_Command.SetVerticalDegree), VerDeg));
      try {
        Console.WriteLine($"{Cfg.Name} M=TurnToPosition IP={Cfg.IP} Data= {string.Join(',', P1)}");
        SendPackage(P1);
      }
      catch (Exception E) {
        Console.WriteLine(E);
        Console.WriteLine($"{Cfg.Name} M=TurnToPosition IP={Cfg.IP} Data= {string.Join(',', P1)}");
      }
      try {
        Console.WriteLine($"{Cfg.Name} M=TurnToPosition IP={Cfg.IP} Data= {string.Join(',', P2)}");
        SendPackage(P2);
      }
      catch (Exception E) {
        Console.WriteLine(E);
        Console.WriteLine($"{Cfg.Name} M=TurnToPosition IP={Cfg.IP} Data= {string.Join(',', P2)}");
      }
    }

    public void TurnToPosition() {
      Console.WriteLine($"SendPackageTo {Cfg.Name} H={TRS2PTZ_Horizontal(LocalRotationEuler.Y)} FROM {LocalRotationEuler.Y} V={TRS2PTZ_Vertical(LocalRotationEuler.X)} FROM {LocalRotationEuler.X}");
      TurnToPosition(TRS2PTZ_Horizontal(LocalRotationEuler.Y), TRS2PTZ_Vertical(LocalRotationEuler.X));
    }

    /// <summary>
    /// 设置点,并扫描
    /// </summary>
    public void SetPointsAndScan(in Vector3 Start, in Vector3 End, in ushort VSpeed = 0, in ushort HSpeed = 0) {

    }
    /// <summary>
    /// 设置扫描速度
    /// </summary>
    /// <param name="VSpeed"></param>
    /// <param name="HSpeed"></param>
    public void SetScanSpeed(in ushort VSpeed = 0, in ushort HSpeed = 0) {

    }



    /// <summary>
    /// 初始化配置
    /// </summary>
    /// <param name="Cfg"></param>
    /// <returns></returns>
    public bool InitCfg(SP1510Cfg Cfg) {
      this.Cfg = Cfg;

      SelfRotation = new Vector3(Cfg.VerticalDefault / 100, Cfg.HorizonDefault / 100, 0F);
      SelfPosition = new Vector3(Cfg.SelfPositionX, Cfg.SelfPositionY, Cfg.SelfPositionZ);


      LocalRotationEuler = (SelfRotation);
      LocalPosition = SelfPosition;
      Name = Cfg.Name;

      return true;
    }
    /// <summary>
    /// 是否已连接设备
    /// </summary>
    public bool Connected { get => (_Communicator?.Connected) ?? false; }
    /// <summary>
    /// 连接设备
    /// </summary>
    public void Connect() {
      if (Cfg.Enable) {
        if (!Connected) {
          _Communicator.Connect(new IPEndPoint(IPAddress.Parse(Cfg.IP), Cfg.Port));
          Console.WriteLine($"Connect To {Cfg.Name} IP={Cfg.IP}");
          Task.Run(async () => {
            if (_Communicator.Available >= 7) {
              _Communicator.GetStream().Read(RecviedData, 0, RecviedData.Length);
              if (((RecviedData[1] + RecviedData[2] + RecviedData[3] + RecviedData[4] + RecviedData[5]) & 0xFF) == RecviedData[6]) {
                if (RecviedData[1] == Cfg.Address) {
                  if (RecviedData[3] == 0x59 && RecviedData[2] == 0x00) {
                    //水平角度
                    var Deg = Byte2ToInt16FromByteArrayRev(RecviedData, 4);
                    Console.WriteLine($"REC:H{Deg}");
                  }
                  else if (RecviedData[3] == 0x5B && RecviedData[2] == 0x00) {
                    //垂直角度
                    var Deg = Byte2ToInt16FromByteArrayRev(RecviedData, 4);
                    Console.WriteLine($"REC:V{Deg}");
                  }
                }
              }
            }
          });
        }
      }
      else
        Console.WriteLine($"Disabled -> Skip connect To {Cfg.Name} IP={Cfg.IP}");


    }
    /// <summary>
    /// 发送数据包
    /// </summary>
    /// <param name="Package"></param>
    protected void SendPackage(in byte[] Package) {
      _Communicator.GetStream().Write(Package, 0, Package.Length);
    }

    private bool disposedValue;
    protected virtual void Dispose(bool disposing) {
      if (!disposedValue) {
        if (disposing) {
          _Communicator.Close();
          _Communicator.Dispose();
          // TODO: dispose managed state (managed objects)
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }
    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~PTZCtrl_SP1510()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }
    public void Dispose() {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    public override void OnEngage() {
      base.OnEngage();
      InitCfg(new SP1510Cfg { Address = 0x01, Port = 23, IP = "192.168.0.7", Enable = true, HorizonDefault = 18000, Name = "PTZ1", SelfPositionX = 750, SelfPositionY = 400, SelfPositionZ = 20000, VerticalDefault = 0 });
      Connect();
      ResetPositionToDefault();
    }
    public override void OnEject() {
      base.OnEject();
    }

  }
  public enum PTZ_Pelco_D_Command : ushort {
    Stop = 0x0000,
    Tilt_Up = 0x0000 | 0x0008,
    Tilt_Down = 0x0000 | 0x0010,
    Pan_Left = 0x0000 | 0x0004,
    Pan_Right = 0x0000 | 0x0002,

    Pan_RightAndTilt_Up = 0x0000 | 0x0002 | 0x0008,
    Pan_RightAndTilt_Down = 0x0000 | 0x0002 | 0x0010,
    Pan_LeftAndTilt_Up = 0x0000 | 0x0004 | 0x0008,
    Pan_LeftAndTilt_Down = 0x0000 | 0x0004 | 0x0010,

    SetHorizentalDegree = 0x0000 | 0x004B,
    SetVerticalDegree = 0x0000 | 0x004D,
    SetDegreeMoveSpeed = 0x0000 | 0x005F,

    SetScanStartPoint = 0x0000 | 0x0011,
    SetScanEndPoint = 0x0000 | 0x0013,
    SetScanStart = 0x0000 | 0x001B,
    SetScanEnd = 0x0000 | 0x001D,

    SetPresetPosition = 0x0000 | 0x0003,
    DelPresetPosition = 0x0000 | 0x0005,
    GetPresetPosition = 0x0000 | 0x0007,
  }
  public class SP1510Cfg {
    public bool Enable { get; set; }
    public string Name { get; set; }
    public string IP { get; set; }
    public int Port { get; set; }
    public byte Address { get; set; } = 0x01;
    /// <summary>
    /// unit:0.01° Min:0 Max:36000
    /// </summary>
    public ushort HorizonDefault { get; set; } = 0;
    /// <summary>
    /// unit:0.01° InUse(Min:0 Max:17999 Horizantal:9000) | OnDevice: Down(Min:0 Max:9000) Up(Min:27000 Max:35999)
    /// </summary>
    public ushort VerticalDefault { get; set; } = 9000;
    /// <summary>
    /// UNIT:cm Min:0 Max:65535
    /// </summary>
    public ushort SelfPositionX { get; set; } = 725;
    /// <summary>
    /// UNIT:cm Min:0 Max:65535
    /// </summary>
    public ushort SelfPositionY { get; set; } = 183 * 100;
    /// <summary>
    /// UNIT:cm Min:0 Max:65535
    /// </summary>
    public ushort SelfPositionZ { get; set; } = 4 * 100;

    public string LookAtTargetName { get; set; }

  }


}
