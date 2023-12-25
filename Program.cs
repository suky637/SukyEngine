using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;

using System;
using SukyEngine;

namespace RogueLikeGame
{
    class main
    {

        static void Main(string[] args)
        {
            GameWindow window = new GameWindow();

            Camera camera = new Camera(window);

            PlayerMovement playerMovement = new PlayerMovement();
            playerMovement.AttachWindow(window);

            playerMovement.playerObj = new Sprite(new Texture("Ressources/placeholder.png"));
            playerMovement.cam = camera;

            SukyEngineSetup.AttachAll(window);
            RectangleShape bg = new RectangleShape(new Vector2f(window.getWindow().Size.X, window.getWindow().Size.Y));
            bg.FillColor = Color.White;
            while (window.shouldWindowClose())
            {
                window.pollEvents();
                window.clear();
                bg.Size = new Vector2f(window.getWindow().Size.X, window.getWindow().Size.Y);
                window.getWindow().Draw(bg);
                window.getWindow().Draw(playerMovement.playerObj);
                window.render();
            }
        }
    }
}