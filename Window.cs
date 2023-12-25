using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;

namespace SukyEngine
{

    public delegate void OnWindowUpdate();
    public delegate void OnWindowFirstFrame();
    public delegate void OnWindowFixedFrame();
    public class GameWindow
    {
        public event OnWindowUpdate? onWindowUpdate;
        public event OnWindowUpdate? onWindowRender;
        public event OnWindowFirstFrame? onWindowFirstFrame;
        public event OnWindowFixedFrame? onWindowFixedFrame;
        private RenderWindow renderWindow;
        bool isOpen = true;
        bool firstFrame = true;
        void onClose(object? sender, EventArgs e)
        {
            isOpen = false;
            renderWindow.Close();
        }

        public GameWindow() {

            renderWindow = new RenderWindow(new VideoMode(500, 500), "RogueLike");

            renderWindow.Closed += onClose;
            renderWindow.Resized += onResize;
        }

        private void onResize(object? sender, SizeEventArgs e)
        {
            View v = new View(new Vector2f(e.Width / 2, e.Height / 2), new Vector2f(e.Width, e.Height));
            renderWindow.SetView(v);
        }

        public void callFixedFrame()
        {
            if (onWindowFixedFrame != null)
                onWindowFixedFrame.Invoke();
        }

        public void pollEvents()
        {
            renderWindow.DispatchEvents();
            if (firstFrame)
            {
                if (onWindowFirstFrame != null)
                    onWindowFirstFrame.Invoke();
                firstFrame = false;
            }
            if (onWindowUpdate != null)
                onWindowUpdate.Invoke();
        }

        public RenderWindow getWindow()
        {
            return renderWindow;
        }

        public void clear()
        {
            renderWindow.Clear();
        }

        public void render()
        {
            renderWindow.Display();
            if (onWindowRender != null)
                onWindowRender.Invoke();
        }

        public bool shouldWindowClose()
        {
            return isOpen;
        }
    }
}
