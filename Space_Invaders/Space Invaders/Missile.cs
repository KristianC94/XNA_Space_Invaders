using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    class Missile
    {
        Vector2 MissilePos;

        public Missile(int InitialXPos, int InitialYPos) // Set missile position
        {
            MissilePos = new Vector2(InitialXPos, InitialYPos);
        }

        public void Move()  // Missile movement (upward)
        {
            MissilePos.Y = MissilePos.Y - 8;
        }

        public Vector2 GetMissilePos() // Update missile position
        {
            return MissilePos;
        }
    }
}