using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;

namespace SukyEngine
{
    public class Vector2
    {
        public float x;
        public float y;
        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public float magnitude { get { return MathF.Sqrt(x*x + y*y); } }
        public Vector2 normalized { get { var m = MathF.Sqrt(this.x * this.x + this.y * this.y); if (m != 0) { return new Vector2(this.x / m, this.y / m); } else return new Vector2(0, 0); } }

        public Vector2(float x, float y)
        {
            this.x = x; this.y = y;
        }

        public Vector2f ToSFML()
        {
            return new Vector2f(this.x, this.y);
        }

        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ")";
        }

        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator*(Vector2 a, float d)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator/(Vector2 a, float d)
        {
            return new Vector2(a.x / d, a.y / d);
        }

        public static Vector2 operator+(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator-(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static bool operator==(Vector2 a, Vector2 b)
        {
            return (a.x == b.x) && (a.y == b.y) && (a.magnitude == b.magnitude);
        }
        public static bool operator!=(Vector2 a, Vector2 b)
        {
            return (a.x != b.x) || (a.y != b.y) || (a.magnitude != b.magnitude);
        }

        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x < b.x || a.x == b.x ? a.x : b.x, a.y < b.y || a.y == b.y ? a.y : b.y);
        }

        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x > b.x || a.x == b.x ? a.x : b.x, a.y > b.y || a.y == b.y ? a.y : b.y);
        }

        public void Normalize()
        {
            var m = MathF.Sqrt(this.x*this.x + this.y*this.y);
            if (m != 0)
            {
                this.x /= m;
                this.y /= m;
            }
            else
            {
                this.x = 0;
                this.y = 0;
            }
        }

        public static Vector2 zero { get; } = new Vector2(0, 0);
        public static Vector2 down { get; } = new Vector2(0, 1);
        public static Vector2 left { get; } = new Vector2(-1, 0);
        public static Vector2 one { get; } = new Vector2(1, 1);
        public static Vector2 right { get; } = new Vector2(1, 0);
        public static Vector2 up { get; } = new Vector2(0, -1);
    }
    public abstract class Behaviour
    {
        public void AttachWindow(GameWindow window)
        {
            window.onWindowUpdate += this.Update;
            window.onWindowFirstFrame += this.Start;
            window.onWindowFixedFrame += this.FixedUpdate;
            
        }

        public abstract void Start();

        public abstract void Update();

        public abstract void FixedUpdate();

        
    }

    public static class SukyEngineSetup
    {
        public static void AttachAll(GameWindow window)
        {
            Input.AttachWindow(window);
            Time.AttachWindow(window);
        }
    }

    public class Camera
    {
        public Vector2 position = new Vector2(0, 0);
        public GameWindow window;
        public float Size = 100.0f;
        
        public Camera(GameWindow window)
        {
            this.window = window;
        }
    }

    public static class Converter
    {
        public static Vector2 WorldToScreenPoint(Vector2 point, Camera camera)
        {
            Vector2 returnV = new Vector2(0, 0);
            returnV.Set(camera.window.getWindow().Size.X / 2 + point.x * (camera.window.getWindow().Size.Y / camera.Size), camera.window.getWindow().Size.Y / 2 + point.y * (camera.window.getWindow().Size.Y / camera.Size));
            return returnV;
        }

        public static Vector2 ScreenToWorldPoint(Vector2 point, Camera camera)
        {
            Vector2 returnV = new Vector2(0, 0);
            returnV.Set((point.x - camera.window.getWindow().Size.X / 2) / (camera.window.getWindow().Size.Y / camera.Size), (point.y - camera.window.getWindow().Size.Y / 2) / (camera.window.getWindow().Size.Y / camera.Size));
            return returnV;
        }

        public static Vector2 WorldToScreenSize(Vector2 size, Camera camera)
        {
            Vector2 returnV = Vector2.one;
            returnV *= camera.window.getWindow().Size.Y / camera.Size;
            return returnV;
        }
    }

    public static class Time
    {
        static Clock startTime = new Clock();
        static Clock _deltaTimeTimer = new Clock();

        static float curTime = 0;

        static public float time { get { return startTime.ElapsedTime.AsSeconds(); } }
        static int _frameCount = 0;
        static public int frameCount { get { return _frameCount; } }

        static float _deltaTime = 0;
        static public float deltaTime { get { return _deltaTime; } }
        static public float fixedDeltaTime { get { return 1.0f / 60.0f; } }

        static List<GameWindow> windows = new List<GameWindow>();

        public static void AttachWindow(GameWindow window)
        {
            window.onWindowUpdate += Update;
            window.onWindowRender += LateUpdate;
            window.onWindowFirstFrame += Start;
            
            windows.Add(window);
        }

        static void Start()
        {
            startTime.Restart();
        }

        static void Update()
        {
            _frameCount++;
            _deltaTimeTimer.Restart();
            if (curTime >= fixedDeltaTime)
            {
                curTime = 0;
                foreach (var w in windows)
                {
                    w.callFixedFrame();
                }
            }
        }

        static void LateUpdate()
        {
            _deltaTime = _deltaTimeTimer.ElapsedTime.AsSeconds();
            curTime += _deltaTime;
        }
    }
}
