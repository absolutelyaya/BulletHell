using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullets
{
    public class Petal : BulletBase
    {

        public Color[] PetalColors;

        SpriteRenderer sprite;

        void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            sprite.color = PetalColors[Random.Range(0, PetalColors.Length)];
        }

        public override void Move()
        {
            base.Move();
        }
    }
}
