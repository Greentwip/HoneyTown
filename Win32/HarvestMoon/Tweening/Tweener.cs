using HarvestMoon.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Tweening
{
    class Tween
    {
        private Interactable _target;
        private Vector2 _positionTarget;
        private float _duration;
        private float _elapsed;

        private Action _onBegin;
        private Action _onComplete;

        public Tween(Interactable target, Vector2 positionTarget, float duration)
        {
            _target = target;
            _positionTarget = positionTarget;
            _duration = duration;
        }

        public void OnBegin(Action onBegin)
        {
            _onBegin = onBegin;
        }

        public void OnComplete(Action onComplete)
        {
            _onComplete = onComplete;
        }

        public bool Finished { get; set; }

        public void Update(float elapsed)
        {
            _elapsed += elapsed;

            if(_elapsed == elapsed)
            {
                _onBegin?.Invoke();
            }

            if(_elapsed >= _duration)
            {
                _target.X = _positionTarget.X;
                _target.Y = _positionTarget.Y;

                _onComplete?.Invoke();
                Finished = true;
            }
            else
            {
                _target.X = _target.X + (_positionTarget.X  - _target.X )* (_elapsed / _duration);
                _target.Y = _target.Y + (_positionTarget.Y - _target.Y) * (_elapsed / _duration);
            }
        }
    }

    class Tweener
    {
        private List<Tween> _tweens = new List<Tween>();

        public bool Tweening => _tweens.Count > 0;

        public Tween Tween(Interactable target, Vector2 targetPosition, float duration)
        {

            var newTween = new Tween(target, targetPosition, duration);
            _tweens.Add(newTween);
            return newTween;
        }

        public void Update(float deltaTime)
        {
            List<Tween> toRemove = new List<Tween>();

            foreach(var tween in _tweens)
            {
                tween.Update(deltaTime);

                if (tween.Finished)
                {
                    toRemove.Add(tween);
                }
            }
           
            foreach(var tween in toRemove)
            {
                _tweens.Remove(tween);
            }
        }

    }
}
