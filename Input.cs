using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SukyEngine
{
    class Key
    {
        public bool isFirstDown { get; set; } = false;
        public bool isDown { get; set; } = false;
        public bool isReleased { get; set; } = false;
    }

    public enum AxisType
    {
        KEYBOARD,
        MOUSE
    }

    public class Axis
    {
        public List<int> negative = new List<int>();
        public List<int> positive = new List<int>();
        public AxisType type = AxisType.KEYBOARD;
        public float range = 0;
        public Axis() { }
        public Axis(int neg, int pos)
        {
            negative.Add(neg);
            positive.Add(pos);
        }
        public Axis(List<int> neg,  List<int> pos)
        {
            negative.AddRange(neg);
            positive.AddRange(pos);
        }
        public Axis(List<int> neg, List<int> pos, AxisType t)
        {
            negative.AddRange(neg);
            positive.AddRange(pos);
            type = t;
        }
        public Axis(AxisType type)
        {
            this.type = type;
        }
    }

    public static class Input
    {
        
        static private Dictionary<int, Key> keys = new Dictionary<int, Key>();
        static private Dictionary<int, Key> mouseButton = new Dictionary<int, Key>();
        static private Dictionary<string, Axis> axis = new Dictionary<string, Axis>
        {
            {
                "Horizontal",
                new Axis(
                    new List<int>
                    {
                        (int)Keyboard.Key.A,
                        (int)Keyboard.Key.Left
                    },
                    new List<int>
                    {
                        (int)Keyboard.Key.D,
                        (int)Keyboard.Key.Right
                    }
                )
            },
            {
                "Vertical",
                new Axis(
                    new List<int>
                    {
                        (int)Keyboard.Key.W,
                        (int)Keyboard.Key.Up
                    },
                    new List<int>
                    {
                        (int)Keyboard.Key.S,
                        (int)Keyboard.Key.Down
                    }
                )
            },
            {
                "Mouse X",
                new Axis(AxisType.MOUSE)
            },
            {
                "Mouse Y",
                new Axis(AxisType.MOUSE)
            }
        };

        // PUBLIC VARIABLES
        static private Vector2 LastMousePos = new Vector2(-1, -1);
        static private Vector2 MousePos = new Vector2(-1, -1);
        static public Vector2 mousePosition { get { return MousePos; } }

        static private bool AnyKey = false;
        static public bool anyKey { get { return AnyKey; } }

        static private bool AnyKeyDown = false;
        static public bool anyKeyDown { get { return AnyKeyDown; } }

        static public void addAxis(string name, Axis a)
        {
            axis.Add(name, a);
        }

        static public void AttachWindow(GameWindow window)
        {
            window.getWindow().KeyPressed += keyPress;
            window.getWindow().KeyReleased += keyRelease;
            window.getWindow().MouseButtonPressed += mousePress;
            window.getWindow().MouseButtonReleased += mouseReleased;
            window.getWindow().MouseMoved += mouseMoved;
            window.onWindowRender += LateUpdate;
            window.onWindowUpdate += Update;
        }

        private static void Update()
        {
            foreach (var el in axis)
            {
                // getting positives
                float neg = 0;
                float pos = 0;

                foreach (int key in axis[el.Key].negative)
                {
                    if (!keys.ContainsKey(key)) continue;
                    if (keys[key].isDown)
                        neg = 1;
                }
                foreach (int key in axis[el.Key].positive)
                {
                    if (!keys.ContainsKey(key)) continue;
                    if (keys[key].isDown)
                        pos = 1;
                }

                axis[el.Key].range = pos - neg;
            }
        }

        private static void mouseMoved(object? sender, MouseMoveEventArgs e)
        {
       
            MousePos.X = e.X;
            MousePos.Y = e.Y;
            axis["Mouse X"].range = ((MousePos - LastMousePos)).normalized.X;
            axis["Mouse Y"].range = ((MousePos - LastMousePos)).normalized.Y;
        }

        public static float GetAxis(string axisName)
        {
            return axis[axisName].range;
        }

        private static void LateUpdate()
        {
            AnyKeyDown = false;
            foreach (var k in keys)
            {
                int key = k.Key;
                keys[key].isReleased = false;
                keys[key].isFirstDown = false;
            }
            foreach (var m in mouseButton)
            {
                int mouse = m.Key;
                mouseButton[mouse].isReleased = false;
                mouseButton[mouse].isFirstDown = false;
            }
            LastMousePos.X = MousePos.X;
            LastMousePos.Y = MousePos.Y;
        }

        public static bool isKey(int keyCode)
        {
            if (keys.ContainsKey(keyCode))
                return keys[keyCode].isDown;
            return false;
        }

        public static bool isKeyUp(int keyCode)
        {
            if (keys.ContainsKey(keyCode))
                return keys[keyCode].isReleased;
            return false;
        }

        public static bool isKeyDown(int keyCode)
        {
            if (keys.ContainsKey(keyCode))
                return keys[keyCode].isFirstDown;
            return false;
        }

        public static bool isMouseButtonDown(int mouseBtn)
        {
            if (mouseButton.ContainsKey(mouseBtn))
                return keys[mouseBtn].isFirstDown;
            return false;
        }

        public static bool isMouseButtonUp(int mouseBtn)
        {
            if (mouseButton.ContainsKey(mouseBtn))
                return keys[mouseBtn].isReleased;
            return false;
        }

        public static bool isMouseButton(int mouseBtn)
        {
            if (mouseButton.ContainsKey(mouseBtn))
                return keys[mouseBtn].isDown;
            return false;
        }

        private static void mouseReleased(object? sender, MouseButtonEventArgs e)
        {
            if (!mouseButton.ContainsKey((int)e.Button))
                mouseButton.Add((int)e.Button, new Key());
            mouseButton[((int)e.Button)].isDown = false;
            mouseButton[((int)e.Button)].isReleased = true;
        }

        private static void mousePress(object? sender, MouseButtonEventArgs e)
        {
            if (!mouseButton.ContainsKey((int)e.Button))
                mouseButton.Add((int)e.Button, new Key());
            if (!mouseButton[(int)e.Button].isDown)
                mouseButton[(int)e.Button].isFirstDown = true;
            mouseButton[(int)e.Button].isDown = true;
        }

        private static void keyRelease(object? sender, KeyEventArgs e)
        {
            if (!keys.ContainsKey((int)e.Code))
                keys.Add((int)e.Code, new Key());
            AnyKey = false;
            keys[((int)e.Code)].isDown = false;
            keys[((int)e.Code)].isReleased = true;
        }

        private static void keyPress(object? sender, KeyEventArgs e)
        {
            if (!keys.ContainsKey((int)e.Code))
                keys.Add((int)e.Code, new Key());
            if (!keys[(int)e.Code].isDown)
            {
                AnyKeyDown = true;
                keys[(int)e.Code].isFirstDown = true;
            }
            AnyKey = true;
            keys[(int)e.Code].isDown = true;
        }
    }
}
