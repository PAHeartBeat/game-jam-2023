public interface IObstacles : IShapeInfo {
	float DestroyTime { get; }
	void CheckShape(IBlockController shape);
	void DestroyObstacle(bool wasByBot);
}
