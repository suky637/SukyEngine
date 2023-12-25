using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SukyEngine;

namespace RogueLikeGame
{
    class PlayerMovement : Behaviour
    {
        public Sprite playerObj;
        public Camera cam;
        Vector2 position = Vector2.zero;
        Vector2 scale = Vector2.one;
        public float speed = 45;

        public override void FixedUpdate()
        {
            playerObj.Position = Converter.WorldToScreenPoint(position, cam).ToSFML();
            playerObj.Scale = Converter.WorldToScreenSize(scale, cam).ToSFML();
        }

        public override void Start()
        {
            cam.Size = 200;
            
        }

        public override void Update()
        {
            var _x = Input.GetAxis("Horizontal");
            var _y = Input.GetAxis("Vertical");
            Vector2 velocity = new Vector2(_x, _y);

            velocity.Normalize();
            velocity *= Time.deltaTime * speed;

            position += velocity;

            
        }
    }
}
