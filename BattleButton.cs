using Godot;
using System;
using System.Threading.Tasks;

public partial class BattleButton : TextureButton
{
	protected BattleActor data = null;
	protected AnimatedSprite2D sprite;
	public BattleActor GetData(){
		return data;
	}
	public void Initialize(BattleActor actor){
		this.data = actor;
		this.data.HpChanged += OnHpChanged; 
	}
	public AnimatedSprite2D getSprite(){
		return sprite;
	}
	public virtual void OnHpChanged(int hp, int change){
		Label hit_text = new Label();
		hit_text.Text = Math.Abs(change).ToString();
		hit_text.Position = new Vector2(55, -40);
		AddChild(hit_text);
		Tween tween = CreateTween();
		//tween.SetParallel(true);
		tween.TweenProperty(hit_text, "position", hit_text.Position + new Vector2(0, -30), 0.7f);
		tween.TweenProperty(hit_text, "modulate", new Color(1,1,1,0), 0.5f);
		tween.TweenCallback(Callable.From(hit_text.QueueFree));
		if(!data.isAlive)
		{
			this.Disabled = true;
			if(sprite != null){
				sprite.Modulate = new Color(0.5f, 0.5f, 0.5f);
			}			
		}
	}
		public async Task PlayAttackAnimation(Vector2 direction, float distance = 50.0f, float duration = 0.2f){
			Vector2 startPosition = Position;
			Vector2 lungePosition = startPosition + direction * distance;
			Tween tween = CreateTween();
			tween.SetEase(Tween.EaseType.OutIn);
			tween.TweenProperty(this, "position", lungePosition, duration);
			tween.TweenProperty(this, "position", startPosition, duration);
			await ToSignal(tween, Tween.SignalName.Finished);
		}
	public void PlayVFX(Color effectColor, float duration){
		if(sprite == null || !IsInstanceValid(sprite)) return;
		Color originalColor = sprite.Modulate;
		Tween tween = CreateTween();
		tween.SetParallel(false);
		tween.TweenProperty(sprite, "modulate", effectColor, duration * 0.5f);
		tween.TweenProperty(sprite, "modulate", originalColor, duration * 0.5f);
	}
		
}
