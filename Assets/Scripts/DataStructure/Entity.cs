public abstract class Entity {
	public float Health { get; set; }

	protected Entity()
		=> this.Health = 100f;
}
