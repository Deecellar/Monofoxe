﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Engine;



namespace Monofoxe.Engine
{
	public class Objects
	{
		
		/// <summary>
		/// List of all game objects.
		/// </summary>
		private static List<GameObj> _gameObjects = new List<GameObj>();

		/// <summary>
		/// List of newly created game objects. Since it won't be that cool to modify main list 
		/// in mid-step, they'll be added in next one.
		/// </summary>
		private static List<GameObj> _newGameObjects = new List<GameObj>();

		/// <summary>
		/// List of objects that should be destroyed in next step.
		/// </summary>
		private static List<GameObj> _destroyedGameObjects = new List<GameObj>();

		private static double _fixedUpdateAl = 0;


		public static void Update(GameTime gameTime)
		{
			// Clearing main list from destroyed objects.
			for(var i = 0; i < _destroyedGameObjects.Count; i += 1)
			{_gameObjects.Remove(_destroyedGameObjects[i]);}
			_destroyedGameObjects.Clear();
			// Clearing main list from destroyed objects.


			// Adding new objects to the list.
			_gameObjects.AddRange(_newGameObjects);
			_newGameObjects.Clear();
			// Adding new objects to the list.


			// Fixed updates.
			_fixedUpdateAl += gameTime.ElapsedGameTime.TotalSeconds;

			if (_fixedUpdateAl >= GameCntrl.FixedUpdateRate)
			{
				var overflow = (int)(_fixedUpdateAl / GameCntrl.FixedUpdateRate); // In case of lags.
				_fixedUpdateAl -= GameCntrl.FixedUpdateRate * overflow;	
				
				foreach(GameObj obj in _gameObjects)
				{
					if (obj.Active)
					{obj.FixedUpdateBegin();}
				}

				foreach(GameObj obj in _gameObjects)
				{
					if (obj.Active)
					{obj.FixedUpdate();}
				}

				foreach(GameObj obj in _gameObjects)
				{
					if (obj.Active)
					{obj.FixedUpdateEnd();}
				}
			}
			// Fixed updates.


			// Normal updates.
			foreach(GameObj obj in _gameObjects)
			{
				if (obj.Active)
				{obj.UpdateBegin();}
			}

			foreach(GameObj obj in _gameObjects)
			{
				if (obj.Active)
				{obj.Update();}
			}
		
			foreach(GameObj obj in _gameObjects)
			{
				if (obj.Active)
				{obj.UpdateEnd();}
			}
			// Normal updates.

		}
		

		public static void Draw()
		{
				
			var depthSortedObjects = _gameObjects; // Replace!

			foreach(Camera camera in DrawCntrl.Cameras)
			{
			
				DrawCntrl.Device.SetRenderTarget(camera.ViewSurface);
				
				DrawCntrl.Device.Clear(Color.Transparent);

				DrawCntrl.Batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.CreateTranslationMatrix());

				foreach(GameObj obj in depthSortedObjects)
				{
					if (obj.Active)
					{obj.DrawBegin();}
				}

				foreach(GameObj obj in depthSortedObjects)
				{
					if (obj.Active)
					{obj.Draw();}
				}
			
				foreach(GameObj obj in depthSortedObjects)
				{
					if (obj.Active)
					{obj.DrawEnd();}
				}
			
				DrawCntrl.Batch.End();

			}
			
			DrawCntrl.Device.SetRenderTarget(null);
			
			DrawCntrl.Device.Clear(Color.YellowGreen);
			
			foreach(Camera camera in DrawCntrl.Cameras)
			{
				DrawCntrl.Batch.Begin();
				DrawCntrl.Batch.Draw(camera.ViewSurface, new Vector2(camera.ViewportX, camera.ViewportY), Color.White);
				DrawCntrl.Batch.End();
			}
			
		}


		/// <summary>
		/// Adds object to object list.
		/// </summary>
		/// <param name="obj"></param>
		public static void AddObject(GameObj obj)
		{_newGameObjects.Add(obj);}


		#region user functions 

		/// <summary>
		/// Returns list of objects of certain type.
		/// </summary>
		/// <typeparam name="T">Object type.</typeparam>
		public static List<T> GetList<T>()
		{return (List<T>)(_gameObjects.OfType<T>());}


		/// <summary>
		/// Counts amount of objects of certain type.
		/// </summary>
		/// <typeparam name="T">Object type.</typeparam>
		/// <returns>Returns amount of objects.</returns>
		public static int Count<T>()
		{return _gameObjects.OfType<T>().Count();}


		/// <summary>
		/// Destroys game object.
		/// </summary>
		/// <param name="obj">Game object.</param>
		public static void Destroy(GameObj obj)
		{
			if (!_destroyedGameObjects.Contains(obj))
			{_destroyedGameObjects.Add(obj);}
		}


		/// <summary>
		/// Checks if given instance exists.
		/// </summary>
		/// <param name="obj">Object to check.</param>
		public static bool ObjExists(GameObj obj)
		{
			foreach(GameObj obj1 in _gameObjects)
			{
				if (obj == obj1)
				{return true;}
			}
			return false;
		}


		/// <summary>
		/// Finds n-th object of given type.
		/// </summary>
		/// <typeparam name="T">Type to search.</typeparam>
		/// <param name="count">Number of the object in object list.</param>
		/// <returns>Returns object if it was found, or null, if it wasn't.</returns>
		public static GameObj ObjFind<T>(int count)
		{
			int counter = 0;

			foreach(GameObj obj in _gameObjects)
			{
				if (counter >= count && obj is T)
				{return obj;}
				counter += 1;
			}
			return null;
		}

		#endregion user functions 

	}
}
