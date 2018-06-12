using RLGame.Interfaces;
using RLGame.Systems;
using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLGame.Bodyparts;
using RLGame.Actions.BaseActions;
using RLGame.GameStates;

namespace RLGame.Core
{
	public class Actor : IActor, IDrawable, IScheduleable, IUpdateable
	{
		public Actor() {
			
			Update = GameController.SchedulingSystem.update;
			Update.UpdateEvent += OnUpdateEvent;

			_bodyparts = new List<Bodypart>();
			Actions = new List<BaseAction>();
		}
		//IActor
		private int _awareness;
		private string _name;
		private int _initiative;
		private int _speed;
		private double _regen;
		private List<Bodypart> _bodyparts;
		public BaseAction LastAction { get; set; }
		public List<BaseAction> Actions { get; set; }

		public List<Bodypart> Bodyparts {
			get {
				return _bodyparts;
			}
			set {
				_bodyparts = value;
			}
		}

		public double Regen {
			get {
				return _regen;
			}
			set {
				_regen = value;
			}
		}

		public int Strength {
			get {
				Arm a = (Arm) GetBodypart( BodypartType.Arm );
				if(a != null )
				{
					return a.Strength;
				}
				Head h = (Head) GetBodypart( BodypartType.Head );
				return h.Strength;
			}
		}

		public int Awareness {
			get {
				return _awareness;
			}
			set {
				_awareness = value;
			}
		}

		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}

		public int Initiative {
			get {
				return _initiative;
			}
			set {
				_initiative = value;
			}
		}

		public int Speed {
			get {
				return _speed;
			}
			set {
				_speed = value;
			}
		}

		public int Size {
			get{
				int size = 0;
				foreach ( Bodypart bodypart in Bodyparts )
				{
					size += bodypart.Size;
				}
				return size;
			}
		}

		public virtual Bodypart TakeDamage( int damage, Bodypart bodypart ) {
			if ( bodypart.Health > 0 )
			{
				bodypart.Health -= damage;
			}
			else
			{
				bodypart = GetVitalBodypart( false );
				bodypart.Health -= damage;

			}
			return bodypart;
		}

		public bool IsHurt() {
			bool isHurt = false;
			foreach (Bodypart bodypart in Bodyparts)
			{
				if ( bodypart.Health < bodypart.MaxHealth )
					isHurt = true;
			}
			return isHurt;
		}

		public virtual bool IsDying() {
			foreach ( Bodypart bodypart in _bodyparts )
			{
				if ( bodypart.IsVital && bodypart.Health <= 0 )
				{
					return true;
				}
			}
			return false;
		}

		public Bodypart GetBodypart( BodypartType type) {
			return Bodyparts.Find( bodypart => bodypart.partType == type );
		}

		public Bodypart GetBodypart( string bodypartName ) {
			return Bodyparts.Find( bodypart => bodypart.Name == bodypartName );
		}

		public Bodypart GetBodypart( bool sizeWeighted ) {
			Bodypart bodypart = null;
			if ( sizeWeighted )
			{
				bodypart = GetWeightedBodypart( bodypart, Bodyparts );
			}
			else
			{
				bodypart = Bodyparts.ElementAt( Game.Random.Next( Bodyparts.Count() - 1 ) );
			}
			return bodypart;
		}

		public Bodypart GetVitalBodypart( bool sizeWeighted ) {
			Bodypart vitalBodypart = null;
			if ( sizeWeighted )
			{
				List<Bodypart> vitalBodyparts = Bodyparts.FindAll( bodypart => bodypart.IsVital == true );
				vitalBodypart = GetWeightedBodypart( vitalBodypart, vitalBodyparts );
			}
			else
			{
				List<Bodypart> vitalBodyparts = Bodyparts.FindAll( bodypart => bodypart.IsVital == true );
				vitalBodypart = vitalBodyparts.ElementAt( Game.Random.Next( vitalBodyparts.Count() - 1 ) );
			}
			return vitalBodypart;
		}

		//IScheduleable
		public int Time {
			get {
				return Speed;
			}
		}
		public bool History {
			get {
				return true;
			}
		}

		//IDrawable
		public RLColor Color { get; set; }
		public char Symbol { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public void Draw( RLConsole console, IMap map ) {
			if ( !map.GetCell( X, Y ).IsExplored )
			{
				return;
			}
			if ( map.IsInFov( X, Y ) )
			{
				console.Set( X, Y, Color, Colors.FloorBackgroundFov, Symbol );
			}
			else
			{
				console.Set( X, Y, Colors.Floor, Colors.FloorBackground, '.' );
			}
		}



		//IUpdateable
		public Update Update;
		virtual public void OnUpdateEvent( object sender, EventArgs e ) {
			//Console.WriteLine("Updated my journal.");
		}

		//Helpers
		public void UpdateTimeline( Timeline timeline, IMap map ) {
			if ( map.IsInFov( X, Y ) )
			{
				if ( !timeline.VisibleActors.Contains( this ) )
				{
					timeline.VisibleActors.Add( this );
				}
			}
			else
			{
				if ( timeline.VisibleActors.Contains( this ) )
				{
					timeline.VisibleActors.Remove( this );
				}
			}
		}

		private Bodypart GetWeightedBodypart( Bodypart bodypart, List<Bodypart> bodyparts ) {
			foreach ( Bodypart b in bodyparts )
			{
				Game.WeightedRandom.Add( b, b.Size );
			}
			bodypart = Game.WeightedRandom.NextWithReplacement();
			foreach ( Bodypart b in bodyparts )
			{
				Game.WeightedRandom.Remove( b );
			}
			return bodypart;
		}

	}
}