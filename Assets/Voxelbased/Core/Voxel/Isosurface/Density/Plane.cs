
namespace VoxelbasedCom
{
    /// <summary>
    /// Plane density.
    /// </summary>
	public class Plane : Density {
		
		private float height;

		public Plane(float height)
		{
			this.height = height;
		}
		
		public override float GetDensity(float x, float y, float z)
		{


			return y - height;
		}
	}
}