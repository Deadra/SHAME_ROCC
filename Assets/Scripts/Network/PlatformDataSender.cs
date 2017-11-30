using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// Отсюда посылаются все данные на сервер
/// </summary>
public class PlatformDataSender : MonoBehaviour
{
    public PlatformType platformType = PlatformType.FlyMotion;

    /// <summary>
    /// Включает или выключает режим сглаживания телеметрии.
    /// Если значение True, то телеметрия от игрового объекта перед отправкой на сервер
    /// будут интерполироваться/сглаживаться.
    /// </summary>
    /// <remarks>Оно не работает</remarks>
    private bool m_isSmooth = false;
    /// <summary>
    /// Чем меньше число тем дольше мы будет ждать достижения целевого значения.
    /// Число для мгновенного достижения целевого значения примерно 12
    /// </summary>
    private float m_InterpolateSpeed = 8f;

    #region Socket

    private Socket       m_s;
    private string       m_ip = "127.0.0.1";
    private int          m_port = 00000;

    #endregion Socket


    private void Start()
    {
        Connect();
    }

    /// <summary>
    /// Обязательно перед закрытием программы закрываем сокет, иначе будут ошибки
    /// </summary>
    private void OnApplicationQuit()
    {
        if (m_s != null)
        {
            m_s.Shutdown(SocketShutdown.Both);
            m_s.Close();
        }
    }

    private void Connect()
    {
        try
        {
            StopAllCoroutines();

            m_ip   = Settings.GetServerIP(platformType);
            m_port = Settings.GetServerPort(platformType);

            //Создаем соккеты
            m_s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_s.Connect(m_ip, m_port);

            //Если все хорошо, то начинаем отправлять данные
            if (m_s.Connected == true)
            {
                switch (platformType)
                {
                    case PlatformType.FlyMotion:
                        StartCoroutine(SendCopterData());
                        break;
                    case PlatformType.XDMotion:
                        StartCoroutine(SendCarData());
                        break;
                    case PlatformType.FiveDMotion:
                        StartCoroutine(SendTelegaData());
                        break;
                }
            }
            else Debug.LogError("Не удалось подключится к серверу");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// Отправляем на Fly сервер телеметрию коптера.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SendCopterData()
    {
        WOPGameData data = new WOPGameData();

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (m_s.Connected == false)
            {
                Debug.LogWarning("Связь с сервером прервана, переподключаемся");

                //Ожидаем 2 секунды
                yield return new WaitForSeconds(2f);

                Connect();
            }
            else if (!m_isSmooth)
            {
                data.fx = transform.forward.x;
                data.fy = transform.forward.y;
                data.fz = transform.forward.z;

                data.ux = transform.up.x;
                data.uy = transform.up.y;
                data.uz = transform.up.z;

                data.lx = transform.right.x * -1;
                data.ly = transform.right.y * -1;
                data.lz = transform.right.z * -1;

                byte[] bytesData = getBytes(data);
                m_s.Send(bytesData);
            }
            else
            {
                data.fx = Mathf.Lerp(data.fx, transform.forward.x, Time.deltaTime * m_InterpolateSpeed);
                data.fy = Mathf.Lerp(data.fy, transform.forward.y, Time.deltaTime * m_InterpolateSpeed);
                data.fz = Mathf.Lerp(data.fz, transform.forward.z, Time.deltaTime * m_InterpolateSpeed);

                data.ux = Mathf.Lerp(data.ux, transform.up.x, Time.deltaTime * m_InterpolateSpeed);
                data.uy = Mathf.Lerp(data.uy, transform.up.y, Time.deltaTime * m_InterpolateSpeed);
                data.uz = Mathf.Lerp(data.uz, transform.up.z, Time.deltaTime * m_InterpolateSpeed);

                data.lx = Mathf.Lerp(data.lx, transform.right.x * -1, Time.deltaTime * m_InterpolateSpeed);
                data.ly = Mathf.Lerp(data.ly, transform.right.y * -1, Time.deltaTime * m_InterpolateSpeed);
                data.lz = Mathf.Lerp(data.lz, transform.right.z * -1, Time.deltaTime * m_InterpolateSpeed);

                byte[] bytesData = getBytes(data);
                m_s.Send(bytesData);
            }
        }
    }

    /// <summary>
    /// Отправляем на XD сервер телеметрию машинки.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SendCarData()
    {
        GameData data = new GameData();

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (m_s.Connected == false)
            {
                Debug.LogWarning("Связь с сервером прервана, переподключаемся");

                //Ожидаем 2 секунды
                yield return new WaitForSeconds(2f);

                Connect();
            }
            else if (!m_isSmooth)
            {
                data.fx = transform.forward.x;
                data.fy = transform.forward.y;
                data.fz = transform.forward.z;

                data.ux = transform.up.x;
                data.uy = transform.up.y;
                data.uz = transform.up.z;

                data.rx = transform.right.x;
                data.ry = transform.right.y;
                data.rz = transform.right.z;

                data.x = transform.position.x;
                data.y = transform.position.y;
                data.z = transform.position.z;

                byte[] bytesData = getBytes(data);
                m_s.Send(bytesData);
            }
            else
            {
                data.fx = Mathf.Lerp(data.fx, transform.forward.x, Time.deltaTime * m_InterpolateSpeed);
                data.fy = Mathf.Lerp(data.fy, transform.forward.y, Time.deltaTime * m_InterpolateSpeed);
                data.fz = Mathf.Lerp(data.fz, transform.forward.z, Time.deltaTime * m_InterpolateSpeed);

                data.ux = Mathf.Lerp(data.ux, transform.up.x, Time.deltaTime * m_InterpolateSpeed);
                data.uy = Mathf.Lerp(data.uy, transform.up.y, Time.deltaTime * m_InterpolateSpeed);
                data.uz = Mathf.Lerp(data.uz, transform.up.z, Time.deltaTime * m_InterpolateSpeed);

                data.rx = Mathf.Lerp(data.rx, transform.right.x, Time.deltaTime * m_InterpolateSpeed);
                data.ry = Mathf.Lerp(data.ry, transform.right.y, Time.deltaTime * m_InterpolateSpeed);
                data.rz = Mathf.Lerp(data.rz, transform.right.z, Time.deltaTime * m_InterpolateSpeed);

                data.x = Mathf.Lerp(data.x, transform.position.x, Time.deltaTime * m_InterpolateSpeed);
                data.y = Mathf.Lerp(data.y, transform.position.y, Time.deltaTime * m_InterpolateSpeed);
                data.z = Mathf.Lerp(data.z, transform.position.z, Time.deltaTime * m_InterpolateSpeed);

                byte[] bytesData = getBytes(data);
                m_s.Send(bytesData);
            }
        }
    }

    /// <summary>
    /// Отправляем на XD сервер телеметрию телеги.
    /// Оси Forward и Right инвертированы
    /// </summary>
    /// <returns></returns>
    private IEnumerator SendTelegaData()
    {
        GameData data = new GameData();

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (m_s.Connected == false)
            {
                Debug.LogWarning("Связь с сервером прервана, переподключаемся");

                //Ожидаем 2 секунды
                yield return new WaitForSeconds(2f);

                Connect();
            }
            else if(!m_isSmooth)
            {
                data.fx = -transform.forward.x;
                data.fy = -transform.forward.y;
                data.fz = -transform.forward.z;

                data.ux = transform.up.x;
                data.uy = transform.up.y;
                data.uz = transform.up.z;

                data.rx = -transform.right.x;
                data.ry = -transform.right.y;
                data.rz = -transform.right.z;

                data.x = transform.position.x;
                data.y = transform.position.y;
                data.z = transform.position.z;

                byte[] bytesData = getBytes(data);
                m_s.Send(bytesData);
            }
            else
            {
                data.fx = Mathf.Lerp(data.fx, -transform.forward.x, Time.deltaTime * m_InterpolateSpeed);
                data.fy = Mathf.Lerp(data.fy, -transform.forward.y, Time.deltaTime * m_InterpolateSpeed);
                data.fz = Mathf.Lerp(data.fz, -transform.forward.z, Time.deltaTime * m_InterpolateSpeed);

                data.ux = Mathf.Lerp(data.ux, transform.up.x, Time.deltaTime * m_InterpolateSpeed);
                data.uy = Mathf.Lerp(data.uy, transform.up.y, Time.deltaTime * m_InterpolateSpeed);
                data.uz = Mathf.Lerp(data.uz, transform.up.z, Time.deltaTime * m_InterpolateSpeed);

                data.rx = Mathf.Lerp(data.rx, -transform.right.x, Time.deltaTime * m_InterpolateSpeed);
                data.ry = Mathf.Lerp(data.ry, -transform.right.y, Time.deltaTime * m_InterpolateSpeed);
                data.rz = Mathf.Lerp(data.rz, -transform.right.z, Time.deltaTime * m_InterpolateSpeed);

                data.x = Mathf.Lerp(data.x, transform.position.x, Time.deltaTime * m_InterpolateSpeed);
                data.y = Mathf.Lerp(data.y, transform.position.y, Time.deltaTime * m_InterpolateSpeed);
                data.z = Mathf.Lerp(data.z, transform.position.z, Time.deltaTime * m_InterpolateSpeed);

                byte[] bytesData = getBytes(data);
                m_s.Send(bytesData);
            }
        }
    }


    /// <summary>
    /// Конвертируем структуру в байты
    /// </summary>
    /// <param name="str"></param>
    /// <returns>Принимает структуры. WOPGameData, GameData и т.д.</returns>
    private byte[] getBytes(object str)
    {
        int size = Marshal.SizeOf(str);
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(str, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    #region GameData struct

    /// <summary>
    /// Пакет данных для флая
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    struct WOPGameData
    {
        [FieldOffset(0)]         public float x;
        [FieldOffset(4)]         public float y;
        [FieldOffset(8)]         public float z;
        [FieldOffset(12)]        public float fx;
        [FieldOffset(16)]        public float fy;
        [FieldOffset(20)]        public float fz;
        [FieldOffset(24)]        public float ux;
        [FieldOffset(28)]        public float uy;
        [FieldOffset(32)]        public float uz;
        [FieldOffset(36)]        public float lx;
        [FieldOffset(40)]        public float ly;
        [FieldOffset(44)]        public float lz;

        [FieldOffset(48)]        public float roll;
        [FieldOffset(52)]        public float pitch;
        [FieldOffset(56)]        public float yaw;
        [FieldOffset(60)]        public float longitudinal_accel;
        [FieldOffset(64)]        public float lateral_accel;
        [FieldOffset(68)]        public float vertical_accel;

        [FieldOffset(72)]        public float altitude;         // высота полета
        [FieldOffset(76)]        public float speed;            // скорость полета
        [FieldOffset(80)]        public float shake_level;      // уровень тряски (если такого параметра нет, то тряску
                                                                // должны включать в себя ускорения игрока)

        [FieldOffset(84)]        public bool gear_down;       // шасси выпущены
        [FieldOffset(85)]        public bool engine_started;  // двигатель заведен
        [FieldOffset(86)]        public bool autopilot;       // автопилот включен/выключен
        [FieldOffset(88)]        public float flaps;          // положение закрылок, например, в процентах
        [FieldOffset(92)]        public float power;          // мощность двигателя в процентах
        [FieldOffset(96)]        public int damage_flag;      // битовые флаги повреждений самолета (или процент повреждения)

        [FieldOffset(100)]       public bool alive;           // игрок жив или мертв
        [FieldOffset(104)]       public int killed_enemies;   // количество сбитых вражеских самолетов
        [FieldOffset(108)]       public int killed_allies;    // количество сбитых своих самолетов
    }

    /// <summary>
    /// Пакет данных для Телеги и Машинки
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct GameData
    {
        [FieldOffset(0)]         public float fx;
        [FieldOffset(4)]         public float fy;
        [FieldOffset(8)]         public float fz;
        [FieldOffset(12)]        public float rx;
        [FieldOffset(16)]        public float ry;
        [FieldOffset(20)]        public float rz;
        [FieldOffset(24)]        public float ux;
        [FieldOffset(28)]        public float uy;
        [FieldOffset(32)]        public float uz;
        [FieldOffset(36)]        public float x;
        [FieldOffset(40)]        public float y;
        [FieldOffset(44)]        public float z;
        [FieldOffset(48)]        public double dt;
    };

    #endregion GameData struct
}