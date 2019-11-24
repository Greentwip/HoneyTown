using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HarvestMoon.Entities
{
    public interface IEntityManager
    {
        T AddEntity<T>(T entity) where T : Entity;
    }

    [Serializable]
    public class EntityManager : IEntityManager
    {
        protected List<Entity> _entities;
        protected readonly List<Entity> _submittedEntities;

        public List<Entity> Entities { get => _entities; set => _entities = value; }

        public EntityManager()
        {
            _entities = new List<Entity>();
            _submittedEntities = new List<Entity>();
        }

        public T AddEntity<T>(T entity) where T : Entity
        {
            _entities.Add(entity);
            return entity;
        }

        public T SubmitEntity<T>(T entity) where T : Entity
        {
            _submittedEntities.Add(entity);
            return entity;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in _entities.Where(e => !e.IsDestroyed))
            {
                entity.Update(gameTime);
            }

            _entities.RemoveAll(e => e.IsDestroyed);

            foreach(var entity in _submittedEntities)
            {
                _entities.Add(entity);
            }

            _submittedEntities.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in _entities.Where(e => !e.IsDestroyed).OrderBy(e => e.Priority))
            {
                entity.Draw(spriteBatch);
            }
        }
    }
}
