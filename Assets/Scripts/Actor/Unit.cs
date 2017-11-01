using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    abstract public class Unit : Actor
    {
        // is dead?
        protected bool _isDead;
        public bool isDead { get { return _isDead; } }

        // health
        public int health = 0;
        protected int _healthMax = 0;
        public int healthMax { get { return _healthMax; } }

        // attribute
        public int attack = 0;
        public int defence = 0;

        private void Awake()
        {
            _healthMax = health;
        }
    }
}
